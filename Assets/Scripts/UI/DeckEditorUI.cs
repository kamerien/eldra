using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

public class DeckEditorUI : UdonSharpBehaviour
{
    [Header("Layout References")]
    [SerializeField] private RectTransform searchPanel;    // 左側パネル
    [SerializeField] private RectTransform deckPanel;      // 右側パネル

    [Header("Search Panel")]
    [SerializeField] private Transform formatTabContainer;  // フォーマットタブコンテナ
    [SerializeField] private Transform colorFilterContainer; // 色フィルターコンテナ
    [SerializeField] private Transform typeFilterContainer;  // タイプフィルターコンテナ
    [SerializeField] private Transform searchResultsContent; // 検索結果表示エリア
    [SerializeField] private TMP_InputField searchInput;    // 検索入力フィールド

    [Header("Deck Panel")]
    [SerializeField] private Transform mainDeckContent;     // メインデッキ表示エリア
    [SerializeField] private Transform sideboardContent;    // サイドボード表示エリア
    [SerializeField] private Transform companionContent;    // 相棒表示エリア
    [SerializeField] private Transform commanderContent;    // 統率者表示エリア
    
    [Header("Deck Info")]
    [SerializeField] private TextMeshProUGUI mainDeckCount;
    [SerializeField] private TextMeshProUGUI sideboardCount;
    [SerializeField] private TextMeshProUGUI deckNameText;
    
    [Header("Card Templates")]
    [SerializeField] private GameObject cardListItemPrefab;
    [SerializeField] private GameObject deckListItemPrefab;

    [Header("Dependencies")]
    [SerializeField] private CardDataManager cardDataManager;
    [SerializeField] private DeckManager deckManager;

    private string currentFormat = "standard";
    private string[] selectedColors = new string[0];
    private string selectedType = "";
    private int selectedCost = -1;
    private bool showMultiColorAtEnd = true;

    void Start()
    {
        InitializeUI();
        SetupSearchPanel();
        RefreshDeckView();
    }

    private void InitializeUI()
    {
        // フォーマットタブの設定
        string[] formats = new[] { "Standard", "Pioneer", "Modern", "Legacy", "Vintage", "Commander" };
        foreach (string format in formats)
        {
            GameObject tab = CreateFormatTab(format);
            tab.transform.SetParent(formatTabContainer, false);
        }

        // 色フィルターの設定
        string[] colors = new[] { "White", "Blue", "Black", "Red", "Green", "Colorless", "Multi" };
        foreach (string color in colors)
        {
            GameObject filter = CreateColorFilter(color);
            filter.transform.SetParent(colorFilterContainer, false);
        }

        // タイプフィルターの設定
        string[] types = new[] { "Creature", "Instant", "Sorcery", "Enchantment", "Artifact", "Land", "Planeswalker" };
        foreach (string type in types)
        {
            GameObject filter = CreateTypeFilter(type);
            filter.transform.SetParent(typeFilterContainer, false);
        }
    }

    private GameObject CreateFormatTab(string format)
    {
        GameObject tab = new GameObject(format + "Tab");
        tab.AddComponent<RectTransform>();
        Button button = tab.AddComponent<Button>();
        
        // ボタンのテキスト設定
        TextMeshProUGUI text = new GameObject("Text").AddComponent<TextMeshProUGUI>();
        text.transform.SetParent(tab.transform, false);
        text.text = format;
        text.alignment = TextAlignmentOptions.Center;
        
        // ボタンクリックイベント
        button.onClick.AddListener(delegate { OnFormatSelected(format.ToLower()); });
        
        return tab;
    }

    private GameObject CreateColorFilter(string color)
    {
        GameObject filter = new GameObject(color + "Filter");
        filter.AddComponent<RectTransform>();
        Toggle toggle = filter.AddComponent<Toggle>();
        
        // トグルのテキスト設定
        TextMeshProUGUI text = new GameObject("Text").AddComponent<TextMeshProUGUI>();
        text.transform.SetParent(filter.transform, false);
        text.text = color;
        
        // トグル変更イベント
        toggle.onValueChanged.AddListener(delegate { OnColorFilterChanged(color, toggle.isOn); });
        
        return filter;
    }

