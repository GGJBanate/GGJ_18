using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrackController : MonoBehaviour
{
    public int trackLength = 5;

    public float deadEndBreakageProbability = .8f;

    private TrackData track;

    void Start()
    {
        track = GenerateTrack(0, new Pos(), Orientation.NN);
    }

    private TrackData GenerateTrack(int depth, Pos pos, Orientation o, bool broken = false)
    {
        TrackData generatedTrack = new TrackData(pos, o);


        if (!broken && depth > trackLength)
        {
            generatedTrack.type = TrackType.Finish;
            return generatedTrack;
        }



        if (broken && Random.value < deadEndBreakageProbability)
        {
            generatedTrack.type = TrackType.DeadEnd;
            return generatedTrack;
        }


        switch (Random.Range(0, 4))
        {
            case 0:
            case 1:
                generatedTrack.type = TrackType.Straight;
                generatedTrack.track.Add( GenerateTrack(depth + 1, pos.Go(o, 0), o, broken) );
                break;
            case 2:
                int brokenDir = Random.Range(0, 2);
                generatedTrack.type = TrackType.TwoWayJunction;
                generatedTrack.track.Add( GenerateTrack(depth + 1, pos.Go(o, -1), (Orientation) (o + 5 % 6), brokenDir == 0 ? true : broken) );
                generatedTrack.track.Add( GenerateTrack(depth + 1, pos.Go(o, +1), (Orientation) (o + 1 % 6), brokenDir == 1 ? true : broken) );
                break;
            case 3:
                brokenDir = Random.Range(0, 3);
                generatedTrack.type = TrackType.ThreeWayJunction;
                generatedTrack.track.Add( GenerateTrack(depth + 1, pos.Go(o, -1), (Orientation) (o + 5 % 6), brokenDir == 0 ? true : broken) );
                generatedTrack.track.Add( GenerateTrack(depth + 1, pos.Go(o,  0), o,                         brokenDir == 1 ? true : broken) );
                generatedTrack.track.Add( GenerateTrack(depth + 1, pos.Go(o, +1), (Orientation) (o + 1 % 6), brokenDir == 2 ? true : broken) );
                break;
        }

        //generatedTrack.track.Shuffle();
        return generatedTrack;
    }
}

public class TrackData
{
    public List<TrackData> track = new List<TrackData>();

    public TrackType type;

    public Pos pos;

    public Orientation o;

    public TrackData(Pos pos, Orientation o) {
        this.o = o;
        this.pos = pos;
    }

    public override string ToString()
    {
        var outStr = type.ToString();

        if (track.Count > 0)
        {
            outStr += "(";

            foreach (TrackData trackData in track)
            {
                outStr += trackData + ", ";
            }

            outStr = outStr.Substring(0, outStr.Length - 2) + ")";
        }

        return outStr;
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