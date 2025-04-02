using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class GameUI : UdonSharpBehaviour
{
    [Header("Player Info")]
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text lifePointsText;
    
    [Header("Game State")]
    [SerializeField] private GameObject gameStatePanel;
    [SerializeField] private Text currentPhaseText;
    [SerializeField] private Text cardsInDeckText;
    [SerializeField] private Text cardsInHandText;

    [Header("Action Buttons")]
    [SerializeField] private Button drawCardButton;
    [SerializeField] private Button shuffleButton;
    [SerializeField] private Button resetButton;

    private int currentLifePoints = 20;
    private int cardsInHand = 0;
    private int cardsInDeck = 0;

    void Start()
    {
        // 初期値を設定
        UpdateLifePoints(currentLifePoints);
        UpdateCardCounts(0, 60); // 仮の値
        
        if (playerNameText != null)
        {
            playerNameText.text = Networking.LocalPlayer.displayName;
        }
    }

    public void OnDrawCardButtonClick()
    {
        // DeckManagerのDrawCard関数を呼び出す処理は後で実装
        cardsInHand++;
        cardsInDeck--;
        UpdateCardCounts(cardsInHand, cardsInDeck);
    }

    public void OnShuffleButtonClick()
    {
        // DeckManagerのShuffleDeck関数を呼び出す処理は後で実装
    }

    public void OnResetButtonClick()
    {
        // ゲームをリセットする処理は後で実装
        ResetGameState();
    }

    public void UpdateLifePoints(int points)
    {
        currentLifePoints = points;
        if (lifePointsText != null)
        {
            lifePointsText.text = $"ライフ: {currentLifePoints}";
        }
    }

    public void UpdateCardCounts(int handCount, int deckCount)
    {
        cardsInHand = handCount;
        cardsInDeck = deckCount;

        if (cardsInHandText != null)
        {
            cardsInHandText.text = $"手札: {cardsInHand}枚";
        }

        if (cardsInDeckText != null)
        {
            cardsInDeckText.text = $"デッキ: {cardsInDeck}枚";
        }
    }

    public void UpdatePhaseText(string phase)
    {
        if (currentPhaseText != null)
        {
            currentPhaseText.text = $"フェーズ: {phase}";
        }
    }

    private void ResetGameState()
    {
        currentLifePoints = 20;
        cardsInHand = 0;
        cardsInDeck = 60; // デフォルトデッキサイズ

        UpdateLifePoints(currentLifePoints);
        UpdateCardCounts(cardsInHand, cardsInDeck);
        UpdatePhaseText("開始前");
    }

    public void SetButtonsInteractable(bool interactable)
    {
        if (drawCardButton != null)
            drawCardButton.interactable = interactable;
        
        if (shuffleButton != null)
            shuffleButton.interactable = interactable;
        
        if (resetButton != null)
            resetButton.interactable = interactable;
    }

    public void ShowGameState(bool show)
    {
        if (gameStatePanel != null)
        {
            gameStatePanel.SetActive(show);
        }
    }
}
