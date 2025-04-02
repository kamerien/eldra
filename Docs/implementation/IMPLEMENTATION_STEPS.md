# 実装手順書

## 1. 基盤システム実装

### 1.1 データ構造の実装

```csharp
// カードデータ構造
public struct SharedCardData
{
    public string id;
    public string name;
    public CardProperties properties;
    public FormatLegality legality;
    public TextureInfo textureInfo;
}

// フォーマット情報
public struct FormatLegality
{
    public bool standardLegal;
    public bool legacyLegal;
    public bool vintageLegal;
    public string[] restrictions;
}

// テクスチャ情報
public struct TextureInfo
{
    public int atlasIndex;
    public Vector4 uvRect;
    public bool isHighQuality;
    public int cacheStatus;
}
```

### 1.2 ワールド管理システム

```csharp
// ワールド情報
public class WorldConfig
{
    public string worldId;
    public string format;
    public int capacity;
    public string[] requiredAssets;
    public MemoryLimits memoryLimits;
}

// メモリ制限
public struct MemoryLimits
{
    public int maxTextureMemory;
    public int maxAssetMemory;
    public int maxCacheSize;
    public float cleanupThreshold;
}
```

## 2. フェーズ別実装手順

### Phase 1: 基本システム

```yaml
1. データ管理:
    - カードデータベース実装
    - アセット管理システム
    - キャッシュ管理

2. UI実装:
    - デッキビルダー
    - フォーマット切替
    - 検索システム

3. ネットワーク:
    - 同期システム
    - 状態管理
    - エラーハンドリング
```

### Phase 2: 最適化実装

```yaml
1. メモリ管理:
    - 動的ローディング
    - キャッシュシステム
    - リソース解放

2. パフォーマンス:
    - テクスチャ圧縮
    - アトラス最適化
    - バッチ処理

3. 監視システム:
    - メモリモニタリング
    - パフォーマンス計測
    - エラーロギング
```

### Phase 3: フォーマット拡張

```yaml
1. Legacy対応:
    - カードデータ追加
    - ルール実装
    - UI拡張

2. Vintage対応:
    - 追加カード実装
    - 特殊ルール対応
    - バランス調整
```

## 3. コンポーネント実装

### 3.1 カードローダー

```csharp
public class CardLoader : UdonSharpBehaviour
{
    // キャッシュ設定
    [SerializeField] private int maxCachedCards = 1000;
    [SerializeField] private float cacheTimeout = 300f;

    private Dictionary<string, CardData> cardCache;
    private Queue<string> loadQueue;
    private Dictionary<string, float> lastAccessTime;

    public void LoadCard(string cardId)
    {
        if (cardCache.ContainsKey(cardId))
        {
            UpdateAccessTime(cardId);
            return;
        }

        QueueCardLoad(cardId);
        CleanupOldCache();
    }

    private void QueueCardLoad(string cardId)
    {
        loadQueue.Enqueue(cardId);
        ProcessLoadQueue();
    }

    private void CleanupOldCache()
    {
        var currentTime = Time.time;
        var cardsToRemove = lastAccessTime
            .Where(kvp => currentTime - kvp.Value > cacheTimeout)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var cardId in cardsToRemove)
        {
            RemoveFromCache(cardId);
        }
    }
}
```

### 3.2 ワールド管理

```csharp
public class WorldManager : UdonSharpBehaviour
{
    // ワールド設定
    [SerializeField] private WorldConfig[] worldConfigs;
    [SerializeField] private float syncInterval = 1.0f;

    private Dictionary<string, WorldState> worldStates;
    private Queue<WorldTransition> transitionQueue;

    public void InitiateWorldTransition(string targetWorldId)
    {
        var transition = new WorldTransition
        {
            sourceWorld = currentWorldId,
            targetWorld = targetWorldId,
            playerData = GetCurrentPlayerData(),
            timestamp = GetNetworkedTime()
        };

        QueueTransition(transition);
    }

    private void ProcessTransitions()
    {
        while (transitionQueue.Count > 0)
        {
            var transition = transitionQueue.Peek();
            if (CanProcessTransition(transition))
            {
                ExecuteTransition(transition);
                transitionQueue.Dequeue();
            }
            else
            {
                break;
            }
        }
    }
}
```

### 3.3 メモリ管理

```csharp
public class MemoryManager : UdonSharpBehaviour
{
    // メモリ設定
    [SerializeField] private float memoryThreshold = 0.8f;
    [SerializeField] private int cleanupInterval = 60;

    private Dictionary<string, ResourceUsage> resourceUsage;
    private Queue<string> resourceQueue;

    public void TrackResource(string resourceId, int memorySize)
    {
        resourceUsage[resourceId] = new ResourceUsage
        {
            size = memorySize,
            lastAccess = Time.time
        };

        CheckMemoryUsage();
    }

    private void CheckMemoryUsage()
    {
        var totalUsage = resourceUsage.Sum(r => r.Value.size);
        var maxMemory = SystemInfo.systemMemorySize * memoryThreshold;

        if (totalUsage > maxMemory)
        {
            CleanupResources();
        }
    }
}
```

## 4. テスト計画

### 4.1 単体テスト

```yaml
テスト項目:
    カードロード:
        - 正常系テスト
        - エラーハンドリング
        - パフォーマンス計測

    メモリ管理:
        - リソース追跡
        - クリーンアップ動作
        - 閾値処理

    同期システム:
        - データ転送
        - 状態同期
        - エラー回復
```

### 4.2 結合テスト

```yaml
テストシナリオ:
    ワールド間移動:
        - データ保持確認
        - 状態復元確認
        - エラー処理確認

    フォーマット切替:
        - カードロード確認
        - ルール適用確認
        - UI更新確認
```

## 5. デプロイ手順

### 5.1 ステージング環境

```yaml
準備: 1. テストワールドの作成
    2. データセットの準備
    3. モニタリング設定

検証: 1. 基本機能確認
    2. パフォーマンス測定
    3. エラー発生確認
```

### 5.2 本番環境

```yaml
デプロイ手順: 1. アセットのビルド
    2. ワールドのアップロード
    3. 設定の適用
    4. 動作確認

モニタリング: 1. 使用状況確認
    2. エラー監視
    3. パフォーマンス追跡
```
