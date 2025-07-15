using UnityEditor;
using UnityEngine;

public class LevelSelectorWindow : EditorWindow
{
    private LevelDatabase levelDatabase;
    private Vector2 scroll;
    private GUIStyle headerStyle;

    [MenuItem("Tools/CarGame/Level Selector")]
    public static void ShowWindow()
    {
        var window = GetWindow<LevelSelectorWindow>(false, "Level Selector", true);
        window.minSize = new Vector2(420, 320);
    }

    private void OnEnable()
    {
        headerStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 14 };
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        levelDatabase = (LevelDatabase)EditorGUILayout.ObjectField("Level Database", levelDatabase, typeof(LevelDatabase), false);

        if (levelDatabase == null || levelDatabase.levels.Count == 0)
        {
            EditorGUILayout.HelpBox("Assign a valid LevelDatabase.", MessageType.Info);
            return;
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);

        for (int i = 0; i < levelDatabase.levels.Count; i++)
        {
            var level = levelDatabase.levels[i];
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Level " + i + ": " + level.levelName, headerStyle);
            EditorGUILayout.ObjectField("Scene", level.scene, typeof(SceneAsset), false);
            EditorGUILayout.ObjectField("Definition", level.definition, typeof(LevelDefinition), false);

            EditorGUILayout.Space();
            if (GUILayout.Button("Set As Active Level"))
            {
                // Encuentra tu _LevelLoader y actualiza
                _LevelLoader existing = FindObjectOfType<_LevelLoader>();
                if (existing != null)
                {
                    Undo.RecordObject(existing, "Change Active Level");
                    existing.levelIndex = i;
                    existing.levelDatabase = levelDatabase;
                    EditorUtility.SetDirty(existing);
                    Debug.Log($"LevelLoader actualizado con el nivel {level.levelName} (Index {i})");
                }
                else
                {
                    Debug.LogWarning("No _LevelLoader encontrado en la escena.");
                }
            }

            if (GUILayout.Button("Edit Level Definition"))
            {
                Selection.activeObject = level.definition;
                EditorGUIUtility.PingObject(level.definition);
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        if (GUILayout.Button("➕ Create New Level"))
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Save New LevelDefinition", "NewLevel", "asset", "Choose location for new level definition");
            if (!string.IsNullOrEmpty(path))
            {
                var newDef = ScriptableObject.CreateInstance<LevelDefinition>();
                AssetDatabase.CreateAsset(newDef, path);
                AssetDatabase.SaveAssets();

                levelDatabase.levels.Add(new LevelDatabase.LevelEntry
                {
                    levelName = "New Level",
                    definition = newDef
                });

                EditorUtility.SetDirty(levelDatabase);
                Debug.Log("Nuevo nivel creado y agregado a la base de datos.");
            }
        }
    }
}
