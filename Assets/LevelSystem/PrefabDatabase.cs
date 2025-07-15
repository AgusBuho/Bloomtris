using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "CarGame/Prefab Database")]
public class PrefabDatabase : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public string ID;
        public GameObject prefab;
    }

    public List<Entry> prefabs;

    public GameObject GetPrefab(string id)
    {
        return prefabs.Find(e => e.ID == id)?.prefab;
    }
}