    private GameObject CreateTypeFilter(string type)
    {
        GameObject filter = new GameObject(type + "Filter");
        filter.AddComponent<RectTransform>();
        Button button = filter.AddComponent<Button>();
        
        // ボタンのテキスト設定
        TextMeshProUGUI text = new GameObject("Text").AddComponent<TextMeshProUGUI>();
        text.transform.SetParent(filter.transform, false);
        text.text = type;
        
        // ボタンクリックイベント
        button.onClick.AddListener(delegate { OnTypeSelected(type); });
        
        return filter;
    }

    private void SetupSearchPanel()
    {
        // 検索パネルのレイアウト設定
        searchPanel.anchorMin = new Vector2(0, 0);
        searchPanel.anchorMax = new Vector2(0.5f, 1);
        searchPanel.pivot = new Vector2(0, 0.5f);
        
        // 検索結果エリアをコスト順に分割
        for (int i = 0; i <= 7; i++) // 0-6のコスト + 7以上用
        {
            GameObject costSection = new GameObject($"Cost{i}Section");
            costSection.AddComponent<RectTransform>().SetParent(searchResultsContent, false);
            
            TextMeshProUGUI header = new GameObject("Header").AddComponent<TextMeshProUGUI>();
            header.transform.SetParent(costSection.transform, false);
            header.text = i == 7 ? "7+" : i.ToString();
        }
    }

    private void RefreshSearchResults()
    {
        // 既存の検索結果をクリア
        foreach (Transform child in searchResultsContent)
        {
            foreach (Transform cardItem in child)
            {
                if (cardItem.name != "Header")
                    Destroy(cardItem.gameObject);
            }
        }

        // カードデータマネージャーから合法カードリストを取得
        string[] legalCards = cardDataManager.GetLegalCardsForFormat(currentFormat);
        
        foreach (string cardId in legalCards)
        {
            CardDataManager.CardData cardData = cardDataManager.GetCardData(cardId);
            
            // フィルター条件のチェック
            if (!MatchesFilters(cardData))
                continue;

            // コストセクションの決定
            int cost = GetConvertedManaCost(cardData.manaCost);
            int sectionIndex = Mathf.Min(cost, 7);
            
            // マルチカラーカードの場合、各コストの最後に配置
            Transform targetSection = searchResultsContent.GetChild(sectionIndex);
            if (IsMultiColor(cardData) && showMultiColorAtEnd)
            {
                // 新しいカードアイテムを最後に追加
                CreateCardListItem(cardData, targetSection, true);
            }
            else
            {
                // 新しいカードアイテムを追加（マルチカラー以外）
                CreateCardListItem(cardData, targetSection, false);
            }
        }
    }

    private void CreateCardListItem(CardDataManager.CardData cardData, Transform parent, bool isMultiColor)
    {
        GameObject cardItem = VRCInstantiate(cardListItemPrefab);
        cardItem.transform.SetParent(parent, false);
        
        if (isMultiColor)
        {
            // マルチカラーカードを最後に移動
            cardItem.transform.SetAsLastSibling();
        }

        // カード情報をUIに設定
        SetupCardListItem(cardItem, cardData);
    }

    private bool IsMultiColor(CardDataManager.CardData cardData)
    {
        int colorCount = 0;
        if (cardData.manaCost.Contains("W")) colorCount++;
        if (cardData.manaCost.Contains("U")) colorCount++;
        if (cardData.manaCost.Contains("B")) colorCount++;
        if (cardData.manaCost.Contains("R")) colorCount++;
        if (cardData.manaCost.Contains("G")) colorCount++;
        return colorCount > 1;
    }

    private void RefreshDeckView()
    {
        ClearDeckView();

        // メインデッキの表示
        string[] mainDeckCards = deckManager.GetMainDeckCards();
        foreach (string cardId in mainDeckCards)
        {
            if (!string.IsNullOrEmpty(cardId))
            {
                CreateDeckListItem(cardId, mainDeckContent);
            }
        }

        // サイドボードの表示
        string[] sideboardCards = deckManager.GetSideboardCards();
        foreach (string cardId in sideboardCards)
        {
            if (!string.IsNullOrEmpty(cardId))
            {
                CreateDeckListItem(cardId, sideboardContent);
            }
        }

        UpdateDeckCounts();
    }

    private void ClearDeckView()
    {
        foreach (Transform child in mainDeckContent)
            Destroy(child.gameObject);
        foreach (Transform child in sideboardContent)
            Destroy(child.gameObject);
        foreach (Transform child in companionContent)
            Destroy(child.gameObject);
        foreach (Transform child in commanderContent)
            Destroy(child.gameObject);
    }

