using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using System.Collections;
using System.Collections.Generic;

public class CardDataManager : UdonSharpBehaviour
{
    [Header("Card Data Storage")]
    [SerializeField] private string dataStoragePath = "CardData";
    [SerializeField] private int batchSize = 100;
    [SerializeField] private float requestDelay = 0.1f; // Scryfall APIの制限に対応

    // カードデータ構造
    [System.Serializable]
    public struct CardData
    {
        public string id;
        public string name;
        public string set;
        public string type;
        public string manaCost;
        public string text;
        public string power;
        public string toughness;
        public string imageUrl;
        public string rarity;
        public bool isLegal; // 各フォーマットでの使用可否
    }

    private Dictionary<string, CardData> cardDatabase = new Dictionary<string, CardData>();
    private Dictionary<string, string> cardNameToId = new Dictionary<string, string>();
    private Dictionary<string, Texture2D> cardTextures = new Dictionary<string, Texture2D>();

    [Header("Format Settings")]
    [SerializeField] private bool includeStandard = true;
    [SerializeField] private bool includePioneer = true;
    [SerializeField] private bool includeModern = true;
    [SerializeField] private bool includeLegacy = true;
    [SerializeField] private bool includeVintage = true;
    [SerializeField] private bool includePauper = true;

    // APIエンドポイント
    private const string SCRYFALL_API_BASE = "https://api.scryfall.com";
    private const string BULK_DATA_ENDPOINT = "/bulk-data";
    private const string CARDS_ENDPOINT = "/cards";
    private const string SEARCH_ENDPOINT = "/cards/search";

    void Start()
    {
        // データの初期化（実際のAPIリクエストはMCP経由で行う）
        InitializeCardDatabase();
    }

    private void InitializeCardDatabase()
    {
        // 以下はMCPサーバーで実装される処理の概要
        /*
        1. バルクデータの取得
        GET https://api.scryfall.com/bulk-data

        2. 最新のカードデータJSONのダウンロード
        GET [bulk_data_uri]

        3. フォーマット別のカードリストの取得
        GET https://api.scryfall.com/cards/search?q=format:standard+legal:standard （例：スタンダード）
        GET https://api.scryfall.com/cards/search?q=format:pioneer+legal:pioneer （例：パイオニア）
        */

        // カード名からIDへのマッピングを作成
        foreach (var card in cardDatabase)
        {
            if (!cardNameToId.ContainsKey(card.Value.name.ToLower()))
            {
                cardNameToId.Add(card.Value.name.ToLower(), card.Key);
            }
        }
    }

    public string FindCardIdByName(string cardName)
    {
        string normalizedName = cardName.ToLower();
        return cardNameToId.ContainsKey(normalizedName) ? cardNameToId[normalizedName] : "";
    }

    // カード情報の取得
    public CardData GetCardData(string cardId)
    {
        if (cardDatabase.ContainsKey(cardId))
        {
            return cardDatabase[cardId];
        }
        return new CardData();
    }

    // カードテクスチャの取得
    public Texture2D GetCardTexture(string cardId)
    {
        if (cardTextures.ContainsKey(cardId))
        {
            return cardTextures[cardId];
        }
        return null;
    }

    // フォーマット別の合法カードリストを取得
    public string[] GetLegalCardsForFormat(string format)
    {
        List<string> legalCards = new List<string>();
        foreach (var card in cardDatabase)
        {
            if (card.Value.isLegal)
            {
                legalCards.Add(card.Key);
            }
        }
        return legalCards.ToArray();
    }

    // キャッシュ管理
    public void ClearTextureCache()
    {
        foreach (var texture in cardTextures.Values)
        {
            DestroyImmediate(texture);
        }
        cardTextures.Clear();
    }

    public void OptimizeMemoryUsage()
    {
        // 未使用のテクスチャを解放
        // メモリ使用量の最適化
        System.GC.Collect();
    }

    // フォーマットチェック
    public bool IsCardLegalInFormat(string cardId, string format)
    {
        if (!cardDatabase.ContainsKey(cardId))
            return false;

        // フォーマット別の制限チェック
        switch (format.ToLower())
        {
            case "standard":
                return includeStandard && cardDatabase[cardId].isLegal;
            case "pioneer":
                return includePioneer && cardDatabase[cardId].isLegal;
            case "modern":
                return includeModern && cardDatabase[cardId].isLegal;
            case "legacy":
                return includeLegacy && cardDatabase[cardId].isLegal;
            case "vintage":
                return includeVintage && cardDatabase[cardId].isLegal;
            case "pauper":
                return includePauper && cardDatabase[cardId].isLegal;
            default:
                return false;
        }
    }
}
