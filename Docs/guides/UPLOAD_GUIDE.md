# VRChat ワールドのアップロード手順

## 1. 最終確認

### ワールド設定

```yaml
World Settings:
    Name: MTG Standard Format World
    Description: |
        MTGをVRChatで遊べるワールド（Standardフォーマット対応）
        - 最大4人対戦
        - デッキビルド機能
        - カード検索機能
    Capacity: 16
    Tags:
        - game
        - cards
        - tabletop
```

### パフォーマンスランク

```yaml
Target Rank: Good
Requirements:
    - Draw Calls: < 100
    - Materials: < 20
    - Meshes: < 100
    - Particle Systems: < 10
    - Audio Sources: < 10
    - Bones: < 2000
```

### セーフティチェック

```yaml
Pre-upload Checks:
    Content: □ 著作権違反がないか確認
        □ コミュニティガイドラインに準拠
        □ アバター干渉がないか確認
        □ 不適切なコンテンツがないか確認

    Technical: □ シェーダーがVRChat対応か確認
        □ ライティングが正しく設定されているか
        □ コライダーの配置が適切か
        □ ネットワーク同期が正しく動作するか
```

## 2. 最終ビルド手順

### 2.1 アセットの準備

```bash
1. プロジェクトクリーンアップ
   - 未使用アセットの削除
   - 重複ファイルの削除
   - メタファイルの整理

2. リソース最適化
   - アトラステクスチャの生成
   - ライトマップのベイク
   - シーン最適化
```

### 2.2 VRChat SDK 設定

```yaml
SDK Settings:
    Pipeline: Built-in
    Layer Setup: VRChat Default
    Collision Matrix: Default
    Audio Setup: Default

Build Settings:
    Platform: Windows
    Architecture: x64
    Compression: Default
```

### 2.3 ビルド手順

```bash
1. Tools > MTG > Build Optimizer
   - 全最適化オプションを有効化
   - [最適化実行]をクリック
   - 完了まで待機

2. VRChat SDK > Show Control Panel
   - [Build & Test]をクリック
   - ローカルテストを実行
   - 問題がないことを確認

3. VRChat SDK > Build & Publish
   - ワールド情報を入力
   - プラットフォーム設定を確認
   - アップロード開始
```

## 3. テスト手順

### 3.1 ローカルテスト

```yaml
Test Environment:
    VRChat Client: Latest Version
    Platform: PC (VR/Desktop)
    Test Duration: 30 minutes minimum

Check Items:
    Basic: □ ワールドのロード
        □ プレイヤースポーン
        □ 基本移動/操作

    Game Features: □ カードの描画
        □ ドラッグ＆ドロップ
        □ デッキビルド
        □ 検索機能

    Performance: □ フレームレート
        □ メモリ使用量
        □ ネットワーク遅延
```

### 3.2 フレンドテスト

```yaml
Test Scenarios:
    2 Players: □ 基本対戦
        □ カード操作
        □ 同期状態

    4 Players: □ 満員時の動作
        □ パフォーマンス
        □ ネットワーク安定性
```

## 4. 公開後の対応

### 4.1 モニタリング

```yaml
Monitor Items:
    - インスタンス数
    - 同時接続数
    - エラーレポート
    - フィードバック
    - パフォーマンス統計
```

### 4.2 メンテナンス計画

```yaml
Regular Updates:
    Daily:
        - インスタンス状態確認
        - エラーログチェック

    Weekly:
        - パフォーマンス分析
        - フィードバック確認

    Monthly:
        - カードデータ更新
        - 新機能の検討
        - 最適化の見直し
```

### 4.3 緊急対応

```yaml
Emergency Procedures:
    Critical Issues:
        - クラッシュ
        - 同期エラー
        - セーフティ違反

    Response Plan: 1. 問題の切り分け
        2. 一時的な対策実施
        3. 恒久的な解決策の実装
        4. 再発防止策の導入
```

## 5. 更新手順

### 5.1 パッチ更新

```yaml
Update Process: 1. テストワールドで変更をテスト
    2. VRChat SDKで新バージョンをビルド
    3. テストユーザーによる確認
    4. 本番ワールドの更新
```

### 5.2 メジャーアップデート

```yaml
Major Update Steps: 1. 開発環境での実装とテスト
    2. クローズドベータテスト
    3. フィードバック収集と修正
    4. 本番環境への展開
    5. アナウンスと告知
```

## 6. バックアップ計画

### 6.1 データバックアップ

```yaml
Backup Items:
    - プロジェクトファイル
    - アセットデータ
    - ビルド設定
    - カードデータベース
    - ユーザー設定

Backup Schedule:
    - 毎日: 差分バックアップ
    - 毎週: フルバックアップ
    - 更新前: 完全バックアップ
```

### 6.2 復旧手順

```yaml
Recovery Steps: 1. 問題の特定と切り分け
    2. 該当バックアップの特定
    3. テスト環境での復旧確認
    4. 本番環境への適用
    5. 動作確認と監視
```
