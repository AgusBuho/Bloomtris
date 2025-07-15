using UnityEngine;

[CreateAssetMenu(menuName = "CarGame/Level Settings")]
public class LevelSettings : ScriptableObject
{
    [Header("Drift Advanced")]
    public float driftTriggerAngle = 15f;    // Ángulo a partir del cual se activa sobreviraje
    public float driftOversteer     = 50f;   // Torque extra al derrapar

    [Header("General")]
    public float speed = 20f;
    public float acceleration = 15f;
    public float maxTurnSpeed = 100f;

    [Header("Drift")]
    public float driftFactor = 0.95f;
    public float gripRecovery = 2f;

    [Header("Spawn")]
    public float spawnDistance = 100f;
    public float despawnDistance = 30f;

    [Header("Level File")]
    public TextAsset levelFile;

    [Header("Prefab Database")]
    public PrefabDatabase prefabDatabase;
}
