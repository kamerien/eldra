using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System;

public class DeckDataManager : UdonSharpBehaviour
{
    [System.Serializable]
    public struct DeckData
    {
        public string name;
        public string[] mainDeckCards;
        public string[] sideboardCards;
        public string format;
        public bool isValid;
    }

    [Header("Dependencies")]
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private CardDataManager cardDataManager;

    [Header("Storage Settings")]
    [SerializeField] private string playerPrefsPrefix = "VRCDeck_";
    private DeckData currentDeck;

    void Start()
    {
        LoadDeck();
    }

    private void LoadDeck()
    {
        string deckJson = PlayerPrefs.GetString($"{playerPrefsPrefix}{Networking.LocalPlayer.playerId}", "");
        if (!string.IsNullOrEmpty(deckJson))
        {
            // JSON文字列からデッキデータを復元
            currentDeck = ParseDeckData(deckJson);
        }
        else
        {
            // 新規デッキを作成
            currentDeck = new DeckData
            {
                name = "新規デッキ",
                format = "standard",
                mainDeckCards = new string[0],
                sideboardCards = new string[0]
            };
        }
    }

    private DeckData ParseDeckData(string json)
    {
        // 簡易的なJSONパース（UdonSharpの制限により、完全なJSONパースは実装できません）
        DeckData deck = new DeckData();
        
        // 基本的な文字列分割でデータを抽出
        string[] lines = json.Split(new[] { '\n' });
        foreach (string line in lines)
        {
            string[] parts = line.Split(new[] { ':' });
            if (parts.Length != 2) continue;

            string key = parts[0].Trim();
            string value = parts[1].Trim();

            switch (key)
            {
                case "name":
                    deck.name = value;
                    break;
                case "format":
                    deck.format = value;
                    break;
                case "mainDeck":
                    deck.mainDeckCards = value.Split(new[] { ',' });
                    break;
                case "sideboard":
                    deck.sideboardCards = value.Split(new[] { ',' });
                    break;
            }
        }

        // デッキの有効性チェック
        deck.isValid = ValidateDeck(deck);
        return deck;
    }

    private bool ValidateDeck(DeckData deck)
    {
        if (deck.mainDeckCards == null || deck.mainDeckCards.Length < 60)
            return false;

        if (deck.sideboardCards != null && deck.sideboardCards.Length > 15)
            return false;

        // カードの4枚制限チェック
        var cardCounts = new System.Collections.Generic.Dictionary<string, int>();
        foreach (string cardId in deck.mainDeckCards)
        {
            if (!cardCounts.ContainsKey(cardId))
                cardCounts[cardId] = 0;
            cardCounts[cardId]++;

            if (cardCounts[cardId] > 4)
                return false;
        }

        return true;
    }

    public void SaveDeck(string deckName, string format)
    {
        if (!Networking.IsOwner(gameObject))
            return;

        // デッキデータの更新
        currentDeck.name = deckName;
        currentDeck.format = format;
        currentDeck.mainDeckCards = deckManager.GetMainDeckCards();
        currentDeck.sideboardCards = deckManager.GetSideboardCards();

        // デッキの検証
        if (!ValidateDeck(currentDeck))
        {
            Debug.LogError("Invalid deck configuration");
            return;
        }

        // デッキの保存
        SaveDeckToPlayerPrefs(currentDeck);
    }

    private void SaveDeckToPlayerPrefs(DeckData deck)
    {
        // デッキデータを文字列形式に変換
        string deckString = $"name:{deck.name}\n" +
                          $"format:{deck.format}\n" +
                          $"mainDeck:{string.Join(",", deck.mainDeckCards)}\n" +
                          $"sideboard:{string.Join(",", deck.sideboardCards)}";

        PlayerPrefs.SetString($"{playerPrefsPrefix}{Networking.LocalPlayer.playerId}", deckString);
        PlayerPrefs.Save();
    }

