#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;

[ExecuteInEditMode]
public class LevelBlock : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private string uniqueID;

    public string ID => uniqueID;

    void OnEnable()
    {
        #if UNITY_EDITOR
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this); // marca como modificado
        }
        #endif
    }
}
