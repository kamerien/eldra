# ビルド設定ガイド

## テクスチャ設定

### カード画像

```yaml
TextureImporter:
    maxTextureSize: 512
    textureFormat: ASTC_6x6
    compressionQuality: 100
    mipmapEnabled: false
    filterMode: Bilinear
    aniso: 1
    wrapMode: Clamp
    npotScale: ToNearest
```

### アトラス設定

```yaml
TextureAtlas:
    maxAtlasSize: 4096
    padding: 2
    allowRotation: false
    tightPacking: true
    format: ASTC_6x6
    compression: High Quality
```

### UI 要素

```yaml
TextureImporter:
    maxTextureSize: 1024
    textureFormat: ASTC_4x4
    compressionQuality: 100
    mipmapEnabled: false
    filterMode: Bilinear
    aniso: 0
    spritePackingTag: 'UI'
```

## モデル設定

### ワールドジオメトリ

```yaml
ModelImporter:
    meshCompression: Medium
    optimizeMesh: true
    weldVertices: true
    importBlendShapes: false
    generateColliders: false

LODGroup:
    LOD0: 100% # 2m以内
    LOD1: 60% # 5m以内
    LOD2: 30% # 10m以上
```

### コライダー設定

```yaml
MeshCollider:
    convex: false
    cookingOptions: EnableMeshCleaning
    skinWidth: 0.01
```

## ライティング設定

### ライトマップ

```yaml
LightmapSettings:
    resolution: 8
    padding: 2
    compressedLightmaps: true
    directionalMode: NonDirectional
    lightmapsMode: NonDirectional
    mixedBakeMode: 2
```

### リアルタイムライト

```yaml
LightSettings:
    shadowType: Soft Shadows
    shadowResolution: Medium
    shadowDistance: 20
    shadowCascades: 2
```

## VRChat 固有の設定

### ワールド設定

```yaml
VRCWorldSettings:
    capacity: 32
    recommendedCapacity: 16
    optimizationRank: Medium
    preloadAssets: true
```

### SDK 設定

```yaml
SDKSettings:
    forceSDKUpdateMode: false
    detectMobilePlatform: true
    allowUdonNetworking: true
```

### パフォーマンスランク

```yaml
PerformanceSettings:
    targetFrameRate: 72
    vertexLimit: 50000
    materialCount: 20
    drawCallLimit: 100
    particleSystemLimit: 10
```

## アセットバンドル設定

### メインバンドル

```yaml
AssetBundle:
    compression: LZMA
    includeDependencies: true
    strictMode: true
    loadType: LoadFromMemory
```

### カードテクスチャバンドル

```yaml
TextureBundle:
    compression: LZ4
    loadType: LoadFromMemoryAsync
    includeDependencies: false
    chunkBasedCompression: true
```

## 最終ビルド手順

1. プリビルドチェック

```bash
# アセットの整理
- 未使用アセット削除
- 重複アセットチェック
- 参照整合性確認

# テクスチャ最適化
- アトラス生成
- 圧縮設定適用
- ミップマップ設定

# ライトマップベイク
- プログレッシブベイク
- ライトプローブ生成
- リフレクションプローブ更新
```

2. ビルド実行

```bash
# アセットバンドルビルド
- カードテクスチャ生成
- UI要素パッケージ化
- 依存関係チェック

# シーンビルド
- オクルージョンカリング適用
- バッチング最適化
- シャドウデータ生成

# 最終パッケージ化
- マニフェスト生成
- 依存関係の解決
- パッケージ圧縮
```

3. ビルド後チェック

```bash
# パフォーマンス検証
- フレームレート測定
- メモリ使用量確認
- ロード時間計測

# 最適化検証
- Draw Call数確認
- テクスチャメモリ使用量
- バッチ処理効率

# ネットワーク検証
- 同期遅延測定
- パケットサイズ確認
- 帯域使用量チェック
```
