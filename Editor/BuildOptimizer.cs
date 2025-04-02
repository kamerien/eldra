using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class BuildOptimizer : EditorWindow
{
    private bool optimizeTextures = true;
    private bool optimizeMeshes = true;
    private bool generateLightmaps = true;
    private bool buildAssetBundles = true;

    private string[] formatOptions = { "Standard", "Pioneer", "Modern", "Legacy", "Vintage", "Pauper" };
    private bool[] selectedFormats;
    private Vector2 scrollPosition;

    [MenuItem("Tools/MTG/Build Optimizer")]
    public static void ShowWindow()
    {
        GetWindow<BuildOptimizer>("Build Optimizer");
    }

    private void OnEnable()
    {
        selectedFormats = new bool[formatOptions.Length];
        selectedFormats[0] = true; // Standardをデフォルトで選択
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("MTG ワールド最適化ツール", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
        {
            scrollPosition = scrollView.scrollPosition;

            DrawFormatSelection();
            DrawOptimizationOptions();
            DrawBuildButtons();
        }
    }

    private void DrawFormatSelection()
    {
        EditorGUILayout.LabelField("フォーマット選択", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        for (int i = 0; i < formatOptions.Length; i++)
        {
            selectedFormats[i] = EditorGUILayout.Toggle(formatOptions[i], selectedFormats[i]);
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.Space(10);
    }

    private void DrawOptimizationOptions()
    {
        EditorGUILayout.LabelField("最適化オプション", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        optimizeTextures = EditorGUILayout.Toggle("テクスチャ最適化", optimizeTextures);
        optimizeMeshes = EditorGUILayout.Toggle("メッシュ最適化", optimizeMeshes);
        generateLightmaps = EditorGUILayout.Toggle("ライトマップ生成", generateLightmaps);
        buildAssetBundles = EditorGUILayout.Toggle("アセットバンドル生成", buildAssetBundles);

        EditorGUI.indentLevel--;
        EditorGUILayout.Space(10);
    }

    private void DrawBuildButtons()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("最適化実行", GUILayout.Height(30)))
        {
            OptimizeProject();
        }
        if (GUILayout.Button("サイズ計算", GUILayout.Height(30)))
        {
            CalculateSize();
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("ビルド＆アップロード", GUILayout.Height(40)))
        {
            BuildAndUpload();
        }
    }

    private void OptimizeProject()
    {
        try
        {
            EditorUtility.DisplayProgressBar("最適化中", "プロジェクトの最適化を開始...", 0f);

            if (optimizeTextures)
            {
                OptimizeTextures();
            }

            if (optimizeMeshes)
            {
                OptimizeMeshes();
            }

            if (generateLightmaps)
            {
                GenerateLightmaps();
            }

            if (buildAssetBundles)
            {
                BuildAssetBundles();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("完了", "プロジェクトの最適化が完了しました。", "OK");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"最適化エラー: {e.Message}");
            EditorUtility.DisplayDialog("エラー", $"最適化中にエラーが発生しました:\n{e.Message}", "OK");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private void OptimizeTextures()
    {
        EditorUtility.DisplayProgressBar("テクスチャ最適化", "テクスチャの処理中...", 0.2f);

        // カードテクスチャの最適化
        string[] cardTextures = AssetDatabase.FindAssets("t:texture2D", new[] { "Assets/Resources/CardTextures" });
        for (int i = 0; i < cardTextures.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(cardTextures[i]);
            OptimizeCardTexture(path);

            float progress = (float)i / cardTextures.Length;
            EditorUtility.DisplayProgressBar("テクスチャ最適化", $"カード {i + 1}/{cardTextures.Length}", 0.2f + progress * 0.3f);
        }

        // アトラス生成
        GenerateAtlases();
    }

    private void OptimizeCardTexture(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.maxTextureSize = 512;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.compressionQuality = 100;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }
    }

    private void GenerateAtlases()
    {
        // アトラス設定
        string atlasPath = "Assets/Resources/CardAtlases";
        if (!Directory.Exists(atlasPath))
        {
            Directory.CreateDirectory(atlasPath);
        }

        // アトラス生成処理
        // 実際の実装ではUnityのSprite Atlasシステムを使用
    }

    private void OptimizeMeshes()
    {
        EditorUtility.DisplayProgressBar("メッシュ最適化", "メッシュの処理中...", 0.5f);

        string[] meshes = AssetDatabase.FindAssets("t:model");
        foreach (string guid in meshes)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            OptimizeMesh(path);
        }
    }

    private void OptimizeMesh(string path)
    {
        ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
        if (importer != null)
        {
            importer.meshCompression = ModelImporterMeshCompression.Medium;
            importer.optimizeMesh = true;
            importer.weldVertices = true;
            importer.SaveAndReimport();
        }
    }

    private void GenerateLightmaps()
    {
        EditorUtility.DisplayProgressBar("ライトマップ生成", "ライトマップのベイク中...", 0.7f);

        var lightmapSettings = new LightmapEditorSettings();
        Lightmapping.BakeAsync();
        while (Lightmapping.isRunning)
        {
            System.Threading.Thread.Sleep(100);
        }
    }

    private void BuildAssetBundles()
    {
        EditorUtility.DisplayProgressBar("アセットバンドル", "バンドルのビルド中...", 0.9f);

        string bundlePath = "Assets/AssetBundles";
        if (!Directory.Exists(bundlePath))
        {
            Directory.CreateDirectory(bundlePath);
        }

        BuildPipeline.BuildAssetBundles(
            bundlePath,
            BuildAssetBundleOptions.ChunkBasedCompression,
            BuildTarget.StandaloneWindows64
        );
    }

    private void CalculateSize()
    {
        long totalSize = 0;
        Dictionary<string, long> categorySizes = new Dictionary<string, long>();

        // テクスチャサイズの計算
        string[] textures = AssetDatabase.FindAssets("t:texture2D");
        foreach (string guid in textures)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                long size = new FileInfo(path).Length;
                totalSize += size;
                AddToCategorySize(categorySizes, "Textures", size);
            }
        }

        // メッシュサイズの計算
        string[] meshes = AssetDatabase.FindAssets("t:model");
        foreach (string guid in meshes)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            long size = new FileInfo(path).Length;
            totalSize += size;
            AddToCategorySize(categorySizes, "Meshes", size);
        }

        // 結果表示
        EditorUtility.DisplayDialog("サイズ計算結果",
            $"総サイズ: {FormatSize(totalSize)}\n\n" +
            string.Join("\n", GetCategorySizeReport(categorySizes)),
            "OK");
    }

    private void AddToCategorySize(Dictionary<string, long> sizes, string category, long size)
    {
        if (!sizes.ContainsKey(category))
            sizes[category] = 0;
        sizes[category] += size;
    }

    private string[] GetCategorySizeReport(Dictionary<string, long> sizes)
    {
        List<string> report = new List<string>();
        foreach (var kvp in sizes)
        {
            report.Add($"{kvp.Key}: {FormatSize(kvp.Value)}");
        }
        return report.ToArray();
    }

    private string FormatSize(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < suffixes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {suffixes[order]}";
    }

    private void BuildAndUpload()
    {
        if (EditorUtility.DisplayDialog("ビルド確認",
            "プロジェクトのビルドとアップロードを開始します。\n" +
            "この処理には時間がかかる場合があります。",
            "実行", "キャンセル"))
        {
            OptimizeProject();
            // VRChat SDKのビルド＆アップロード処理を呼び出し
        }
    }
}
