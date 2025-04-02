# MTG VRChat World Project

## 概要

VRChat 上で MTG を遊べるワールドを提供するプロジェクトです。Standard フォーマットを基本とし、将来的に Legacy/Vintage フォーマットへの対応を計画しています。

## 特徴

-   最適化されたカードシステム（153MB）
-   VR/デスクトップ両対応
-   パフォーマンス重視設計

## システム要件

### VR

```yaml
推奨:
    - Oculus Quest 2/3
    - Valve Index
    - HTC Vive
最小:
    - Oculus Rift S
    - Windows Mixed Reality
```

### デスクトップ

```yaml
推奨:
    CPU: i5-9400以上
    GPU: GTX 1660以上
    RAM: 16GB以上
最小:
    CPU: i5-6500
    GPU: GTX 1060 3GB
    RAM: 8GB
```

## プロジェクト構造

```
Unity/
├── Assets/
│   ├── Scripts/        # コアスクリプト
│   ├── Resources/      # リソースファイル
│   ├── Prefabs/        # プレハブ
│   └── Documentation/  # ドキュメント
├── Editor/             # エディタツール
└── Tools/              # 開発ツール
```

## セットアップ手順

1. リポジトリのクローン

```bash
git clone [repository-url]
cd [project-directory]
```

2. 依存パッケージのインストール

```
- Unity 2022.3.6f1
- VRChat SDK3
- UdonSharp
```

3. プロジェクトの初期化

```
1. Unity Editorを開く
2. Tools > MTG > Setup Project Structure を実行
3. アセットの生成を待機
```

## ドキュメント一覧

### 開発者向け

-   [プロジェクト構造](Project_Structure.md)
-   [セットアップガイド](SETUP_PROJECT.md)
-   [実装手順](IMPLEMENTATION_STEPS.md)
-   [テスト計画](TEST_PLAN.md)

### 設計ドキュメント

-   [ワールド実装](VRCHAT_IMPLEMENTATION.md)
-   [最適化計画](STANDARD_OPTIMIZATION.md)
-   [Legacy/Vintage 対応](LEGACY_VINTAGE_SUPPORT.md)
-   [同期システム](WORLD_SYNC_SYSTEM.md)

### ユーザー向け

-   [ユーザーガイド](USER_GUIDE.md)
-   [トラブルシューティング](TROUBLESHOOTING.md)

## 主要機能

### カードシステム

```yaml
基本機能:
    - カード表示/操作
    - デッキ構築
    - 対戦システム

最適化機能:
    - テクスチャアトラス
    - 動的ローディング
    - メモリ管理
```

### ネットワーク機能

```yaml
同期システム:
    - カード状態同期
    - プレイヤー管理
    - ゲーム進行同期

セーフティ機能:
    - エラー検出
    - 自動リカバリー
    - 状態バックアップ
```

## 開発ロードマップ

### Phase 1: 基本実装

```yaml
Standard対応:
    - 基本システム実装
    - パフォーマンス最適化
    - 安定性向上
```

### Phase 2: 機能拡張

```yaml
Legacy対応:
    - データベース拡張
    - UI/UX改善
    - 運用体制確立
```

### Phase 3: 完全版

```yaml
Vintage対応:
    - 全カード対応
    - イベント機能
    - コミュニティ機能
```

## コントリビューション

### バグ報告

1. Issues に報告
2. 再現手順の記載
3. ログと環境情報の添付

### プルリクエスト

1. 開発ブランチの作成
2. コーディング規約の順守
3. テストの実施
4. プルリクエストの作成

## ライセンス

このプロジェクトは MIT ライセンスの下で公開されています。

## サポート

-   [Discord](https://discord.gg/xxx)
-   [Issues](https://github.com/xxx/issues)
-   [Wiki](https://github.com/xxx/wiki)
