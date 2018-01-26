using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackController : MonoBehaviour
{
    public int trackLength = 10;

    public float deadEndBreakageProbability = .4f;

    private TrackData track;
    
    void Start()
    {
        track = GenerateTrack(0);

        Debug.Log(track.ToString());
    }

    private TrackData GenerateTrack(int depth, bool broken = false)
    {
        Debug.Log("Gener");

        TrackData generatedTrack = new TrackData();

        if (depth > trackLength)
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
                switch (Random.Range(0, 3))
                {
                    case 0:
                        generatedTrack.type = TrackType.Straight;
                        generatedTrack.track = new List<TrackData>
                        {
                            GenerateTrack(depth + 1)
                        };
                        break;
                    case 1:
                        generatedTrack.type = TrackType.TwoWayJunction;
                        generatedTrack.track = new List<TrackData>
                        {
                            GenerateTrack(depth + 1),
                            GenerateTrack(depth + 1, true)
                        };
                        break;
                    case 2:
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
        var outStr = type + "\n";
        foreach (TrackData trackData in track)
        {
            outStr += trackData.type + " - ";
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