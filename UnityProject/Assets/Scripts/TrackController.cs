using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrackController : MonoBehaviour
{
    public int trackLength = 5;

    public float deadEndBreakageProbability = .8f;

    private TrackData track;

    public Dictionary<Pos, TrackData> map = new Dictionary<Pos, TrackData>();

    void Start()
    {
        track = GenerateTrack(trackLength);
    }

    private TrackData GenerateTrack(int depth) {
        TrackData generatedTrack = new TrackData(Orientation.NN);
        generatedTrack.type = TrackType.Straight;
        this.GenerateTrackStep(depth, new Pos(), Orientation.NN, false, generatedTrack);

        return generatedTrack;
    }

    private void GenerateTrackStep(int depth, Pos pos, Orientation o, bool broken, TrackData parent)
    {
        TrackData generatedTrack = new TrackData(o);
        map.Add(pos, generatedTrack);
        parent.track.Add(generatedTrack);


        if (!broken && depth > trackLength)
        {
            generatedTrack.type = TrackType.Finish;
            return;
        }



        if (broken && Random.value < deadEndBreakageProbability)
        {
            generatedTrack.type = TrackType.DeadEnd;
            return;
        }


        Pos pL = pos.Go(o, -1);
        Pos pC = pos.Go(o,  0);
        Pos pR = pos.Go(o, +1);

        switch (Random.Range(0, 4))
        {
            case 0:
            case 1:
                if(map.ContainsKey(pL) ) {
                    generatedTrack.type = TrackType.DeadEnd;
                    break;
                }

                generatedTrack.type = TrackType.Straight;
                GenerateTrackStep(depth + 1, pos.Go(o, 0), o, broken, generatedTrack);
                break;


            case 2:
                if(map.ContainsKey(pL) || map.ContainsKey(pR) )
                    goto case 1;

                int brokenDir = Random.Range(0, 2);
                generatedTrack.type = TrackType.TwoWayJunction;
                GenerateTrackStep(depth + 1, pL, (Orientation) (o + 5 % 6), brokenDir == 0 ? true : broken, generatedTrack);
                GenerateTrackStep(depth + 1, pR, (Orientation) (o + 1 % 6), brokenDir == 1 ? true : broken, generatedTrack);
                break;


            case 3:
                if(map.ContainsKey(pL) || map.ContainsKey(pC) || map.ContainsKey(pR) )
                    goto case 2;

                brokenDir = Random.Range(0, 3); 
                generatedTrack.type = TrackType.ThreeWayJunction;
                GenerateTrackStep(depth + 1, pL, (Orientation) (o + 5 % 6), brokenDir == 0 ? true : broken, generatedTrack);
                GenerateTrackStep(depth + 1, pC, o,                         brokenDir == 1 ? true : broken, generatedTrack);
                GenerateTrackStep(depth + 1, pR, (Orientation) (o + 1 % 6), brokenDir == 2 ? true : broken, generatedTrack);
                break;
        }

    }
}

public class TrackData
{
    public List<TrackData> track = new List<TrackData>();

    public TrackType type;

    public Orientation o;

    public TrackData(Orientation o) {
        this.o = o;
    }
}

public enum TrackType
{
    Straight,
    TwoWayJunction,
    ThreeWayJunction,
    Finish,
    DeadEnd
}

public enum Orientation
{
    NN,
    NE,
    SE,
    SS,
    SW,
    NW
}

public class Pos {
    public int x = 0;
    public int y = 0;
    public int z = 0;

    public Pos() {}

    public Pos(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Pos(int x, int y) {
        this.x = x;
        this.y = y;
    }

    /**
        orientation of the tile
        direction in witch to go (-1 left, 0 center, +1 right)
    */
    public Pos Go (Orientation o, int dir) {
        Pos ret = new Pos(this.x, this.y, this.z);

        switch((Orientation)o + dir % 6) {
            case Orientation.NN: 
                ret.y-= 1;
                break;
            case Orientation.NE:
                ret.x+= 1;
                ret.y-= 1;
                break;
            case Orientation.SE:
                ret.x+= 1;
                break;
            case Orientation.SS:
                ret.y+= 1;
                break;
            case Orientation.SW:
                ret.x-= 1;
                break;
            case Orientation.NW:
                ret.x-= 1;
                ret.y-= 1;
                break;
        }

        return ret;
    }
}