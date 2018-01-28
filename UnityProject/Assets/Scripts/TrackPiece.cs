using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrackPiece : MonoBehaviour
{
    public static float pieceLength = 32;
    public static float pieceHeight = 1;
    public static float brokenPieceLength = 35;
    public static float deadEndPieceLength = 0;//16;
    public Transform barrier;

    private float getPieceLength(){
        switch(type){
            case TrackType.Broken: return brokenPieceLength;
            case TrackType.DeadEnd: return deadEndPieceLength;
            default: return pieceLength;
        }
    }

    public Vector3 EndPos
    {
        get
        {
            var pos = new Vector3(0, pieceHeight / 2, getPieceLength() / 2);
            return transform.TransformPoint(pos);
        }
    }

    public List<GameObject> nextPiecePositions;

    [HideInInspector]
    public List<TrackPiece> nextPieces = new List<TrackPiece>();

    [HideInInspector] public TrackData pieceData;

    public TrackType type = TrackType.Straight;

    public Transform spawnPos;

    private bool nextPiecesSpawned;

    public TrackEntryCollider entryCollider;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (TrackController.Instance != null)
        {
            Vector3 tmp = TrackController.Instance.transform.forward;
            Vector3 dir = new Vector3(tmp.x, tmp.y, tmp.z);

            Quaternion rot = Quaternion.AngleAxis(((int) pieceData.o) * 60, Vector3.up);

            DrawArrow.ForGizmo(EndPos, rot * dir);
        }
        else
        {
            DrawArrow.ForGizmo(EndPos, transform.forward);
        }
    }

    public void Awake()
    {
        entryCollider = GetComponentsInChildren<TrackEntryCollider>().First();
        entryCollider.track = this;
    }

    public void Start()
    {
        if (type == TrackType.Start)
        {
            CartPlayerController player = Instantiate(TrackController.Instance.playerPrefab, spawnPos.position,
                transform.rotation);
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
            TrackPiece p = TrackController.Instance.BuildPiece(data, nextPiecePositions[index].transform);

            p.entryCollider.active = data.data.switchActive;

            nextPieces.Add(p);

            if (GameServer.Instance.DebugMode)
                p.SpawnNextPieces();
        }

        nextPiecesSpawned = true;
    }

    void Update(){
        if(type != TrackType.Crossing){
            return;
        }
        GameServer gs = GameServer.Instance;
        float speed = 0.1F;
        if(gs.crossesAreOpen){            
            barrier.localRotation = Quaternion.Lerp(barrier.localRotation, Quaternion.Euler(0,0,90), Time.time * speed);
        }else{
            barrier.localRotation = Quaternion.Lerp(barrier.localRotation, Quaternion.Euler(0,0,0), Time.time * speed);
        }
    }
}