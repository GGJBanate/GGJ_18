using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class TrackController : MonoBehaviour
{
    public List<TrackPiece> trackPiecePrefabs;

    [HideInInspector]
    public TrackData track;

    public static TrackController Instance { get; private set; }

    public CartPlayerController playerPrefab;

    void Awake()
    {
        Instance = this;
    }

    public void BuildTrack()
    {
        TrackPiece piece = BuildPiece(track, transform);
        piece.SpawnNextPieces();
    }

    public TrackPiece BuildPiece(TrackData trackData, Transform baseTransform)
    {
        TrackPiece trackPiecePrefab = trackPiecePrefabs.First(t => t.type == trackData.type);
        TrackPiece piece = Instantiate(trackPiecePrefab, baseTransform.position, baseTransform.rotation, transform);
        piece.pieceData = trackData;

        return piece;
    }

    public void SetSwitchState(int trackPieceId, int switchDirection)
    {
        SetSwitchState(track, trackPieceId, switchDirection);
    }

    private void SetSwitchState(TrackData trackPiece, int trackPieceId, int switchDirection)
    {
        if (trackPiece.data.id == trackPieceId)
        {
            trackPiece.data.activeChild = switchDirection;
            for (int index = 0; index < trackPiece.track.Count; index++)
            {
                TrackData child = trackPiece.track[index];
                child.data.switchActive = index == switchDirection;
            }
        }
        else
        {
            foreach (TrackData child in trackPiece.track)
            {
                SetSwitchState(child, trackPieceId, switchDirection);
            }
        }

        var trackPieces = FindObjectsOfType<TrackPiece>();

        foreach (TrackPiece piece in trackPieces)
        {
            if (piece.pieceData.data.id == trackPieceId)
            {
                var target = piece.pieceData.track[switchDirection].data.id;

                foreach (TrackPiece p in trackPieces)
                {
                    if (p.pieceData.data.id == target)
                    {
                        p.entryCollider.active = true;
                        break;
                    }
                }
                break;
            }
        }
    }
}