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

    public bool debug = false;

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
}