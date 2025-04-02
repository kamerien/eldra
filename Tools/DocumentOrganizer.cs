using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
public class DocumentOrganizer : EditorWindow
{
    private static readonly Dictionary<string, string> documentCategories = new Dictionary<string, string>
    {
        // セットアップ関連
        { "QUICKSTART.md", "setup" },
        { "SETUP_PROJECT.md", "setup" },
        { "SETUP_GUIDE.md", "setup" },
        { "VCC_SETUP.md", "setup" },
        { "BUILD_SETTINGS.md", "setup" },

        // 設計関連
        { "PROJECT_STRUCTURE.md", "design" },
        { "Project_Structure_New.md", "design" },
        { "IMPLEMENTATION_STEPS.md", "design" },
        { "IMPLEMENTATION.md", "design" },
        { "VRCHAT_IMPLEMENTATION.md", "design" },
        { "STANDARD_OPTIMIZATION.md", "design" },
        { "LEGACY_VINTAGE_SUPPORT.md", "design" },
        { "WORLD_SYNC_SYSTEM.md", "design" },
        { "SIMPLE_VERSION.md", "design" },

        // ユーザー関連
        { "USER_GUIDE.md", "user" },
        { "TROUBLESHOOTING.md", "user" },

        // 開発者関連
        { "CARD_MANAGEMENT.md", "dev" },
        { "WORLD_SIZE_ESTIMATION.md", "dev" },
        { "TEST_PLAN.md", "dev" },
        { "UPLOAD_GUIDE.md", "dev" },

        // その他
        { "README.md", "meta" },
        { "CONCERNS.md", "meta" },
        { "CONCLUSION.md", "meta" },
        { "SUMMARY.md", "meta" }
    };

    [MenuItem("Tools/MTG/Organize Documentation")]
    public static void OrganizeDocuments()
    {
        CreateDirectoryStructure();
        MoveAllDocuments();
        AssetDatabase.Refresh();
        Debug.Log("Documentation reorganization completed!");
    }

    private static void CreateDirectoryStructure()
    {
        string[] directories = {
            "Docs",
            "Docs/setup",
            "Docs/design",
            "Docs/user",
            "Docs/dev",
            "Docs/meta"
        };

        foreach (string dir in directories)
        {
            string fullPath = Path.Combine(Application.dataPath, "..", dir);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
        }
    }

    private static void MoveAllDocuments()
    {
        string rootPath = Path.Combine(Application.dataPath, "..");
        string[] allFiles = Directory.GetFiles(rootPath, "*.md", SearchOption.TopDirectoryOnly);

        foreach (string filePath in allFiles)
        {
            string fileName = Path.GetFileName(filePath);
            if (documentCategories.TryGetValue(fileName, out string category))
            {
                MoveDocument(fileName, category);
            }
            else
            {
                Debug.LogWarning($"Category not defined for: {fileName}");
            }
        }

        // 新しい場所にINDEX.mdが存在することを確認
        string indexPath = Path.Combine(rootPath, "Docs", "INDEX.md");
        if (!File.Exists(indexPath))
        {
            CreateIndexFile();
        }
    }

    private static void MoveDocument(string sourceFile, string targetDir)
    {
        string sourcePath = Path.Combine(Application.dataPath, "..", sourceFile);
        string targetPath = Path.Combine(Application.dataPath, "..", "Docs", targetDir, Path.GetFileName(sourceFile));

        if (File.Exists(sourcePath))
        {
            string targetDirectory = Path.GetDirectoryName(targetPath);
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }

            File.Move(sourcePath, targetPath);
            Debug.Log($"Moved {sourceFile} to {targetDir}");
        }
    }

    private static void CreateIndexFile()
    {
        string indexContent = @"# MTG VRChat World ドキュメント

## 1. セットアップガイド

-   [セットアップガイド](setup/SETUP_GUIDE.md)
-   [クイックスタート](setup/QUICKSTART.md)
-   [プロジェクトセットアップ](setup/SETUP_PROJECT.md)
-   [VCC設定](setup/VCC_SETUP.md)
-   [ビルド設定](setup/BUILD_SETTINGS.md)

## 2. 設計ドキュメント

-   [プロジェクト構造](design/PROJECT_STRUCTURE.md)
-   [実装手順](design/IMPLEMENTATION_STEPS.md)
-   [ワールド実装](design/VRCHAT_IMPLEMENTATION.md)
-   [最適化戦略](design/STANDARD_OPTIMIZATION.md)
-   [Legacy/Vintage対応](design/LEGACY_VINTAGE_SUPPORT.md)
-   [同期システム](design/WORLD_SYNC_SYSTEM.md)
-   [シンプル版](design/SIMPLE_VERSION.md)

## 3. ユーザーガイド

-   [ユーザーマニュアル](user/USER_GUIDE.md)
-   [トラブルシューティング](user/TROUBLESHOOTING.md)

## 4. 開発者ガイド

-   [カード管理](dev/CARD_MANAGEMENT.md)
-   [ワールドサイズ](dev/WORLD_SIZE_ESTIMATION.md)
-   [テスト計画](dev/TEST_PLAN.md)
-   [アップロードガイド](dev/UPLOAD_GUIDE.md)

## 5. その他

-   [プロジェクト概要](meta/README.md)
-   [課題と懸念事項](meta/CONCERNS.md)
-   [プロジェクト総括](meta/CONCLUSION.md)
-   [サマリー](meta/SUMMARY.md)";

        string indexPath = Path.Combine(Application.dataPath, "..", "Docs", "INDEX.md");
        File.WriteAllText(indexPath, indexContent);
        Debug.Log("Created new INDEX.md");
    }
}
#endif
