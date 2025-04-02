using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class CardAssetBuilder : EditorWindow
{
    private const string ASSET_PATH = "Assets/Resources/CardData";
    private const string ATLAS_PATH = "Assets/Resources/CardAtlases";
    private const int ATLAS_SIZE = 4096;
    private const int CARDS_PER_ATLAS = 100;

    private bool[] formatEnabled = new bool[6];
    private string[] formats = { "Standard", "Pioneer", "Modern", "Legacy", "Vintage", "Pauper" };
    private bool showAdvancedSettings = false;
    private bool compressTextures = true;
    private bool generateMipmaps = false;

    [MenuItem("Tools/MTG/Card Asset Builder")]
    public static void ShowWindow()
    {
        GetWindow<CardAssetBuilder>("Card Asset Builder");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("MTG Card Asset Builder", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        // フォーマット選択
        EditorGUILayout.LabelField("Build Formats", EditorStyles.boldLabel);
        for (int i = 0; i < formats.Length; i++)
        {
            formatEnabled[i] = EditorGUILayout.Toggle(formats[i], formatEnabled[i]);
        }

        EditorGUILayout.Space(10);

        // 詳細設定
        showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "Advanced Settings", true);
        if (showAdvancedSettings)
        {
            EditorGUI.indentLevel++;
            compressTextures = EditorGUILayout.Toggle("Compress Textures", compressTextures);
            generateMipmaps = EditorGUILayout.Toggle("Generate Mipmaps", generateMipmaps);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(20);

        // ビルドボタン
        if (GUILayout.Button("Build Card Assets", GUILayout.Height(30)))
        {
            BuildCardAssets();
        }

        // クリーンアップボタン
        if (GUILayout.Button("Cleanup Old Assets"))
        {
            CleanupOldAssets();
        }
    }

    private void BuildCardAssets()
    {
        try
        {
            EditorUtility.DisplayProgressBar("Building Card Assets", "Initializing...", 0f);

            // 出力ディレクトリの作成
            CreateDirectories();

            // 選択されたフォーマットのカードデータを処理
            for (int i = 0; i < formats.Length; i++)
            {
                if (formatEnabled[i])
                {
                    ProcessFormat(formats[i].ToLower());
                }
            }

            // アセットバンドルの生成（必要な場合）
            if (compressTextures)
            {
                BuildAssetBundles();
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", "Card assets built successfully!", "OK");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error building card assets: {e.Message}");
            EditorUtility.DisplayDialog("Error", $"Failed to build card assets:\n{e.Message}", "OK");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private void CreateDirectories()
    {
        Directory.CreateDirectory(ASSET_PATH);
        Directory.CreateDirectory(ATLAS_PATH);
    }

    private void ProcessFormat(string format)
    {
        EditorUtility.DisplayProgressBar("Building Card Assets", $"Processing {format} format...", 0.2f);

        // カードデータの読み込み
        var cardData = LoadCardData(format);
        
        // テクスチャアトラスの生成
        var atlases = GenerateAtlases(cardData);
        
        // UV座標マップの生成
        var uvMap = GenerateUVMap(cardData, atlases);
        
        // 最適化されたデータの保存
        SaveProcessedData(format, cardData, atlases, uvMap);
    }

    private List<CardData> LoadCardData(string format)
    {
        // JSONからカードデータを読み込み
        string jsonPath = Path.Combine(ASSET_PATH, $"{format}/cards.json");
        if (File.Exists(jsonPath))
        {
            string jsonContent = File.ReadAllText(jsonPath);
            // JSONデータをパースして返す
            return new List<CardData>();
        }
        return new List<CardData>();
    }

    private List<Texture2D> GenerateAtlases(List<CardData> cardData)
    {
        var atlases = new List<Texture2D>();
        var currentAtlas = new Texture2D(ATLAS_SIZE, ATLAS_SIZE);
        int cardCount = 0;

        foreach (var card in cardData)
        {
            if (cardCount >= CARDS_PER_ATLAS)
            {
                // アトラスを保存して新しいアトラスを開始
                SaveAtlas(currentAtlas, atlases.Count);
                atlases.Add(currentAtlas);
                currentAtlas = new Texture2D(ATLAS_SIZE, ATLAS_SIZE);
                cardCount = 0;
            }

            // カードテクスチャをアトラスに追加
            AddCardToAtlas(card, currentAtlas);
            cardCount++;
        }

        // 最後のアトラスを保存
        if (cardCount > 0)
        {
            SaveAtlas(currentAtlas, atlases.Count);
            atlases.Add(currentAtlas);
        }

        return atlases;
    }

    private void SaveAtlas(Texture2D atlas, int index)
    {
        string path = Path.Combine(ATLAS_PATH, $"atlas_{index}.asset");
        AssetDatabase.CreateAsset(atlas, path);
        
        // テクスチャ設定の最適化
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureCompression = compressTextures ? 
                TextureImporterCompression.Compressed : 
                TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = generateMipmaps;
            importer.SaveAndReimport();
        }
    }

    private void AddCardToAtlas(CardData card, Texture2D atlas)
    {
        // カードテクスチャをアトラスに追加する処理
    }

    private Dictionary<string, Vector4> GenerateUVMap(List<CardData> cardData, List<Texture2D> atlases)
    {
        // UV座標マップの生成
        var uvMap = new Dictionary<string, Vector4>();
        // マッピング処理
        return uvMap;
    }

    private void SaveProcessedData(string format, List<CardData> cardData, 
        List<Texture2D> atlases, Dictionary<string, Vector4> uvMap)
    {
        // 最適化されたデータの保存
        string formatPath = Path.Combine(ASSET_PATH, format);
        Directory.CreateDirectory(formatPath);

        // カードデータの保存
        string cardDataPath = Path.Combine(formatPath, "processed_cards.asset");
        var cardDataAsset = ScriptableObject.CreateInstance<CardDataAsset>();
        cardDataAsset.Initialize(cardData);
        AssetDatabase.CreateAsset(cardDataAsset, cardDataPath);

        // UVマップの保存
        string uvMapPath = Path.Combine(formatPath, "uv_map.asset");
        var uvMapAsset = ScriptableObject.CreateInstance<UVMapAsset>();
        uvMapAsset.Initialize(uvMap);
        AssetDatabase.CreateAsset(uvMapAsset, uvMapPath);
    }

    private void CleanupOldAssets()
    {
        if (EditorUtility.DisplayDialog("Cleanup Assets",
            "Are you sure you want to delete all existing card assets?",
            "Yes", "Cancel"))
        {
            if (Directory.Exists(ASSET_PATH))
                Directory.Delete(ASSET_PATH, true);
            if (Directory.Exists(ATLAS_PATH))
                Directory.Delete(ATLAS_PATH, true);

            AssetDatabase.Refresh();
        }
    }

    private void BuildAssetBundles()
    {
        // アセットバンドルのビルド（必要な場合）
    }
}