    public void LoadCurrentDeck()
    {
        if (!Networking.IsOwner(gameObject))
            return;

        // デッキマネージャーにデータを渡す
        deckManager.ClearDeck();
        
        if (currentDeck.mainDeckCards != null)
        {
            foreach (string cardId in currentDeck.mainDeckCards)
            {
                if (!string.IsNullOrEmpty(cardId))
                    deckManager.AddCardToDeck(cardId);
            }
        }
        
        if (currentDeck.sideboardCards != null)
        {
            foreach (string cardId in currentDeck.sideboardCards)
            {
                if (!string.IsNullOrEmpty(cardId))
                    deckManager.AddCardToSideboard(cardId);
            }
        }
    }

    public void DeleteDeck()
    {
        if (!Networking.IsOwner(gameObject))
            return;

        PlayerPrefs.DeleteKey($"{playerPrefsPrefix}{Networking.LocalPlayer.playerId}");
        PlayerPrefs.Save();
        
        currentDeck = new DeckData
        {
            name = "新規デッキ",
            format = "standard",
            mainDeckCards = new string[0],
            sideboardCards = new string[0]
        };
        
        deckManager.ClearDeck();
    }

    public string ExportDeck()
    {
        // デッキリストのテキスト形式での出力
        string export = $"// {currentDeck.name}\n";
        export += $"// Format: {currentDeck.format}\n\n";

        // メインデッキ
        export += "// Main Deck\n";
        foreach (string cardId in currentDeck.mainDeckCards)
        {
            if (!string.IsNullOrEmpty(cardId))
            {
                CardDataManager.CardData card = cardDataManager.GetCardData(cardId);
                export += $"1 {card.name}\n";
            }
        }

        // サイドボード
        if (currentDeck.sideboardCards != null && currentDeck.sideboardCards.Length > 0)
        {
            export += "\n// Sideboard\n";
            foreach (string cardId in currentDeck.sideboardCards)
            {
                if (!string.IsNullOrEmpty(cardId))
                {
                    CardDataManager.CardData card = cardDataManager.GetCardData(cardId);
                    export += $"1 {card.name}\n";
                }
            }
        }

        return export;
    }

    public bool ImportDeck(string deckList, string deckName, string format)
    {
        if (!Networking.IsOwner(gameObject))
            return false;

        // デッキリストの解析
        string[] lines = deckList.Split(new[] { '\n' });
        var mainDeckCards = new System.Collections.Generic.List<string>();
        var sideboardCards = new System.Collections.Generic.List<string>();
        bool isSideboard = false;

        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line.Trim()) || line.StartsWith("//"))
                continue;

            if (line.ToLower().Contains("sideboard"))
            {
                isSideboard = true;
                continue;
            }

            // カード行の解析（例: "2 Lightning Bolt"）
            string[] parts = line.Trim().Split(new[] { ' ' }, 2);
            if (parts.Length != 2)
                continue;

            int count;
            if (!int.TryParse(parts[0], out count))
                continue;

            string cardName = parts[1];
            string cardId = FindCardIdByName(cardName);
            if (string.IsNullOrEmpty(cardId))
                continue;

            // カードの追加
            var targetList = isSideboard ? sideboardCards : mainDeckCards;
            for (int i = 0; i < count; i++)
            {
                targetList.Add(cardId);
            }
        }

        // デッキの更新
        currentDeck = new DeckData
        {
            name = deckName,
            format = format,
            mainDeckCards = mainDeckCards.ToArray(),
            sideboardCards = sideboardCards.ToArray()
        };

        if (!ValidateDeck(currentDeck))
            return false;

        // デッキの保存
        SaveDeckToPlayerPrefs(currentDeck);
        LoadCurrentDeck();
        return true;
    }

    private string FindCardIdByName(string cardName)
    {
        // TODO: CardDataManagerにカード名からIDを検索する機能を実装
        return cardDataManager != null ? cardDataManager.FindCardIdByName(cardName) : "";
    }
}
