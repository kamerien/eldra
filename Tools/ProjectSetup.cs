using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
public class ProjectSetup : EditorWindow
{
    [MenuItem("Tools/MTG/Setup Project Structure")]
    public static void SetupProjectStructure()
    {
        CreateDirectoryStructure();
        MoveFiles();
        AssetDatabase.Refresh();
        Debug.Log("Project structure setup completed!");
    }

    private static void CreateDirectoryStructure()
    {
        string[] directories = {
            // コアスクリプト
            "Assets/Scripts/Card",
            "Assets/Scripts/Game",
            "Assets/Scripts/Deck",
            "Assets/Scripts/UI",
            "Assets/Scripts/Network",
            "Assets/Scripts/Utils",

            // リソース
            "Assets/Resources/CardData",
            "Assets/Resources/CardData/Standard",
            "Assets/Resources/CardData/Legacy",
            "Assets/Resources/CardData/Vintage",
            "Assets/Resources/CardAtlases",
            "Assets/Resources/Prefabs",

            // エディタツール
            "Assets/Editor/Tools",
            "Assets/Editor/Inspectors",
            "Assets/Editor/Windows",

            // プレハブ
            "Assets/Prefabs/Cards",
            "Assets/Prefabs/UI",
            "Assets/Prefabs/GameSystem",
            "Assets/Prefabs/Effects",

            // その他アセット
            "Assets/Materials/Cards",
            "Assets/Materials/UI",
            "Assets/Materials/Effects",
            "Assets/Textures/Cards",
            "Assets/Textures/UI",
            "Assets/Textures/Icons",
            "Assets/Models/Table",
            "Assets/Models/Props",
            "Assets/Audio/BGM",
            "Assets/Audio/SFX",

            // ドキュメント
            "Assets/Documentation",
            "Assets/Documentation/Images"
        };

        foreach (string dir in directories)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }

    private static void MoveFiles()
    {
        // カードシステム関連のスクリプト移動
        MoveAsset("Assets/Scripts/Card/CardBehaviour.cs", "Assets/Scripts/Card/CardBehaviour.cs");
        MoveAsset("Assets/Scripts/Card/CardDataAsset.cs", "Assets/Scripts/Card/CardDataAsset.cs");
        MoveAsset("Assets/Scripts/Card/CardDataManager.cs", "Assets/Scripts/Card/CardDataManager.cs");
        MoveAsset("Assets/Scripts/Card/CardResourceLoader.cs", "Assets/Scripts/Card/CardResourceLoader.cs");
        MoveAsset("Assets/Scripts/Card/UVMapAsset.cs", "Assets/Scripts/Card/UVMapAsset.cs");
        MoveAsset("Assets/Scripts/Card/VRCCardDatabase.cs", "Assets/Scripts/Card/VRCCardDatabase.cs");

        // デッキ管理関連
        MoveAsset("Assets/Scripts/Deck/DeckManager.cs", "Assets/Scripts/Deck/DeckManager.cs");

        // ゲームシステム関連
        MoveAsset("Assets/Scripts/Game/GameManager.cs", "Assets/Scripts/Game/GameManager.cs");

        // UI関連
        MoveAsset("Assets/Scripts/UI/GameUI.cs", "Assets/Scripts/UI/GameUI.cs");

        // エディタツール
        MoveAsset("Assets/Editor/BuildOptimizer.cs", "Assets/Editor/Tools/BuildOptimizer.cs");
        MoveAsset("Assets/Editor/CardAssetBuilder.cs", "Assets/Editor/Tools/CardAssetBuilder.cs");
        MoveAsset("Assets/Editor/CardDataManagerEditor.cs", "Assets/Editor/Inspectors/CardDataManagerEditor.cs");

        // データフェッチャー
        MoveAsset("Assets/Tools/ScryfallDataFetcher.cs", "Assets/Editor/Tools/ScryfallDataFetcher.cs");

        // ドキュメント
        MoveMarkdownFiles();
    }

    private static void MoveMarkdownFiles()
    {
        string[] mdFiles = {
            "BUILD_SETTINGS.md",
            "CARD_MANAGEMENT.md",
            "IMPLEMENTATION_STEPS.md",
            "LEGACY_VINTAGE_SUPPORT.md",
            "QUICKSTART.md",
            "SETUP_GUIDE.md",
            "STANDARD_OPTIMIZATION.md",
            "SUMMARY.md",
            "UPLOAD_GUIDE.md",
            "WORLD_SIZE_ESTIMATION.md",
            "WORLD_SYNC_SYSTEM.md"
        };

        foreach (string file in mdFiles)
        {
            string sourcePath = Path.Combine("Unity", file);
            string targetPath = Path.Combine("Assets/Documentation", file);
            
            if (File.Exists(sourcePath))
            {
                File.Copy(sourcePath, targetPath, true);
            }
        }
    }

    private static void MoveAsset(string sourcePath, string targetPath)
    {
        if (File.Exists(sourcePath))
        {
            string directory = Path.GetDirectoryName(targetPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            AssetDatabase.MoveAsset(sourcePath, targetPath);
        }
    }
}
#endif
