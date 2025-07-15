using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor; 
#endif

[CreateAssetMenu(menuName = "CarGame/Level Database")]
public class LevelDatabase : ScriptableObject
{
    [Serializable]
    public class LevelEntry
    {
        public string levelName;
        public SceneAsset scene;
        public LevelDefinition definition;   // ← lo que faltaba
    }

    public List<LevelEntry> levels = new List<LevelEntry>();
}
