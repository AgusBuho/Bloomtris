using UnityEngine;
using System.Collections.Generic;

public class LevelStreamer : MonoBehaviour
{
    public LevelSettings levelSettings;
    public Transform player;

    private struct BlockData
    {
        public string id;
        public Vector3 position;
        public GameObject instance;
    }

    private List<BlockData> allBlocks = new();
    private int lastIndexSpawned = -1;

    void Start()
    {
        var lines = levelSettings.levelFile.text.Split('\n');
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] parts = line.Split(',');
            string id = parts[0];
            Vector3 pos = new(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));

            allBlocks.Add(new BlockData { id = id, position = pos, instance = null });
        }
    }

    void Update()
    {
        float spawnZ = player.position.z + levelSettings.spawnDistance;
        float despawnZ = player.position.z - levelSettings.despawnDistance;

        for (int i = 0; i < allBlocks.Count; i++)
        {
            var b = allBlocks[i];

            if (b.instance == null && b.position.z <= spawnZ)
            {
                var prefab = levelSettings.prefabDatabase.GetPrefab(b.id);
                b.instance = GameObject.Instantiate(prefab, b.position, Quaternion.identity);
                allBlocks[i] = b;
            }
            else if (b.instance != null && b.position.z < despawnZ)
            {
                GameObject.Destroy(b.instance);
                b.instance = null;
                allBlocks[i] = b;
            }
        }
    }
}
