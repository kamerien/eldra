using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UVMapAsset", menuName = "MTG/UV Map Asset")]
public class UVMapAsset : ScriptableObject
{
    [System.Serializable]
    public class CardUVData
    {
        public string cardId;
        public int atlasIndex;
        public Vector4 uvRect;  // xy: offset, zw: scale
    }

    [Header("Atlas Information")]
    public int totalAtlases;
    public Vector2Int atlasSize = new Vector2Int(4096, 4096);
    public string lastUpdated;

    [Header("UV Mapping")]
    public CardUVData[] uvMappings;

    // 高速検索用のインデックス
    private Dictionary<string, CardUVData> uvIndex;

    public void Initialize(Dictionary<string, Vector4> uvMap)
    {
        List<CardUVData> mappings = new List<CardUVData>();
        
        foreach (var kvp in uvMap)
        {
            var mapping = new CardUVData
            {
                cardId = kvp.Key,
                atlasIndex = (int)kvp.Value.x,
                uvRect = new Vector4(kvp.Value.y, kvp.Value.z, kvp.Value.w, 1)
            };
            mappings.Add(mapping);
        }

        uvMappings = mappings.ToArray();
        lastUpdated = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    // 実行時の初期化
    public void InitializeRuntime()
    {
        uvIndex = new Dictionary<string, CardUVData>();
        foreach (var mapping in uvMappings)
        {
            uvIndex[mapping.cardId] = mapping;
        }
    }

    // UV情報の取得
    public bool TryGetCardUV(string cardId, out CardUVData uvData)
    {
        if (uvIndex == null)
        {
            InitializeRuntime();
        }

        return uvIndex.TryGetValue(cardId, out uvData);
    }

    // アトラスインデックスごとのUVデータ取得
    public CardUVData[] GetUVDataForAtlas(int atlasIndex)
    {
        return System.Array.FindAll(uvMappings, mapping => mapping.atlasIndex == atlasIndex);
    }

    // UV座標の検証
    public bool ValidateUVMapping(string cardId, int atlasIndex, Vector4 uvRect)
    {
        if (uvIndex == null)
        {
            InitializeRuntime();
        }

        if (uvIndex.TryGetValue(cardId, out CardUVData data))
        {
            // アトラスインデックスとUV座標が一致するか確認
            return data.atlasIndex == atlasIndex && 
                   Vector4.Distance(data.uvRect, uvRect) < 0.001f;
        }
        return false;
    }

    // UV情報の更新
    public void UpdateUVData(string cardId, int atlasIndex, Vector4 uvRect)
    {
        if (uvIndex == null)
        {
            InitializeRuntime();
        }

        if (uvIndex.TryGetValue(cardId, out CardUVData data))
        {
            data.atlasIndex = atlasIndex;
            data.uvRect = uvRect;
        }
        else
        {
            // 新しいマッピングの追加
            var newMapping = new CardUVData
            {
                cardId = cardId,
                atlasIndex = atlasIndex,
                uvRect = uvRect
            };

            // 配列の拡張
            System.Array.Resize(ref uvMappings, uvMappings.Length + 1);
            uvMappings[uvMappings.Length - 1] = newMapping;
            uvIndex[cardId] = newMapping;
        }
    }

    // UV情報の最適化
    public void OptimizeUVData()
    {
        // 未使用のマッピングを削除
        var usedMappings = new List<CardUVData>();
        foreach (var mapping in uvMappings)
        {
            if (mapping != null && !string.IsNullOrEmpty(mapping.cardId))
            {
                usedMappings.Add(mapping);
            }
        }

        uvMappings = usedMappings.ToArray();
        uvIndex = null; // 再初期化を強制
    }

    // デバッグ情報
    public string GetDebugInfo()
    {
        return $"Total Mappings: {uvMappings.Length}\n" +
               $"Atlas Size: {atlasSize.x}x{atlasSize.y}\n" +
               $"Total Atlases: {totalAtlases}\n" +
               $"Last Updated: {lastUpdated}";
    }

#if UNITY_EDITOR
    // エディタ用の検証メソッド
    public void ValidateAllMappings()
    {
        var duplicates = new HashSet<string>();
        var invalidMappings = new List<string>();

        foreach (var mapping in uvMappings)
        {
            // カードIDの重複チェック
            if (!duplicates.Add(mapping.cardId))
            {
                Debug.LogError($"Duplicate mapping found for card ID: {mapping.cardId}");
            }

            // UV座標の範囲チェック
            if (mapping.uvRect.x < 0 || mapping.uvRect.x > 1 ||
                mapping.uvRect.y < 0 || mapping.uvRect.y > 1 ||
                mapping.uvRect.z < 0 || mapping.uvRect.z > 1 ||
                mapping.uvRect.w < 0 || mapping.uvRect.w > 1)
            {
                invalidMappings.Add(mapping.cardId);
            }
        }

        if (invalidMappings.Count > 0)
        {
            Debug.LogError($"Invalid UV mappings found for cards: {string.Join(", ", invalidMappings)}");
        }
    }
#endif
}
