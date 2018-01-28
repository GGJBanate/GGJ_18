using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class TrackServer : NetworkBehaviour
{
    public int trackLength = 10;

    public float deadEndBreakageLengthFactorMin = .3f;
    public float deadEndBreakageLengthFactorMax = .7f;

    public Dictionary<Pos, TrackPieceData> map = new Dictionary<Pos, TrackPieceData>(new Pos.EqualityComparer());

    public static TrackServer Instance { get; private set; }

    [SyncVar] public string serializedTrack;

    [SyncVar] public string serializedMap;

    private TrackData track;

    public void Awake()
    {
        Instance = this;
    }

    public override void OnStartServer()
    {
        track = GenerateTrack(0);
        serializedTrack = track.SerializeForNetwork();
        serializedMap = JsonConvert.SerializeObject(map, Formatting.Indented);
    }

    private TrackData GenerateTrack(int depth)
    {
        TrackData generatedTrack = new TrackData(Orientation.NN);
        
        generatedTrack.type = TrackType.Start;
        
        map.Add(new Pos(), new TrackPieceData(generatedTrack.o, generatedTrack.type));
        map.Add(new Pos(0,-1), new TrackPieceData(Orientation.NN));

        this.GenerateTrackStep(depth, new Pos(0,-1), Orientation.NN, false, generatedTrack);
        
        generatedTrack.track[0].data.switchActive = true;
        generatedTrack.data.activeChild = 0;

        return generatedTrack;
    }
    
    private void GenerateTrackStep(int depth, Pos pos, Orientation o, bool broken, TrackData parent)
    {
        TrackData generatedTrack = new TrackData(o);

        TrackPieceData tpd = map[pos];
        tpd.id = map.Count;
        tpd.brokenPath = broken;

        parent.track.Add(generatedTrack);

        if (depth >= (broken ? trackLength * Random.Range(deadEndBreakageLengthFactorMin, deadEndBreakageLengthFactorMax) : trackLength) )
        {
            generatedTrack.type = broken ? TrackType.DeadEnd : TrackType.Finish;
            tpd.type = generatedTrack.type;
            map[pos] = tpd;
            return;
        } 

        Pos pL = pos.Go(o, -1);
        Pos pC = pos.Go(o, 0);
        Pos pR = pos.Go(o, +1);

        Orientation oL = (Orientation) ((int) (o + 5) % 6);
        Orientation oC = o;
        Orientation oR = (Orientation) ((int) (o + 1) % 6);

        switch (RandomTrackType())
        {
            case TrackType.Straight:
                GenerateTrackStepStraight(TrackType.Straight, pC, oC, generatedTrack, broken, depth);
                break;

            case TrackType.Broken:
                GenerateTrackStepStraight(TrackType.Broken, pC, oC, generatedTrack, broken, depth);
                break;

            case TrackType.Danger:
                GenerateTrackStepStraight(TrackType.Danger, pC, oC, generatedTrack, broken, depth);
                break;

            case TrackType.Crossing: //TODO change to another visualization
                GenerateTrackStepStraight(TrackType.Danger, pC, oC, generatedTrack, broken, depth);
                break;

            case TrackType.TwoWayJunction:
                if (map.ContainsKey(pL) || map.ContainsKey(pR))
                    goto case TrackType.Straight;

                generatedTrack.type = TrackType.TwoWayJunction;
                map.Add(pL, new TrackPieceData(oL));
                map.Add(pR, new TrackPieceData(oR));

                switch (Random.Range(0, 2)) {
                    case 0:
                        GenerateTrackStep(depth + 1, pL, oL, broken, generatedTrack);
                        GenerateTrackStep(depth + 1, pR, oR, true, generatedTrack);
                        break;

                    case 1:
                        GenerateTrackStep(depth + 1, pR, oR, broken, generatedTrack);
                        GenerateTrackStep(depth + 1, pL, oL, true, generatedTrack);
                        break;
                }

                int activeChild = Random.Range(0, 2);
                generatedTrack.track[activeChild].data.switchActive = true;
                generatedTrack.data.activeChild = activeChild;
                break;

            case TrackType.ThreeWayJunction:
                if (map.ContainsKey(pL) || map.ContainsKey(pC) || map.ContainsKey(pR))
                    goto case TrackType.TwoWayJunction;

                generatedTrack.type = TrackType.ThreeWayJunction;
                map.Add(pL, new TrackPieceData(oL));
                map.Add(pC, new TrackPieceData(oC));
                map.Add(pR, new TrackPieceData(oR));

                switch (Random.Range(0, 3)) {
                    case 0:
                        GenerateTrackStep(depth + 1, pL, oL, broken, generatedTrack);
                        GenerateTrackStep(depth + 1, pC, oC, true, generatedTrack);
                        GenerateTrackStep(depth + 1, pR, oR, true, generatedTrack);
                        break;

                    case 1:
                        GenerateTrackStep(depth + 1, pC, oC, broken, generatedTrack);
                        GenerateTrackStep(depth + 1, pR, oR, true, generatedTrack);
                        GenerateTrackStep(depth + 1, pL, oL, true, generatedTrack);
                        break;

                    case 2:
                        GenerateTrackStep(depth + 1, pR, oR, broken, generatedTrack);
                        GenerateTrackStep(depth + 1, pC, oC, true, generatedTrack);
                        GenerateTrackStep(depth + 1, pL, oL, true, generatedTrack);
                        break;
                }

                activeChild = Random.Range(0, 3);
                generatedTrack.track[activeChild].data.switchActive = true;
                generatedTrack.data.activeChild = activeChild;
                break;
        }

        tpd.type = generatedTrack.type;
        map[pos] = tpd;
    }

    private void GenerateTrackStepStraight(TrackType type, Pos p, Orientation o, TrackData generatedTrack, bool broken, int depth) {
                if (map.ContainsKey(p))
                {
                    generatedTrack.type = broken ? TrackType.DeadEnd : TrackType.Finish;
                    return;
                }

                generatedTrack.type = type;
                map.Add(p, new TrackPieceData(o));

                GenerateTrackStep(depth + 1, p, o, broken, generatedTrack);

                generatedTrack.track[0].data.switchActive = true;
                generatedTrack.data.activeChild = 0;
                return;
    }

    private TrackType RandomTrackType(int type = -1) {
        switch(type < 0 ? Random.Range(0, 10) : type) {
            case 0:
            case 1:
            case 2:
            case 3:
                return TrackType.Straight;

            case 4:
            case 5:
            case 6:
                return TrackType.TwoWayJunction;

            case 7:
                return TrackType.ThreeWayJunction;

            case 8:
                return TrackType.Broken;

            case 9:
                return TrackType.Danger;

            default:
                return TrackType.Straight;
        }
    }
}


[Serializable]
public struct TrackPieceData
{
    public Orientation o;
    public TrackType type;
    public bool switchActive;
    public int activeChild;
    public int id;
    public bool brokenPath;

    public TrackPieceData(Orientation o, TrackType t = TrackType.Straight)
    {
        this.o = o;
        this.type = t;
        this.switchActive = false;
        this.activeChild = 0;
        this.id = 0;
        this.brokenPath = false;
    }
}