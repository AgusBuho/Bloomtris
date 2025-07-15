using UnityEngine;

[CreateAssetMenu(menuName = "Auto Runner/Auto Config")]
public class AutoConfig : ScriptableObject
{
    public float maxSpeed;
    public float acceleration;
    public float turnStrength;
    public float driftFactor;
}