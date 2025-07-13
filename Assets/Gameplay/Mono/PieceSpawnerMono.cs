using UnityEngine;

public class PieceSpawnerMono : MonoBehaviour, IPieceSpawner
{
    [SerializeField] private PieceSetConfig pieceSet;
    [SerializeField] private Transform spawnPoint;

    private PieceSpawner spawner;

    private void Awake()
    {
        spawner = new PieceSpawner(pieceSet, spawnPoint);
    }

    public void SpawnNextPiece()
    {
        spawner.SpawnNext();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(spawnPoint.position, 0.2f);
        }
    }
#endif
}