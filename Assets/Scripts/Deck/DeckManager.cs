using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DeckManager : UdonSharpBehaviour
{
    [Header("Deck Settings")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform deckPosition;
    [SerializeField] private Transform cardSpawnPoint;
    [SerializeField] private int maxCardsInDeck = 60;
    
    [Header("Card Spawn Settings")]
    [SerializeField] private float cardSpawnHeight = 0.01f; // カード積み重ねの高さ間隔
    [SerializeField] private float dealSpeed = 0.2f; // カードを配る速度

    private GameObject[] cardObjects;
    private GameObject[] sideboardObjects;
    [UdonSynced] private string[] cardIds;
    [UdonSynced] private string[] sideboardCardIds;
    [UdonSynced] private int currentTopCard = 0;
    private bool isDealing = false;
    private bool isSideboardVisible = false;

    void Start()
    {
        cardObjects = new GameObject[maxCardsInDeck];
        sideboardObjects = new GameObject[15];
        cardIds = new string[maxCardsInDeck];
        sideboardCardIds = new string[15];
    }

    public string[] GetMainDeckCards()
    {
        return cardIds;
    }

    public string[] GetSideboardCards()
    {
        return sideboardCardIds;
    }

    public void AddCardToDeck(string cardId)
    {
        if (!Networking.IsOwner(gameObject))
            return;

        // メインデッキに追加
        for (int i = 0; i < cardIds.Length; i++)
        {
            if (string.IsNullOrEmpty(cardIds[i]))
            {
                cardIds[i] = cardId;
                break;
            }
        }
        CreateCardObjects();
        RequestSerialization();
    }

    public void AddCardToSideboard(string cardId)
    {
        if (!Networking.IsOwner(gameObject))
            return;

        // サイドボードに追加
        for (int i = 0; i < sideboardCardIds.Length; i++)
        {
            if (string.IsNullOrEmpty(sideboardCardIds[i]))
            {
                sideboardCardIds[i] = cardId;
                break;
            }
        }
        CreateCardObjects();
        RequestSerialization();
    }

    public void ClearDeck()
    {
        if (!Networking.IsOwner(gameObject))
            return;

        // メインデッキとサイドボードをクリア
        for (int i = 0; i < cardIds.Length; i++)
        {
            cardIds[i] = "";
        }
        for (int i = 0; i < sideboardCardIds.Length; i++)
        {
            sideboardCardIds[i] = "";
        }

        // オブジェクトを破棄
        foreach (var obj in cardObjects)
        {
            if (obj != null) Destroy(obj);
        }
        foreach (var obj in sideboardObjects)
        {
            if (obj != null) Destroy(obj);
        }

        currentTopCard = 0;
        RequestSerialization();
    }

    public int GetMainDeckCount()
    {
        int count = 0;
        foreach (string cardId in cardIds)
        {
            if (!string.IsNullOrEmpty(cardId))
                count++;
        }
        return count;
    }

    public int GetSideboardCount()
    {
        int count = 0;
        foreach (string cardId in sideboardCardIds)
        {
            if (!string.IsNullOrEmpty(cardId))
                count++;
        }
        return count;
    }

    public void RemoveCardFromDeck(string cardId)
    {
        if (!Networking.IsOwner(gameObject))
            return;

        // メインデッキから検索
        for (int i = 0; i < cardIds.Length; i++)
        {
            if (cardIds[i] == cardId)
            {
                cardIds[i] = "";
                CreateCardObjects();
                RequestSerialization();
                return;
            }
        }

        // サイドボードから検索
        for (int i = 0; i < sideboardCardIds.Length; i++)
        {
            if (sideboardCardIds[i] == cardId)
            {
                sideboardCardIds[i] = "";
                CreateCardObjects();
                RequestSerialization();
                return;
            }
        }
    }

    public void InitializeDeck(string[] newCardIds)
    {
        if (!Networking.IsOwner(gameObject))
            return;

        // デッキの初期化
        currentTopCard = 0;
        cardIds = new string[Mathf.Min(newCardIds.Length, maxCardsInDeck)];
        System.Array.Copy(newCardIds, cardIds, cardIds.Length);

        // カードの生成
        CreateCardObjects();
        
        // シャッフル
        ShuffleDeck();

        RequestSerialization();
    }

    private void CreateCardObjects()
    {
        // 既存のカードを削除
        for (int i = 0; i < cardObjects.Length; i++)
        {
            if (cardObjects[i] != null)
            {
                Destroy(cardObjects[i]);
            }
        }

        // 新しいカードを生成
        for (int i = 0; i < cardIds.Length; i++)
        {
            Vector3 spawnPosition = deckPosition.position + Vector3.up * (cardSpawnHeight * i);
            cardObjects[i] = VRCInstantiate(cardPrefab);
            cardObjects[i].transform.position = spawnPosition;
            cardObjects[i].transform.rotation = deckPosition.rotation;

            CardBehaviour card = cardObjects[i].GetComponent<CardBehaviour>();
            if (card != null)
            {
                card.SetCardData(cardIds[i], null); // テクスチャは後で実装
            }
        }
    }

    public void ShuffleDeck()
    {
        if (!Networking.IsOwner(gameObject))
            return;

        // Fisher-Yatesシャッフル
        for (int i = cardIds.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            // カードIDの交換
            string tempId = cardIds[i];
            cardIds[i] = cardIds[j];
            cardIds[j] = tempId;

            // 物理的なカードオブジェクトの交換
            if (cardObjects[i] != null && cardObjects[j] != null)
            {
                Vector3 tempPos = cardObjects[i].transform.position;
                cardObjects[i].transform.position = cardObjects[j].transform.position;
                cardObjects[j].transform.position = tempPos;

                GameObject tempObj = cardObjects[i];
                cardObjects[i] = cardObjects[j];
                cardObjects[j] = tempObj;
            }
        }

        RequestSerialization();
    }

    public void DrawCard()
    {
        if (isDealing || currentTopCard >= cardIds.Length)
            return;

        if (cardObjects[currentTopCard] != null)
        {
            // カードを上部に移動
            Vector3 drawPosition = cardSpawnPoint.position;
            cardObjects[currentTopCard].transform.position = drawPosition;
            
            // カード操作を有効化
            EnableCardInteraction(currentTopCard);
            
            currentTopCard++;
            RequestSerialization();
        }
    }

    private void EnableCardInteraction(int cardIndex)
    {
        if (cardObjects[cardIndex] != null)
        {
            CardBehaviour card = cardObjects[cardIndex].GetComponent<CardBehaviour>();
            if (card != null)
            {
                // カードの操作を有効化する処理（後で実装）
            }
        }
    }

    public void ResetDeck()
    {
        if (!Networking.IsOwner(gameObject))
            return;

        currentTopCard = 0;
        CreateCardObjects();
        ShuffleDeck();
        RequestSerialization();
    }

    public override void OnDeserialization()
    {
        // ネットワーク同期後の処理（後で実装）
    }
}
