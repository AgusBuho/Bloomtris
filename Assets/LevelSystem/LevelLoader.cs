using UnityEngine;


[DefaultExecutionOrder(-100)]
public class _LevelLoader : MonoBehaviour
{
    [Header("Level Source")]
    public LevelDatabase levelDatabase;
    public int levelIndex = 0;

    [Header("Player")]
    public Transform player;

    private LevelDefinition levelDefinition;

    void Start()
    {
        if (levelDatabase == null || levelDatabase.levels.Count == 0)
        {
            Debug.LogError("No LevelDatabase assigned or it's empty.");
            return;
        }

        if (levelIndex < 0 || levelIndex >= levelDatabase.levels.Count)
        {
            Debug.LogError("Invalid level index.");
            return;
        }

        levelDefinition = levelDatabase.levels[levelIndex].definition;
        if (levelDefinition == null)
        {
            Debug.LogError("LevelDefinition missing for this entry.");
            return;
        }

        LoadLevel(levelDefinition);
    }

    void LoadLevel(LevelDefinition def)
    {
        ApplyEnvironment(def);
        SpawnItems(def.blocks);
        SpawnItems(def.decorations);
    }

    void ApplyEnvironment(LevelDefinition def)
    {
        var env = def.environment;
        RenderSettings.ambientLight = env.ambientLight;
        if (env.skybox != null) RenderSettings.skybox = env.skybox;
        RenderSettings.fog = env.enableFog;
        RenderSettings.fogMode = env.fogMode;
        RenderSettings.fogColor = env.fogColor;
        RenderSettings.fogStartDistance = env.fogStart;
        RenderSettings.fogEndDistance = env.fogEnd;

        if (env.ambientAudio != null)
        {
            var audio = new GameObject("AmbientAudio");
            var src = audio.AddComponent<AudioSource>();
            src.clip = env.ambientAudio;
            src.loop = true;
            src.playOnAwake = true;
            src.Play();
        }
    }

    void SpawnItems(System.Collections.Generic.List<LevelItem> items)
    {
        foreach (var item in items)
        {
            if (item.prefab == null) continue;
            var go = Instantiate(item.prefab, item.position, Quaternion.Euler(item.rotation));
            go.transform.localScale = item.scale;
        }
    }
}