    private void CreateDeckListItem(string cardId, Transform parent)
    {
        CardDataManager.CardData cardData = cardDataManager.GetCardData(cardId);
        GameObject cardItem = VRCInstantiate(deckListItemPrefab);
        cardItem.transform.SetParent(parent, false);
        SetupDeckListItem(cardItem, cardData);
    }

    private void SetupCardListItem(GameObject item, CardDataManager.CardData cardData)
    {
        var nameText = item.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
        var costText = item.transform.Find("CostText")?.GetComponent<TextMeshProUGUI>();
        var typeText = item.transform.Find("TypeText")?.GetComponent<TextMeshProUGUI>();
        
        if (nameText != null) nameText.text = cardData.name;
        if (costText != null) costText.text = cardData.manaCost;
        if (typeText != null) typeText.text = cardData.type;

        // VRモード用の設定
        var grabInteractable = item.GetComponent<VRC.SDK3.Components.VRCPickup>();
        if (grabInteractable != null)
        {
            // つかむ操作の設定
            grabInteractable.proximity = 0.1f;  // つかむ距離
            grabInteractable.allowManipulationWhenEquipped = true;  // 持った後の操作を許可
        }

        // デスクトップモード用の設定
        var button = item.GetComponent<Button>();
        if (button != null)
        {
            // ドラッグ&ドロップの設定
            var dragHandler = item.AddComponent<CardDragHandler>();
            dragHandler.Setup(cardData.id, this);
            
            // クリック操作の設定
            button.onClick.AddListener(delegate { HandleCardClick(cardData.id); });
        }
    }

