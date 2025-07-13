using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PieceSpawner
{
    private readonly PieceSetConfig pieceSet;
    private readonly Transform parent;

    public PieceSpawner(PieceSetConfig set, Transform parentTransform)
    {
        pieceSet = set;
        parent = parentTransform;
    }

    public void SpawnNext()
    {
        if (pieceSet.piecePrefabs.Count == 0) return;

        int index = Random.Range(0, pieceSet.piecePrefabs.Count);
        var reference = pieceSet.piecePrefabs[index];

        reference.InstantiateAsync(parent).Completed += OnPieceLoaded;
    }

    private void OnPieceLoaded(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject piece = handle.Result;
            // Configuraciones extra si hace falta
        }
    }
}