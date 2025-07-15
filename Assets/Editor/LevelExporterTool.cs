#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class LevelExporterTool : EditorWindow
{
    [MenuItem("Tools/CarGame/Export Current Level")]
    static void ExportLevel()
    {
        var blocks = GameObject.FindGameObjectsWithTag("LevelBlock");
        string sceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
        string fileName = $"Level_{sceneName}_Data.txt";
        string filePath = $"Assets/Levels/{fileName}";

        using StreamWriter writer = new StreamWriter(filePath);
        foreach (var block in blocks)
        {
            var levelBlock = block.GetComponent<LevelBlock>();
            string id = levelBlock != null ? levelBlock.ID : block.name;
            Vector3 p = block.transform.position;
            writer.WriteLine($"{id},{p.x},{p.y},{p.z}");
        }

        Debug.Log($"Nivel exportado: {filePath}");
        AssetDatabase.Refresh();
    }
}
#endif
