using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPiece : MonoBehaviour
{

    public List<GameObject> nextPiecePositions;

    [HideInInspector]
    public TrackData pieceData;

    public TrackType type = TrackType.Straight;

    public Transform spawnPos;

    private bool nextPiecesSpawned;

    public void Start()
    {
        if (this.type == TrackType.Start)
        {
            Instantiate(TrackController.Instance.playerPrefab, spawnPos.position, transform.rotation);
        }
    }

    [ContextMenu("SpawnNext")]
    public void SpawnNextPieces()
    {
        if (nextPiecesSpawned)
        {
            Debug.Log("Tried to spawn pieces that were already there");
            return;
        }

        for (int index = 0; index < pieceData.track.Count; index++)
        {
            TrackData data = pieceData.track[index];
            Debug.Log(data.o);
            TrackPiece p = TrackController.Instance.BuildPiece(data, nextPiecePositions[index].transform);

            if(TrackController.Instance.debug) {
                p.SpawnNextPieces();
            }
        }

        nextPiecesSpawned = true;
    }
}
