﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPiece : MonoBehaviour
{
    public static float pieceLength = 10;
    public static float pieceHeight = 1;

    public Vector3 EndPos {
        get
        {
            var pos = new Vector3(0, pieceHeight / 2, pieceLength / 2);
            return transform.TransformPoint(pos);
        }
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
        
        Vector3 tmp = TrackController.Instance.transform.forward;
        Vector3 dir = new Vector3(tmp.x, tmp.y, tmp.z);

        Quaternion rot = Quaternion.AngleAxis( ((int)pieceData.o) * 60, Vector3.up);

        DrawArrow.ForGizmo(EndPos, rot * dir);
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
            Debug.Log("Orientation: " + data.o);
            TrackPiece p = TrackController.Instance.BuildPiece(data, nextPiecePositions[index].transform);

            if(TrackController.Instance.debug) {
                p.SpawnNextPieces();
            }
        }

        nextPiecesSpawned = true;
    }
}
