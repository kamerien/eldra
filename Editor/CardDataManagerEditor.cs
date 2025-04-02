using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading.Tasks;

[CustomEditor(typeof(CardDataManager))]
public class CardDataManagerEditor : Editor
{
    private bool showFormatSettings = true;
    private bool showMemorySettings = true;
    private bool showDebugOptions = false;

    public override void OnInspectorGUI()
    {
        var manager = (CardDataManager)target;

        EditorGUILayout.Space(10);
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("MTG Card Data Manager", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // フォーマット設定
            showFormatSettings = EditorGUILayout.Foldout(showFormatSettings, "Format Settings", true);
            if (showFormatSettings)
            {
                EditorGUI.indentLevel++;
                DrawFormatSettings();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // メモリ設定
            showMemorySettings = EditorGUILayout.Foldout(showMemorySettings, "Memory Settings", true);
            if (showMemorySettings)
            {
                EditorGUI.indentLevel++;
                DrawMemorySettings();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // デバッグオプション
            showDebugOptions = EditorGUILayout.Foldout(showDebugOptions, "Debug Options", true);
            if (showDebugOptions)
            {
                EditorGUI.indentLevel++;
                DrawDebugOptions();
                EditorGUI.indentLevel--;
            }
        }

        EditorGUILayout.Space(10);
        DrawActionButtons();
    }

    private void DrawFormatSettings()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            SerializedProperty formatProps = serializedObject.FindProperty("includeStandard");
            EditorGUILayout.PropertyField(formatProps, new GUIContent("Include Standard"));
            formatProps = serializedObject.FindProperty("includePioneer");
            EditorGUILayout.PropertyField(formatProps, new GUIContent("Include Pioneer"));
            formatProps = serializedObject.FindProperty("includeModern");
            EditorGUILayout.PropertyField(formatProps, new GUIContent("Include Modern"));
            formatProps = serializedObject.FindProperty("includeLegacy");
            EditorGUILayout.PropertyField(formatProps, new GUIContent("Include Legacy"));
            formatProps = serializedObject.FindProperty("includeVintage");
            EditorGUILayout.PropertyField(formatProps, new GUIContent("Include Vintage"));
            formatProps = serializedObject.FindProperty("includePauper");
            EditorGUILayout.PropertyField(formatProps, new GUIContent("Include Pauper"));
        }
    }

    private void DrawMemorySettings()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            SerializedProperty memProps = serializedObject.FindProperty("batchSize");
            EditorGUILayout.PropertyField(memProps, new GUIContent("Batch Size"));
            memProps = serializedObject.FindProperty("requestDelay");
            EditorGUILayout.PropertyField(memProps, new GUIContent("Request Delay (ms)"));
        }
    }

    private void DrawDebugOptions()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            if (GUILayout.Button("Clear Card Cache"))
            {
                ClearCardCache();
            }

            if (GUILayout.Button("Validate Card Data"))
            {
                ValidateCardData();
            }

            if (GUILayout.Button("Generate Debug Report"))
            {
                GenerateDebugReport();
            }
        }
    }

    private void DrawActionButtons()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Fetch Card Data", GUILayout.Height(30)))
            {
                FetchCardData();
            }

            if (GUILayout.Button("Generate Atlas", GUILayout.Height(30)))
            {
                GenerateTextureAtlas();
            }
        }

        if (GUILayout.Button("Build All Resources", GUILayout.Height(40)))
        {
            BuildAllResources();
        }
    }

    private async void FetchCardData()
    {
        EditorUtility.DisplayProgressBar("Fetching Card Data", "Initializing...", 0f);
        
        try
        {
            var fetcher = CreateInstance<ScryfallDataFetcher>();
            await Task.Run(() => fetcher.FetchCardData());
            
            EditorUtility.DisplayProgressBar("Fetching Card Data", "Processing...", 0.5f);
            AssetDatabase.Refresh();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private void GenerateTextureAtlas()
    {
        EditorUtility.DisplayProgressBar("Generating Atlas", "Processing textures...", 0f);
        
        try
        {
            // テクスチャアトラスの生成処理
            string texturePath = "Assets/Resources/CardTextures";
            if (Directory.Exists(texturePath))
            {
                // アトラス生成設定
                var settings = new TextureImporterSettings();
                settings.maxTextureSize = 4096;
                settings.textureFormat = TextureImporterFormat.DXT5;

                // アトラス生成とUVマップの作成
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private void BuildAllResources()
    {
        EditorUtility.DisplayProgressBar("Building Resources", "Starting build process...", 0f);
        
        try
        {
            // データのフェッチ
            FetchCardData();
            
            // アトラスの生成
            GenerateTextureAtlas();
            
            // 最適化処理
            OptimizeResources();
            
            AssetDatabase.Refresh();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private void ClearCardCache()
    {
        if (EditorUtility.DisplayDialog("Clear Cache", 
            "Are you sure you want to clear the card cache?", 
            "Clear", "Cancel"))
        {
            string cachePath = "Assets/Resources/CardData/Cache";
            if (Directory.Exists(cachePath))
            {
                Directory.Delete(cachePath, true);
                AssetDatabase.Refresh();
            }
        }
    }

    private void ValidateCardData()
    {
        EditorUtility.DisplayProgressBar("Validating", "Checking card data...", 0f);
        
        try
        {
            // カードデータの検証
            string dataPath = "Assets/Resources/CardData";
            if (Directory.Exists(dataPath))
            {
                // 各フォーマットのデータを検証
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private void GenerateDebugReport()
    {
        string reportPath = "CardDataDebugReport.txt";
        using (StreamWriter writer = new StreamWriter(reportPath))
        {
            writer.WriteLine("MTG Card Data Debug Report");
            writer.WriteLine("==========================");
            
            // 各種情報の収集と書き出し
        }
        
        EditorUtility.RevealInFinder(reportPath);
    }

    private void OptimizeResources()
    {
        // テクスチャ圧縮の最適化
        // アセットバンドルの生成
        // キャッシュの最適化
    }
}
