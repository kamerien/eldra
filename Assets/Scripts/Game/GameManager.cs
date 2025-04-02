using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GameManager : UdonSharpBehaviour
{
    [Header("Game Components")]
    [SerializeField] private DeckManager[] deckManagers; // プレイヤー数分のデッキマネージャー
    [SerializeField] private GameUI[] playerUIs; // プレイヤー数分のUI
    [SerializeField] private Transform[] playerPositions; // プレイヤーの座席位置
    
    [Header("Game Settings")]
    [SerializeField] private int maxPlayers = 4;
    [SerializeField] private int startingLifePoints = 20;
    [SerializeField] private float turnTimeLimit = 180f; // 1ターンの制限時間（秒）

    [UdonSynced] private int currentPlayerCount = 0;
    [UdonSynced] private int activePlayerIndex = 0;
    private VRCPlayerApi[] players;
    private bool gameInProgress = false;

    void Start()
    {
        players = new VRCPlayerApi[maxPlayers];
        InitializeGame();
    }

    private void InitializeGame()
    {
        // UIの初期化
        for (int i = 0; i < playerUIs.Length; i++)
        {
            if (playerUIs[i] != null)
            {
                playerUIs[i].ShowGameState(false);
                playerUIs[i].SetButtonsInteractable(false);
            }
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (currentPlayerCount >= maxPlayers) return;

        // プレイヤーを配列に追加
        players[currentPlayerCount] = player;
        
        // プレイヤーを座席に配置
        if (player.isLocal)
        {
            AssignPlayerPosition(currentPlayerCount);
        }

        currentPlayerCount++;
        
        if (Networking.IsOwner(gameObject))
        {
            RequestSerialization();
        }
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        // プレイヤーの削除処理
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null && players[i].playerId == player.playerId)
            {
                RemovePlayer(i);
                break;
            }
        }
    }

    private void AssignPlayerPosition(int playerIndex)
    {
        if (playerPositions != null && playerIndex < playerPositions.Length)
        {
            Networking.LocalPlayer.TeleportTo(
                playerPositions[playerIndex].position,
                playerPositions[playerIndex].rotation
            );
        }
    }

    private void RemovePlayer(int index)
    {
        // プレイヤーの情報をクリア
        players[index] = null;
        
        // プレイヤー配列を詰める
        for (int i = index; i < currentPlayerCount - 1; i++)
        {
            players[i] = players[i + 1];
            if (players[i] != null && players[i].isLocal)
            {
                AssignPlayerPosition(i);
            }
        }

        currentPlayerCount--;
        
        if (Networking.IsOwner(gameObject))
        {
            RequestSerialization();
        }

        // ゲーム進行中の場合、必要に応じてゲームを終了
        if (gameInProgress && currentPlayerCount < 2)
        {
            EndGame();
        }
    }

    public void StartGame()
    {
        if (!Networking.IsOwner(gameObject) || currentPlayerCount < 2) return;

        gameInProgress = true;
        activePlayerIndex = 0;

        // 各プレイヤーのUIとデッキを初期化
        for (int i = 0; i < currentPlayerCount; i++)
        {
            if (playerUIs[i] != null)
            {
                playerUIs[i].ShowGameState(true);
                playerUIs[i].UpdateLifePoints(startingLifePoints);
                playerUIs[i].SetButtonsInteractable(i == activePlayerIndex);
            }

            if (deckManagers[i] != null)
            {
                deckManagers[i].ResetDeck();
            }
        }

        RequestSerialization();
    }

    public void EndGame()
    {
        if (!Networking.IsOwner(gameObject)) return;

        gameInProgress = false;
        
        // ゲーム終了時の処理
        for (int i = 0; i < playerUIs.Length; i++)
        {
            if (playerUIs[i] != null)
            {
                playerUIs[i].ShowGameState(false);
                playerUIs[i].SetButtonsInteractable(false);
            }
        }

        RequestSerialization();
    }

    public void NextTurn()
    {
        if (!Networking.IsOwner(gameObject) || !gameInProgress) return;

        // 現在のプレイヤーのUIを無効化
        if (playerUIs[activePlayerIndex] != null)
        {
            playerUIs[activePlayerIndex].SetButtonsInteractable(false);
        }

        // 次のプレイヤーへ
        activePlayerIndex = (activePlayerIndex + 1) % currentPlayerCount;

        // 新しいプレイヤーのUIを有効化
        if (playerUIs[activePlayerIndex] != null)
        {
            playerUIs[activePlayerIndex].SetButtonsInteractable(true);
        }

        RequestSerialization();
    }

    public override void OnDeserialization()
    {
        // ゲーム状態の同期後の更新処理
        UpdateGameState();
    }

    private void UpdateGameState()
    {
        // UI状態の更新
        for (int i = 0; i < playerUIs.Length; i++)
        {
            if (playerUIs[i] != null)
            {
                playerUIs[i].SetButtonsInteractable(i == activePlayerIndex && gameInProgress);
            }
        }
    }
}
