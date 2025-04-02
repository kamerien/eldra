# MTG ワールド 開発クイックスタート

## 1. 環境セットアップ

### 必要なツール

```
- Unity 2022.3.6f1
- VRChat SDK3
- UdonSharp
- Visual Studio 2019/2022
```

### 初期設定

1. プロジェクトのクローン

```bash
git clone [repository-url]
cd [project-directory]
```

2. Unity 開始

```
- プロジェクトを開く
- VRChat SDKをインポート
- UdonSharpをインポート
```

## 2. カードデータのセットアップ

### Standard フォーマットの場合

1. データ取得

```
Tools > MTG > Card Asset Builder
- [Standard]を選択
- [Fetch Card Data]をクリック
- ダウンロード完了まで待機
```

2. テクスチャ生成

```
Tools > MTG > Build Optimizer
- [テクスチャ最適化]にチェック
- [最適化実行]をクリック
- アトラス生成完了まで待機
```

### 容量確認

```
Tools > MTG > Build Optimizer
- [サイズ計算]をクリック
- 合計が200MB以下であることを確認
```

## 3. ワールドのセットアップ

1. シーン作成

```
- 新規シーンを作成
- VRCWorldPrefabを配置
- PlayerSpawnPointsを設定
```

2. コンポーネント配置

```
- CardManagerをシーンに追加
- TableManagerを配置
- UIManagerを設定
```

3. プレイエリアの設定

```
- テーブルの配置
- プレイヤー座席の設定
- カメラ視点の調整
```

## 4. ビルドと最適化

1. 最適化実行

```
Tools > MTG > Build Optimizer
[✓] テクスチャ最適化
[✓] メッシュ最適化
[✓] ライトマップ生成
[✓] アセットバンドル生成

- [最適化実行]をクリック
- 処理完了まで待機
```

2. パフォーマンスチェック

```
- カードの描画テスト
- UI応答性の確認
- フレームレートの確認
```

3. ビルド

```
Tools > MTG > Build Optimizer
- [ビルド＆アップロード]をクリック
- VRChat SDKの指示に従う
```

## 5. テスト手順

1. ローカルテスト

```
- VRChatクライアントを起動
- テストワールドに入室
- 基本機能の動作確認
```

2. 動作確認項目

```
□ カードの描画
□ ドラッグ＆ドロップ
□ UI操作
□ ネットワーク同期
□ パフォーマンス
```

3. マルチプレイテスト

```
□ プレイヤー参加処理
□ 同期状態
□ 座席システム
□ インタラクション
```

## 6. トラブルシューティング

### ビルドエラー

```
Q: アセット最適化に失敗する
A: Tools > MTG > Build Optimizer から
   [クリーンアップ]を実行して再試行

Q: テクスチャが表示されない
A: Card Asset Builderで
   データを再取得
```

### パフォーマンス問題

```
Q: フレームレート低下
A: BuildOptimizerで
   [テクスチャ最適化]を実行

Q: メモリ使用量過多
A: アセットバンドルを再生成
```

### 同期問題

```
Q: カードの同期ずれ
A: NetworkManagerの
   設定を確認

Q: UIの更新遅延
A: UpdateRateを調整
```

## 7. 更新手順

### パッチ更新

```
1. ScryfallDataFetcherで
   最新データを取得

2. CardAssetBuilderで
   アセットを再生成

3. BuildOptimizerで
   最適化を実行
```

### フォーマット更新

```
1. Tools > MTG > Card Asset Builder
   - 更新するフォーマットを選択
   - データを再取得

2. BuildOptimizerで最適化
   - テクスチャ再生成
   - アセット最適化
```
