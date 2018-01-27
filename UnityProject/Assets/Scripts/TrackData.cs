using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class TrackData
{
    public List<TrackData> track = new List<TrackData>();

    public TrackType type
    {
        get { return data.type; }
        set { data.type = value; }
    }

    public Orientation o
    {
        get { return data.o; }
        set { data.o = value; }
    }

    public TrackPieceData data;

    public TrackData(Orientation o)
    {
        data = new TrackPieceData();
        data.o = o;
    }

    public string SerializeForNetwork()
    {
        return JsonConvert.SerializeObject(this, Formatting.None);
    }

    public static TrackData DeserializeFromNetwork(string input)
    {
        return JsonConvert.DeserializeObject<TrackData>(input);
    }
}

public enum TrackType
{
    Straight = 0,
    TwoWayJunction = 1,
    ThreeWayJunction = 2,
    Finish = 3,
    DeadEnd = 4,
    Start = 5
}

public enum Orientation
{
    NN = 0,
    NE = 1,
    SE = 2,
    SS = 3,
    SW = 4,
    NW = 5
}