using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CardDataAsset", menuName = "MTG/Card Data Asset")]
public class CardDataAsset : ScriptableObject
{
    [System.Serializable]
    public class SerializedCardData
    {
        public string id;
        public string name;
        public string type;
        public string manaCost;
        public string text;
        public string power;
        public string toughness;
        public string rarity;
        public bool isLegal;
        public int atlasIndex;
        public Vector4 uvRect;
    }

    [Header("Format Information")]
    public string formatName;
    public string formatVersion;
    public string lastUpdated;

    [Header("Card Data")]
    public SerializedCardData[] cards;

    // 高速検索用のインデックス（実行時に初期化）
    private Dictionary<string, SerializedCardData> cardIndex;

    public void Initialize(List<CardData> sourceCards)
    {
        // カードデータの変換と保存
        cards = new SerializedCardData[sourceCards.Count];
        for (int i = 0; i < sourceCards.Count; i++)
        {
            cards[i] = ConvertToSerializedData(sourceCards[i]);
        }

        formatVersion = System.DateTime.Now.ToString("yyyyMMdd");
        lastUpdated = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    private SerializedCardData ConvertToSerializedData(CardData source)
    {
        return new SerializedCardData
        {
            id = source.id,
            name = source.name,
            type = source.type,
            manaCost = source.manaCost,
            text = source.text,
            power = source.power,
            toughness = source.toughness,
            rarity = source.rarity,
            isLegal = source.isLegal
            // atlasIndexとuvRectは別途設定
        };
    }

    // 実行時の初期化
    public void InitializeRuntime()
    {
        cardIndex = new Dictionary<string, SerializedCardData>();
        foreach (var card in cards)
        {
            cardIndex[card.id] = card;
        }
    }

    // カードデータの取得
    public SerializedCardData GetCard(string cardId)
    {
        if (cardIndex == null)
        {
            InitializeRuntime();
        }

        if (cardIndex.TryGetValue(cardId, out SerializedCardData card))
        {
            return card;
        }
        return null;
    }

    // カードリストの取得
    public SerializedCardData[] GetAllCards()
    {
        return cards;
    }

    // レアリティによるフィルタリング
    public SerializedCardData[] GetCardsByRarity(string rarity)
    {
        return System.Array.FindAll(cards, card => card.rarity == rarity);
    }

    // タイプによるフィルタリング
    public SerializedCardData[] GetCardsByType(string type)
    {
        return System.Array.FindAll(cards, card => card.type.Contains(type));
    }

    // 使用可能なカードのみを取得
    public SerializedCardData[] GetLegalCards()
    {
        return System.Array.FindAll(cards, card => card.isLegal);
    }

    // マナコストによるフィルタリング
    public SerializedCardData[] GetCardsByManaCost(int cost)
    {
        return System.Array.FindAll(cards, card => CalculateManaCost(card.manaCost) == cost);
    }

    private int CalculateManaCost(string manaCost)
    {
        // マナコスト文字列からCMCを計算
        // 例: {2}{B}{R} → 4
        if (string.IsNullOrEmpty(manaCost))
            return 0;

        int totalCost = 0;
        string[] costs = manaCost.Split('}');
        
        foreach (string cost in costs)
        {
            string cleanCost = cost.Trim('{', '}');
            if (string.IsNullOrEmpty(cleanCost))
                continue;

            if (int.TryParse(cleanCost, out int numericCost))
            {
                totalCost += numericCost;
            }
            else
            {
                // 色マナは1としてカウント
                totalCost += 1;
            }
        }

        return totalCost;
    }
}
