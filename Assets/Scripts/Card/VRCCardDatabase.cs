using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRCCardDatabase : UdonSharpBehaviour
{
    [Header("Card Data Assets")]
    [SerializeField] private TextAsset[] formatCardData; // フォーマットごとのカードデータ
    [SerializeField] private Texture2D[] cardAtlases; // 事前生成されたテクスチャアトラス
    [SerializeField] private TextAsset cardUVMapData; // アトラスのUV座標データ

    [Header("Format Settings")]
    [SerializeField] private string[] supportedFormats = {
        "standard", "pioneer", "modern", "legacy", "vintage", "pauper"
    };

    private CardDataStore[] cardDataStores; // フォーマットごとのデータストア
    private AtlasUVMap[] uvMaps; // テクスチャアトラスのUV座標マップ

    // カードデータ構造
    private struct CardDataStore
    {
        public string format;
        public CardData[] cards;
    }

    // UV座標マップ構造
    private struct AtlasUVMap
    {
        public string cardId;
        public int atlasIndex;
        public Vector4 uvRect; // xy: offset, zw: scale
    }

    void Start()
    {
        InitializeCardDatabase();
    }

    private void InitializeCardDatabase()
    {
        // カードデータの初期化（ビルド時に含まれるアセットから）
        cardDataStores = new CardDataStore[formatCardData.Length];
        for (int i = 0; i < formatCardData.Length; i++)
        {
            if (formatCardData[i] != null)
            {
                cardDataStores[i] = ParseCardDataAsset(formatCardData[i]);
            }
        }

        // UVマップの初期化
        if (cardUVMapData != null)
        {
            ParseUVMapData(cardUVMapData);
        }
    }

    private CardDataStore ParseCardDataAsset(TextAsset asset)
    {
        // JSONデータのパース（ビルド時に最適化された形式で保存）
        CardDataStore store = new CardDataStore();
        // パース処理...
        return store;
    }

    private void ParseUVMapData(TextAsset uvData)
    {
        // UVマップデータのパース（ビルド時に最適化された形式で保存）
        // パース処理...
    }

    // 指定フォーマットのカードリストを取得
    public CardData[] GetFormatCards(string format)
    {
        for (int i = 0; i < cardDataStores.Length; i++)
        {
            if (cardDataStores[i].format == format)
            {
                return cardDataStores[i].cards;
            }
        }
        return new CardData[0];
    }

    // カードテクスチャの取得
    public Texture2D GetCardAtlas(int index)
    {
        if (index >= 0 && index < cardAtlases.Length)
        {
            return cardAtlases[index];
        }
        return null;
    }

    // カードのUV座標を取得
    public Vector4 GetCardUVRect(string cardId)
    {
        foreach (var uvMap in uvMaps)
        {
            if (uvMap.cardId == cardId)
            {
                return uvMap.uvRect;
            }
        }
        return Vector4.zero;
    }

    // カードのアトラスインデックスを取得
    public int GetCardAtlasIndex(string cardId)
    {
        foreach (var uvMap in uvMaps)
        {
            if (uvMap.cardId == cardId)
            {
                return uvMap.atlasIndex;
            }
        }
        return -1;
    }

    // フォーマットの有効性チェック
    public bool IsValidFormat(string format)
    {
        foreach (var supportedFormat in supportedFormats)
        {
            if (supportedFormat == format)
            {
                return true;
            }
        }
        return false;
    }
}
