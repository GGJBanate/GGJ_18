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
        track = GenerateTrack(0);
    }

    private TrackData GenerateTrack(int depth, bool broken = false)
    {
        TrackData generatedTrack = new TrackData();

        if (!broken && depth > trackLength)
        {
            generatedTrack.type = TrackType.Finish;
            generatedTrack.track = new List<TrackData>();
        }
        else
        {
            if (broken && Random.value < deadEndBreakageProbability)
            {
                generatedTrack.type = TrackType.DeadEnd;
                generatedTrack.track = new List<TrackData>();
            }
            else
            {
                switch (Random.Range(0, 4))
                {
                    case 0:
                    case 1:
                        generatedTrack.type = TrackType.Straight;
                        generatedTrack.track = new List<TrackData>
                        {
                            GenerateTrack(depth + 1)
                        };
                        break;
                    case 2:
                        generatedTrack.type = TrackType.TwoWayJunction;
                        generatedTrack.track = new List<TrackData>
                        {
                            GenerateTrack(depth + 1),
                            GenerateTrack(depth + 1, true)
                        };
                        break;
                    case 3:
                        generatedTrack.type = TrackType.ThreeWayJunction;
                        generatedTrack.track = new List<TrackData>
                        {
                            GenerateTrack(depth + 1),
                            GenerateTrack(depth + 1, true),
                            GenerateTrack(depth + 1, true)
                        };
                        break;
                }
            }
        }

        generatedTrack.track.Shuffle();
        return generatedTrack;
    }
}

public class TrackData
{
    public List<TrackData> track;

    public TrackType type;

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