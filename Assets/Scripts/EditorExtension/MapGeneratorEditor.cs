using UnityEditor;
using UnityEngine;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;

[CustomEditor(typeof(MapGenerator))]
public class MazeManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // MapGeneratorコンポーネントを取得
        MapGenerator mapGenerator = (MapGenerator)target;

        // "Load Map Texts"ボタンを作成
        if (GUILayout.Button("Load Map Texts"))
        {
            LoadMapTextFiles(mapGenerator);
        }
    }

    private void LoadMapTextFiles(MapGenerator mapGenerator)
    {
        // Assets/MapTextsフォルダ内の全てのtxtファイルを取得
        string[] filePaths = Directory.GetFiles("Assets/MapTexts", "*.txt");

        // MapTextListをクリア
        mapGenerator.MapTextList.Clear();

        foreach (string filePath in filePaths)
        {
            // パスの区切り文字をスラッシュに統一
            string normalizedPath = filePath.Replace("\\", "/");

            // TextAssetを読み込む
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(normalizedPath);

            // MapTextListに追加
            if (textAsset != null)
            {
                mapGenerator.MapTextList.Add(textAsset);
            }
            else
            {
                UnityEngine.Debug.LogError("TextAsset not found at path: " + normalizedPath);
            }
        }

        // インスペクターの更新を反映
        EditorUtility.SetDirty(mapGenerator);
    }
}
