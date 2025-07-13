using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Bloomtris/Piece Set Config")]
public class PieceSetConfig : ScriptableObject
{
    public List<AssetReferenceGameObject> piecePrefabs;
}