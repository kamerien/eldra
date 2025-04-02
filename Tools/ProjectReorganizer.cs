using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
public class ProjectReorganizer : EditorWindow
{
    [MenuItem("Tools/MTG/Reorganize Project Structure")]
    public static void ReorganizeProject()
    {
        CreateDirectoryStructure();
        MoveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Project reorganization completed!");
    }

    private static void CreateDirectoryStructure()
    {
        string[] directories = {
            // プロジェクトルート
            "Assets/MTG_VRChat",

            // コアシステム
            "Assets/MTG_VRChat/Core",
            "Assets/MTG_VRChat/Core/Scripts",
            "Assets/MTG_VRChat/Core/Prefabs",
            "Assets/MTG_VRChat/Core/Materials",
            "Assets/MTG_VRChat/Core/Textures",

            // カードシステム
            "Assets/MTG_VRChat/CardSystem",
            "Assets/MTG_VRChat/CardSystem/Scripts",
            "Assets/MTG_VRChat/CardSystem/Data",
            "Assets/MTG_VRChat/CardSystem/Prefabs",
            "Assets/MTG_VRChat/CardSystem/Textures",

            // UI
            "Assets/MTG_VRChat/UI",
            "Assets/MTG_VRChat/UI/Scripts",
            "Assets/MTG_VRChat/UI/Prefabs",
            "Assets/MTG_VRChat/UI/Textures",

            // ゲームシステム
            "Assets/MTG_VRChat/GameSystem",
            "Assets/MTG_VRChat/GameSystem/Scripts",
            "Assets/MTG_VRChat/GameSystem/Prefabs",

            // エディタツール
            "Assets/MTG_VRChat/Editor",
            "Assets/MTG_VRChat/Editor/Tools",
            "Assets/MTG_VRChat/Editor/Inspectors",

            // リソース
            "Assets/MTG_VRChat/Resources",
            "Assets/MTG_VRChat/Resources/CardData",
            "Assets/MTG_VRChat/Resources/Prefabs",

            // ドキュメント
            "Assets/MTG_VRChat/Documentation",

            // 開発ツール
            "Assets/MTG_VRChat/Tools"
        };

        foreach (string dir in directories)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }

    private static void MoveAssets()
    {
        // カードシステムの移動
        MoveDirectory("Assets/Scripts/Card", "Assets/MTG_VRChat/CardSystem/Scripts");
        MoveDirectory("Assets/Prefabs/Cards", "Assets/MTG_VRChat/CardSystem/Prefabs");

        // ゲームシステムの移動
        MoveDirectory("Assets/Scripts/Game", "Assets/MTG_VRChat/GameSystem/Scripts");
        MoveDirectory("Assets/Scripts/Deck", "Assets/MTG_VRChat/GameSystem/Scripts");
        MoveDirectory("Assets/Prefabs/GameSystem", "Assets/MTG_VRChat/GameSystem/Prefabs");

        // UIの移動
        MoveDirectory("Assets/Scripts/UI", "Assets/MTG_VRChat/UI/Scripts");
        MoveDirectory("Assets/Prefabs/UI", "Assets/MTG_VRChat/UI/Prefabs");

        // エディタツールの移動
        MoveDirectory("Assets/Editor", "Assets/MTG_VRChat/Editor");
        MoveDirectory("Assets/Tools", "Assets/MTG_VRChat/Tools");

        // その他アセットの移動
        MoveDirectory("Assets/Materials", "Assets/MTG_VRChat/Core/Materials");
        MoveDirectory("Assets/Textures", "Assets/MTG_VRChat/Core/Textures");
        MoveDirectory("Assets/Models", "Assets/MTG_VRChat/Core/Models");

        // ドキュメントの移動
        MoveMarkdownFiles();
    }

    private static void MoveDirectory(string sourcePath, string targetPath)
    {
        if (Directory.Exists(sourcePath))
        {
            string[] files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (file.EndsWith(".meta")) continue;

                string relativePath = file.Replace(sourcePath, "");
                string targetFilePath = Path.Combine(targetPath, relativePath);
                string targetDir = Path.GetDirectoryName(targetFilePath);

                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                AssetDatabase.MoveAsset(file, targetFilePath);
            }
        }
    }

    private static void MoveMarkdownFiles()
    {
        string[] mdFiles = Directory.GetFiles("Unity", "*.md");
        foreach (string file in mdFiles)
        {
            string fileName = Path.GetFileName(file);
            string targetPath = Path.Combine("Assets/MTG_VRChat/Documentation", fileName);
            
            if (File.Exists(file))
            {
                File.Copy(file, targetPath, true);
            }
        }
    }
}
#endif
