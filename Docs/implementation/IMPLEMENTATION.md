# VRChat MTG ワールド実装仕様

## 必要なツール・アセット

### 基本開発環境

1. Unity 2022.3.6f1

    - VRChat の推奨バージョン
    - ビルド設定：Android/PC VR 対応

2. VRChat SDK3
    - World SDK
    - UdonSharp
    - VRChat Creator Companion (VCC)

### 推奨アセット

1. 物理演算・インタラクション用

    - [PhysBone](https://docs.vrchat.com/docs/physbones)
        - カードの物理挙動
        - 手札の保持システム
    - [Constraints](https://docs.vrchat.com/docs/constraints)
        - カードの配置制限
        - プレイエリアの境界

2. UI 関連
    - [VRChat UI Shape Kit](https://github.com/vrchat-community/ui-shape-kit)
        - カード情報表示
        - メニューシステム
    - [Unity UI Extensions](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions)
        - 高度な UI コンポーネント
        - カードリスト表示

## プロジェクト構造

```
Assets/
  ├── VRChat/                # VRChat SDKファイル
  ├── Udon/                  # Udonスクリプト
  ├── Scripts/               # UdonSharpスクリプト
  │   ├── Card/             # カード関連
  │   ├── Deck/             # デッキ関連
  │   ├── Game/             # ゲームロジック
  │   └── UI/               # UI関連
  ├── Prefabs/              # プレハブ
  │   ├── Cards/           # カードプレハブ
  │   ├── UI/              # UI要素
  │   └── GameSystem/      # ゲームシステム
  ├── Models/               # 3Dモデル
  ├── Materials/            # マテリアル
  └── Textures/            # テクスチャ
```

## 主要コンポーネント実装

### 1. カードシステム

```csharp
using UdonSharp;
using VRC.SDKBase;
using UnityEngine;

public class CardBehaviour : UdonSharpBehaviour
{
    [SerializeField] private string cardId;
    [SerializeField] private MeshRenderer cardRenderer;
    [SerializeField] private Collider cardCollider;

    private VRCPlayerApi currentHolder;
    private bool isRevealed;

    public void Initialize(string id, Texture2D cardTexture)
    {
        cardId = id;
        cardRenderer.material.mainTexture = cardTexture;
    }

    public override void OnPickup()
    {
        currentHolder = Networking.LocalPlayer;
        UpdateVisibility();
    }

    public override void OnDrop()
    {
        currentHolder = null;
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        // カードの表示/非表示制御
    }
}
```

### 2. デッキ管理システム

```csharp
using UdonSharp;
using VRC.SDKBase;
using UnityEngine;

public class DeckManager : UdonSharpBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform deckPosition;

    [UdonSynced] private string[] cardIds;
    [UdonSynced] private int topCardIndex;

    public void LoadDeck(string[] newCardIds)
    {
        cardIds = newCardIds;
        Shuffle();
    }

    public void DrawCard()
    {
        if (topCardIndex >= cardIds.Length) return;

        // カードを生成して配る処理
        GameObject newCard = VRCInstantiate(cardPrefab);
        newCard.GetComponent<CardBehaviour>().Initialize(cardIds[topCardIndex]);
        topCardIndex++;
    }

    private void Shuffle()
    {
        // Fisher-Yatesシャッフルアルゴリズム
        for (int i = cardIds.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = cardIds[i];
            cardIds[i] = cardIds[j];
            cardIds[j] = temp;
        }
    }
}
```

### 3. プレイエリア管理

```csharp
using UdonSharp;
using VRC.SDKBase;
using UnityEngine;

public class PlayArea : UdonSharpBehaviour
{
    [SerializeField] private Transform[] playerPositions;
    [SerializeField] private Transform[] handAreas;
    [SerializeField] private Transform battleField;

    private readonly int MAX_PLAYERS = 4;
    private VRCPlayerApi[] players = new VRCPlayerApi[4];

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        AssignPlayerPosition(player);
    }

    private void AssignPlayerPosition(VRCPlayerApi player)
    {
        // プレイヤーの位置割り当て
    }

    public bool IsValidCardPlacement(Vector3 position)
    {
        // カード配置の有効性チェック
        return battleField.GetComponent<BoxCollider>().bounds.Contains(position);
    }
}
```

### 4. ネットワーク同期

```csharp
using UdonSharp;
using VRC.SDKBase;
using UnityEngine;

public class NetworkManager : UdonSharpBehaviour
{
    [UdonSynced] private int currentTurn;
    [UdonSynced] private int activePlayer;

    public override void OnDeserialization()
    {
        // 同期変数が更新された時の処理
        UpdateGameState();
    }

    private void UpdateGameState()
    {
        // ゲーム状態の更新処理
    }

    public bool RequestTurnChange()
    {
        if (!Networking.IsOwner(gameObject)) return false;

        activePlayer = (activePlayer + 1) % 4;
        RequestSerialization();
        return true;
    }
}
```

### 5. UI 管理システム

```csharp
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : UdonSharpBehaviour
{
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Text phaseText;
    [SerializeField] private Text lifePointsText;

    public void UpdatePhaseDisplay(string phase)
    {
        phaseText.text = phase;
    }

    public void UpdateLifePoints(int points)
    {
        lifePointsText.text = $"Life: {points}";
    }
}
```

## パフォーマンス最適化

### 1. アセット最適化

-   テクスチャアトラスの使用
    -   カード画像の統合
    -   メモリ使用量の削減
-   LOD（Level of Detail）設定
    -   遠距離のカード表示を簡略化
    -   描画負荷の軽減

### 2. スクリプト最適化

-   オブジェクトプーリング
    -   カードオブジェクトの再利用
    -   インスタンス化/破棄の削減
-   更新処理の最適化
    -   必要最小限の更新頻度
    -   条件分岐による処理スキップ

### 3. ネットワーク最適化

-   同期変数の最小化
    -   必要な情報のみを同期
    -   バッチ処理による同期
-   権限管理
    -   オーナーシップの適切な移譲
    -   同期処理の分散

## デバッグ・テスト方法

1. Unity Editor

    - UdonSharp のローカルテスト
    - UI 配置の確認
    - 基本動作の検証

2. VRChat クライアント

    - マルチプレイヤーテスト
    - ネットワーク同期確認
    - パフォーマンス計測

3. ビルド設定
    - Android 向け最適化
    - VR 対応確認
    - クロスプラットフォームテスト
