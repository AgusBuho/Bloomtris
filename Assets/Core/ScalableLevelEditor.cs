#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using static UnityEditor.PrefabUtility;
#endif
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

// Level Definition ScriptableObject
[CreateAssetMenu(menuName = "CarGame/Level Definition")]
public class LevelDefinition : ScriptableObject
{
    [Header("Blocks (Gameplay)")]
    public List<LevelItem> blocks = new List<LevelItem>();

    [Header("Decorations (Visual)")]
    public List<LevelItem> decorations = new List<LevelItem>();

    [Header("Environment Settings")]
    public EnvironmentSettings environment = new EnvironmentSettings();
}

[Serializable]
public class LevelItem
{
    public string id;
    public GameObject prefab;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale = Vector3.one;
}

[Serializable]
public class EnvironmentSettings
{
    public Color ambientLight = Color.white;
    public Material skybox;
    public bool enableFog = false;
    public FogMode fogMode = FogMode.Linear;
    public Color fogColor = Color.gray;
    public float fogStart = 0f;
    public float fogEnd = 300f;
    public AudioClip ambientAudio;
}

// Runtime loader
public class LevelLoader : MonoBehaviour
{
    [Tooltip("ScriptableObject with the level definition")]
    public LevelDefinition levelDefinition;
    public Transform player;
    private List<GameObject> spawned = new List<GameObject>();

    void Start()
    {
        if (levelDefinition == null)
        {
            Debug.LogError("LevelDefinition not assigned on LevelLoader.");
            return;
        }
        ApplyEnvironment();
        SpawnAll();
    }

    void ApplyEnvironment()
    {
        var env = levelDefinition.environment;
        RenderSettings.ambientLight = env.ambientLight;
        if (env.skybox != null) RenderSettings.skybox = env.skybox;
        RenderSettings.fog = env.enableFog;
        RenderSettings.fogMode = env.fogMode;
        RenderSettings.fogColor = env.fogColor;
        RenderSettings.fogStartDistance = env.fogStart;
        RenderSettings.fogEndDistance = env.fogEnd;
        if (env.ambientAudio != null)
        {
            var go = new GameObject("AmbientAudio");
            var src = go.AddComponent<AudioSource>();
            src.clip = env.ambientAudio;
            src.loop = true;
            src.playOnAwake = true;
            src.Play();
        }
    }

    void SpawnAll()
    {
        foreach (var item in levelDefinition.blocks)
            SpawnItem(item);
        foreach (var item in levelDefinition.decorations)
            SpawnItem(item);
    }

    void SpawnItem(LevelItem item)
    {
        if (item.prefab == null) return;
        var inst = Instantiate(item.prefab, item.position, Quaternion.Euler(item.rotation));
        inst.transform.localScale = item.scale;
        spawned.Add(inst);
    }
}

#if UNITY_EDITOR
// Editor window: Level Definition Editor with Auto-Collect
public class LevelDefinitionWindow : EditorWindow
{
    private LevelDefinition def;
    private SerializedObject serializedDef;
    private ReorderableList blocksList;
    private ReorderableList decoList;

    [MenuItem("Tools/CarGame/Level Definition Editor")]
    public static void ShowWindow() => GetWindow<LevelDefinitionWindow>("Level Def Editor");

    void OnEnable()
    {
        var guids = AssetDatabase.FindAssets("t:LevelDefinition");
        if (guids.Length > 0)
            def = AssetDatabase.LoadAssetAtPath<LevelDefinition>(AssetDatabase.GUIDToAssetPath(guids[0]));
        if (def != null)
        {
            serializedDef = new SerializedObject(def);
            SetupLists();
        }
    }

    void SetupLists()
    {
        serializedDef.Update();
        blocksList = new ReorderableList(serializedDef, serializedDef.FindProperty("blocks"), true, true, true, true)
        {
            drawHeaderCallback = r => EditorGUI.LabelField(r, "Blocks"),
            drawElementCallback = (r, i, a, f) =>
            {
                var e = blocksList.serializedProperty.GetArrayElementAtIndex(i);
                EditorGUI.PropertyField(r, e, true);
            }
        };
        decoList = new ReorderableList(serializedDef, serializedDef.FindProperty("decorations"), true, true, true, true)
        {
            drawHeaderCallback = r => EditorGUI.LabelField(r, "Decorations"),
            drawElementCallback = (r, i, a, f) =>
            {
                var e = decoList.serializedProperty.GetArrayElementAtIndex(i);
                EditorGUI.PropertyField(r, e, true);
            }
        };
        serializedDef.ApplyModifiedProperties();
    }

    void OnGUI()
    {
        if (def == null)
        {
            EditorGUILayout.HelpBox("No LevelDefinition asset found.", MessageType.Warning);
            if (GUILayout.Button("Create New LevelDefinition"))
            {
                def = CreateInstance<LevelDefinition>();
                var path = EditorUtility.SaveFilePanelInProject("Save LevelDefinition", "NewLevelDef", "asset", "");
                if (!string.IsNullOrEmpty(path))
                {
                    AssetDatabase.CreateAsset(def, path);
                    AssetDatabase.SaveAssets();
                    serializedDef = new SerializedObject(def);
                    SetupLists();
                }
            }
            return;
        }

        serializedDef.Update();
        EditorGUILayout.LabelField("Editing: " + def.name, EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("Auto-Collect From Scene (LevelBlock tag)"))
            AutoCollect("LevelBlock", def.blocks);
        blocksList.DoLayoutList();

        EditorGUILayout.Space();
        if (GUILayout.Button("Auto-Collect From Scene (Decoration tag)"))
            AutoCollect("Decoration", def.decorations);
        decoList.DoLayoutList();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Environment Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedDef.FindProperty("environment"), true);
        serializedDef.ApplyModifiedProperties();

        EditorGUILayout.Space();
        if (GUILayout.Button("Export to JSON"))
        {
            var json = JsonUtility.ToJson(def, true);
            var folder = "Assets/Levels";
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            var filePath = Path.Combine(folder, def.name + ".json");
            File.WriteAllText(filePath, json);
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Exported", "Level JSON saved to " + filePath, "OK");
        }
    }

    void AutoCollect(string tag, List<LevelItem> list)
    {
        serializedDef.Update();
        list.Clear();
        var objs = GameObject.FindGameObjectsWithTag(tag);
        foreach (var go in objs)
        {
            var prefab = PrefabUtility.GetCorrespondingObjectFromSource(go) as GameObject;
            var id = go.name;
            list.Add(new LevelItem {
                id = id,
                prefab = prefab ?? go,
                position = go.transform.position,
                rotation = go.transform.eulerAngles,
                scale = go.transform.localScale
            });
        }
        serializedDef.ApplyModifiedProperties();
        EditorUtility.SetDirty(def);
    }
}
#endif
