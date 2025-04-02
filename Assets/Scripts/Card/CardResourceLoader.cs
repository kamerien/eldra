using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System.Collections.Generic;

public class CardResourceLoader : UdonSharpBehaviour
{
    [Header("Resource Settings")]
    [SerializeField] private string cardDataPath = "CardData";
    [SerializeField] private string textureAtlasPath = "CardTextures";
    [SerializeField] private int maxCachedTextures = 1000;

    [Header("Memory Management")]
    [SerializeField] private int texturePoolSize = 100;
    [SerializeField] private float textureUnloadDelay = 300f; // 5分
    
    // キャッシュシステム
    private Dictionary<string, CardDataManager.CardData> cardDataCache;
    private Dictionary<string, Texture2D> textureCache;
    private Queue<string> textureCacheQueue;
    private Dictionary<string, float> lastAccessTime;

    // テクスチャアトラス参照
    private Texture2D[] textureAtlases;
    private Dictionary<string, Vector4> atlasUVMap; // xy: offset, zw: scale

    void Start()
    {
        InitializeCache();
        LoadAtlasData();
    }

    private void InitializeCache()
    {
        cardDataCache = new Dictionary<string, CardDataManager.CardData>();
        textureCache = new Dictionary<string, Texture2D>();
        textureCacheQueue = new Queue<string>();
        lastAccessTime = new Dictionary<string, float>();
    }

    private void LoadAtlasData()
    {
        // テクスチャアトラスとUVマップの読み込み
        TextAsset atlasMapData = Resources.Load<TextAsset>($"{textureAtlasPath}/atlas_map");
        if (atlasMapData != null)
        {
            // UVマップデータの解析と設定
            ParseAtlasMapData(atlasMapData.text);
        }

        // テクスチャアトラスの読み込み
        textureAtlases = Resources.LoadAll<Texture2D>(textureAtlasPath);
    }

    private void ParseAtlasMapData(string mapData)
    {
        atlasUVMap = new Dictionary<string, Vector4>();
        // UVマップデータのパース処理
        // フォーマット: cardId,atlasIndex,x,y,width,height
    }

    public CardDataManager.CardData LoadCardData(string cardId, string format)
    {
        // キャッシュチェック
        if (cardDataCache.ContainsKey(cardId))
        {
            return cardDataCache[cardId];
        }

        // リソースからカードデータを読み込み
        TextAsset cardDataAsset = Resources.Load<TextAsset>($"{cardDataPath}/{format}/cards");
        if (cardDataAsset != null)
        {
            // JSONデータのパースと保存
            CardDataManager.CardData cardData = ParseCardData(cardDataAsset.text, cardId);
            if (cardData.id != null)
            {
                cardDataCache[cardId] = cardData;
                return cardData;
            }
        }

        return new CardDataManager.CardData();
    }

    private CardDataManager.CardData ParseCardData(string jsonData, string targetCardId)
    {
        // JSON文字列からカードデータを抽出
        // 実際の実装ではJSONパーサーを使用
        return new CardDataManager.CardData();
    }

    public Texture2D GetCardTexture(string cardId)
    {
        // キャッシュチェック
        if (textureCache.ContainsKey(cardId))
        {
            UpdateTextureAccessTime(cardId);
            return textureCache[cardId];
        }

        // アトラスからテクスチャを取得
        if (atlasUVMap.ContainsKey(cardId))
        {
            Vector4 uvData = atlasUVMap[cardId];
            int atlasIndex = (int)uvData.x;
            if (atlasIndex < textureAtlases.Length)
            {
                Texture2D cardTexture = CreateCardTextureFromAtlas(
                    textureAtlases[atlasIndex],
                    uvData
                );
                CacheTexture(cardId, cardTexture);
                return cardTexture;
            }
        }

        return null;
    }

    private Texture2D CreateCardTextureFromAtlas(Texture2D atlas, Vector4 uvData)
    {
        // アトラスからカードテクスチャを切り出し
        int width = 336;  // MTGカードの標準サイズ
        int height = 468;
        
        Texture2D cardTexture = new Texture2D(width, height);
        // テクスチャの切り出しと設定
        // UV座標を使用してアトラスから該当部分を抽出

        return cardTexture;
    }

    private void CacheTexture(string cardId, Texture2D texture)
    {
        if (textureCacheQueue.Count >= maxCachedTextures)
        {
            // 古いテクスチャを削除
            string oldestCardId = textureCacheQueue.Dequeue();
            if (textureCache.ContainsKey(oldestCardId))
            {
                DestroyImmediate(textureCache[oldestCardId]);
                textureCache.Remove(oldestCardId);
                lastAccessTime.Remove(oldestCardId);
            }
        }

        textureCache[cardId] = texture;
        textureCacheQueue.Enqueue(cardId);
        UpdateTextureAccessTime(cardId);
    }

    private void UpdateTextureAccessTime(string cardId)
    {
        lastAccessTime[cardId] = Time.time;
    }

    public void CleanupUnusedTextures()
    {
        float currentTime = Time.time;
        List<string> textureToRemove = new List<string>();

        foreach (var access in lastAccessTime)
        {
            if (currentTime - access.Value > textureUnloadDelay)
            {
                textureToRemove.Add(access.Key);
            }
        }

        foreach (string cardId in textureToRemove)
        {
            if (textureCache.ContainsKey(cardId))
            {
                DestroyImmediate(textureCache[cardId]);
                textureCache.Remove(cardId);
                lastAccessTime.Remove(cardId);
            }
        }
    }

    void OnDisable()
    {
        // クリーンアップ処理
        foreach (var texture in textureCache.Values)
        {
            DestroyImmediate(texture);
        }
        textureCache.Clear();
        cardDataCache.Clear();
        textureCacheQueue.Clear();
        lastAccessTime.Clear();
    }
}
