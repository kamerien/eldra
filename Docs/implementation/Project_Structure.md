# プロジェクト構造

```
Unity/
├── Assets/
│   ├── Scripts/
│   │   ├── Card/           # カード関連のコアスクリプト
│   │   │   ├── CardBehaviour.cs
│   │   │   ├── CardDataAsset.cs
│   │   │   ├── CardDataManager.cs
│   │   │   ├── CardResourceLoader.cs
│   │   │   ├── UVMapAsset.cs
│   │   │   └── VRCCardDatabase.cs
│   │   │
│   │   ├── Game/           # ゲームロジック
│   │   │   └── GameManager.cs
│   │   │
│   │   ├── Deck/           # デッキ管理
│   │   │   └── DeckManager.cs
│   │   │
│   │   ├── UI/            # UI関連
│   │   │   └── GameUI.cs
│   │   │
│   │   ├── Network/       # ネットワーク同期
│   │   └── Utils/         # ユーティリティ
│   │
│   ├── Resources/
│   │   ├── CardData/      # カードデータJSON
│   │   │   ├── Standard/  # Standardフォーマット
│   │   │   ├── Legacy/    # Legacyフォーマット
│   │   │   └── Vintage/   # Vintageフォーマット
│   │   │
│   │   ├── CardAtlases/   # テクスチャアトラス
│   │   │   ├── Standard/
│   │   │   ├── Legacy/
│   │   │   └── Vintage/
│   │   │
│   │   └── Prefabs/       # 共有プレハブ
│   │
│   ├── Editor/
│   │   ├── Tools/         # エディタツール
│   │   │   ├── BuildOptimizer.cs
│   │   │   ├── CardAssetBuilder.cs
│   │   │   └── ScryfallDataFetcher.cs
│   │   │
│   │   ├── Inspectors/    # カスタムインスペクタ
│   │   │   └── CardDataManagerEditor.cs
│   │   │
│   │   └── Windows/       # エディタウィンドウ
│   │
│   ├── Prefabs/
│   │   ├── Cards/         # カードプレハブ
│   │   │   └── Card.prefab
│   │   │
│   │   ├── UI/           # UI要素
│   │   │   └── PlayerUI.prefab
│   │   │
│   │   ├── GameSystem/   # ゲームシステム
│   │   │   └── TableManager.prefab
│   │   │
│   │   └── Effects/      # エフェクト
│   │
│   ├── Materials/
│   │   ├── Cards/        # カード用マテリアル
│   │   ├── UI/          # UI用マテリアル
│   │   └── Effects/     # エフェクト用マテリアル
│   │
│   ├── Textures/
│   │   ├── Cards/       # カードテクスチャ
│   │   ├── UI/         # UI用テクスチャ
│   │   └── Icons/      # アイコン類
│   │
│   ├── Models/
│   │   ├── Table/      # テーブルモデル
│   │   └── Props/      # 小物類
│   │
│   ├── Audio/
│   │   ├── BGM/        # 背景音楽
│   │   └── SFX/        # 効果音
│   │
│   └── Documentation/   # ドキュメント
│       ├── Images/      # 説明用画像
│       └── *.md         # 各種ドキュメント
│
└── Tools/              # 開発用ツール
    └── ProjectSetup.cs  # プロジェクト構造セットアップ

```

## フォルダ構造の説明

### Scripts/

-   `Card/`: カードゲームの中核となるスクリプト
-   `Game/`: ゲーム進行やルール管理
-   `Deck/`: デッキ構築と管理
-   `UI/`: ユーザーインターフェース
-   `Network/`: VRChat 向けの同期処理
-   `Utils/`: ユーティリティ関数

### Resources/

-   `CardData/`: 各フォーマットのカードデータ
-   `CardAtlases/`: 最適化されたテクスチャアトラス
-   `Prefabs/`: 共有プレハブ

### Editor/

-   `Tools/`: アセット生成・最適化ツール
-   `Inspectors/`: カスタムインスペクタ
-   `Windows/`: エディタ拡張ウィンドウ

### Prefabs/

-   `Cards/`: カード表示用プレハブ
-   `UI/`: インターフェース要素
-   `GameSystem/`: ゲームシステム
-   `Effects/`: 視覚効果

### Assets/

-   `Materials/`: マテリアル設定
-   `Textures/`: 画像アセット
-   `Models/`: 3D モデル
-   `Audio/`: 音声ファイル

### Documentation/

-   プロジェクトドキュメント
-   実装ガイド
-   設定リファレンス