    private void HandleCardClick(string cardId)
    {
        CardDataManager.CardData cardData = cardDataManager.GetCardData(cardId);

        // キー修飾子に基づいてカードの追加先を決定
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (cardData.type.Contains("Creature") && IsValidCompanion(cardData))
            {
                // Ctrl+クリックで相棒に設定
                SetAsCompanion(cardId);
            }
        }
        else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (IsValidCommander(cardData))
            {
                // Shift+クリックで統率者に設定
                SetAsCommander(cardId);
            }
        }
        else if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            // Alt+クリックでサイドボードに追加
            AddCardToSideboard(cardId);
        }
        else
        {
            // 通常クリックでメインデッキに追加
            AddCardToDeck(cardId);
        }
    }

    private bool IsValidCompanion(CardDataManager.CardData cardData)
    {
        // 相棒の条件チェック（例: クリーチャーである、特定のキーワードを持つなど）
        return cardData.type.Contains("Creature") && cardData.text.Contains("Companion");
    }

    private bool IsValidCommander(CardDataManager.CardData cardData)
    {
        // 統率者の条件チェック
        return cardData.type.Contains("Legendary") && 
               (cardData.type.Contains("Creature") || cardData.text.Contains("can be your commander"));
    }

    private void SetAsCompanion(string cardId)
    {
        if (!Networking.IsOwner(gameObject))
            return;

        // 既存の相棒をクリア
        foreach (Transform child in companionContent)
            Destroy(child.gameObject);

        // 新しい相棒を設定
        CreateDeckListItem(cardId, companionContent);
    }

    private void SetAsCommander(string cardId)
    {
        if (!Networking.IsOwner(gameObject))
            return;

        // 既存の統率者をクリア
        foreach (Transform child in commanderContent)
            Destroy(child.gameObject);

        // 新しい統率者を設定
        CreateDeckListItem(cardId, commanderContent);
    }

    // VRでのカード追加処理
    public void OnCardDropped(string cardId, Vector3 dropPosition)
    {
        // ドロップ位置に基づいてデッキエリアを判定
        if (IsOverDeckArea(dropPosition))
        {
            AddCardToDeck(cardId);
        }
        else if (IsOverSideboardArea(dropPosition))
        {
            AddCardToSideboard(cardId);
        }
        else if (IsOverCommanderArea(dropPosition))
        {
            SetAsCommander(cardId);
        }
        else if (IsOverCompanionArea(dropPosition))
        {
            SetAsCompanion(cardId);
        }
    }

    private bool IsOverDeckArea(Vector3 position)
    {
        // メインデッキエリアの範囲チェック
        RectTransform deckRect = mainDeckContent.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(deckRect, position);
    }

    private bool IsOverSideboardArea(Vector3 position)
    {
        RectTransform sideboardRect = sideboardContent.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(sideboardRect, position);
    }

    private bool IsOverCommanderArea(Vector3 position)
    {
        RectTransform commanderRect = commanderContent.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(commanderRect, position);
    }

    private bool IsOverCompanionArea(Vector3 position)
    {
        RectTransform companionRect = companionContent.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(companionRect, position);
    }

    private void SetupDeckListItem(GameObject item, CardDataManager.CardData cardData)
    {
        var nameText = item.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
        var costText = item.transform.Find("CostText")?.GetComponent<TextMeshProUGUI>();
        
        if (nameText != null) nameText.text = cardData.name;
        if (costText != null) costText.text = cardData.manaCost;

        var button = item.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(delegate { RemoveCardFromDeck(cardData.id); });
        }
    }

    private void UpdateDeckCounts()
    {
        int mainCount = deckManager.GetMainDeckCount();
        int sideCount = deckManager.GetSideboardCount();

        mainDeckCount.text = $"Main Deck: {mainCount}/60";
        sideboardCount.text = $"Sideboard: {sideCount}/15";

        mainDeckCount.color = mainCount < 60 ? Color.red : Color.white;
        sideboardCount.color = sideCount > 15 ? Color.red : Color.white;
    }

    public void OnFormatSelected(string format)
    {
        currentFormat = format;
        RefreshSearchResults();
    }

    public void OnColorFilterChanged(string color, bool isSelected)
    {
        // 色フィルターの更新
        var colorsList = new System.Collections.Generic.List<string>(selectedColors);
        if (isSelected && !colorsList.Contains(color))
        {
            colorsList.Add(color);
        }
        else if (!isSelected && colorsList.Contains(color))
        {
            colorsList.Remove(color);
        }
        selectedColors = colorsList.ToArray();
        
        RefreshSearchResults();
    }

    public void OnTypeSelected(string type)
    {
        selectedType = type;
        RefreshSearchResults();
    }

    private bool MatchesFilters(CardDataManager.CardData cardData)
    {
        // 色フィルター
        if (selectedColors.Length > 0)
        {
            bool matchesColor = false;
            foreach (string color in selectedColors)
            {
                if (color == "Multi" && IsMultiColor(cardData))
                {
                    matchesColor = true;
                    break;
                }
                else if (cardData.manaCost.Contains(GetColorSymbol(color)))
                {
                    matchesColor = true;
                    break;
                }
            }
            if (!matchesColor) return false;
        }

        // タイプフィルター
        if (!string.IsNullOrEmpty(selectedType) && !cardData.type.Contains(selectedType))
            return false;

        // 検索テキスト
        if (!string.IsNullOrEmpty(searchInput.text) && 
            !cardData.name.ToLower().Contains(searchInput.text.ToLower()))
            return false;

        return true;
    }

    private string GetColorSymbol(string color)
    {
        switch (color.ToLower())
        {
            case "white": return "W";
            case "blue": return "U";
            case "black": return "B";
            case "red": return "R";
            case "green": return "G";
            case "colorless": return "C";
            default: return "";
        }
    }

    private int GetConvertedManaCost(string manaCost)
    {
        int cmc = 0;
        bool inBracket = false;
        string currentNumber = "";

        foreach (char c in manaCost)
        {
            if (c == '{')
            {
                inBracket = true;
                continue;
            }
            else if (c == '}')
            {
                if (!string.IsNullOrEmpty(currentNumber))
                {
                    cmc += int.Parse(currentNumber);
                    currentNumber = "";
                }
                inBracket = false;
                continue;
            }

            if (inBracket)
            {
                if (char.IsDigit(c))
                {
                    currentNumber += c;
                }
                else if (c != 'X' && c != 'Y' && c != 'Z')
                {
                    cmc++;
                }
            }
        }

        return cmc;
    }

    public void AddCardToDeck(string cardId)
    {
        if (!Networking.IsOwner(gameObject))
            return;

        deckManager.AddCardToDeck(cardId);
        RefreshDeckView();
    }

    public void RemoveCardFromDeck(string cardId)
    {
        if (!Networking.IsOwner(gameObject))
            return;

        deckManager.RemoveCardFromDeck(cardId);
        RefreshDeckView();
    }

    public void OnSearchInputChanged()
    {
        RefreshSearchResults();
    }

    public override void OnDeserialization()
    {
        RefreshDeckView();
    }
}
