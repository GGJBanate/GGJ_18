using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPiece : MonoBehaviour
{
    public static float pieceLength = 10;
    public static float pieceHeight = 1;

    public Vector3 EndPos {
        get { return transform.position + new Vector3(0, pieceHeight / 2, pieceLength / 2); }
    }

    public List<GameObject> nextPiecePositions;

    [HideInInspector]
    public TrackData pieceData;

    public TrackType type = TrackType.Straight;

    public Transform spawnPos;

    private bool nextPiecesSpawned;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        DrawArrow.ForGizmo(EndPos, transform.forward);
    }

    public void Start()
    {
        if (type == TrackType.Start)
        {
            CartPlayerController player = Instantiate(TrackController.Instance.playerPrefab, spawnPos.position, transform.rotation);
            player.currentTrack = this;
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
            TrackController.Instance.BuildPiece(data, nextPiecePositions[index].transform);
        }

        nextPiecesSpawned = true;
    }
}
