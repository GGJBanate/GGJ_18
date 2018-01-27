using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;

public class TrackData
{
    public List<TrackData> track = new List<TrackData>();

    public TrackType type;

    public Orientation o;

    public TrackData(Orientation o)
    {
        this.o = o;
    }

    /**
     * Format: {type,orientation(left,middle,right)}
     */
    public string SerializeForNetwork()
    {
        string serial = JsonConvert.SerializeObject(this, Formatting.None);
        Debug.Log(serial);
        return serial;
        /*StringBuilder outStr = new StringBuilder("{type:\"" + type + "\",o:\"" + o + "\",track:[");
        foreach (TrackData data in track)
        {
            outStr.Append(data);
            outStr.Append(',');
        }
        outStr.Remove(0, outStr.Length);
        outStr.Append("]}");
        return outStr.ToString();*/
    }

    //private static char[] splitters = {',', '('};

    public static TrackData DeserializeFromNetwork(string input)
    {
        return JsonConvert.DeserializeObject<TrackData>(input);
        /*input = input.Substring(1, input.Length - 2);

        string[] split = input.Split(splitters, 3);

        TrackType type = (TrackType) int.Parse(split[0]);
        Orientation o = (Orientation) int.Parse(split[1]);

        TrackData t = new TrackData(o);
        t.type = type;

        input = split[2].Substring(0, split[2].Length);

        return t;*/
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

public class Pos
{
    public int x = 0;
    public int y = 0;
    public int z = 0;

    public Pos()
    {
    }

    public Pos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Pos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    /**
        orientation of the tile
        direction in witch to go (-1 left, 0 center, +1 right)
    */
    public Pos Go(Orientation o, int dir)
    {
        Pos ret = new Pos(this.x, this.y, this.z);

        switch ((Orientation) ((int) (o + 6 + dir) % 6))
        {
            case Orientation.NN:
                ret.y -= 1;
                break;
            case Orientation.NE:
                ret.x += 1;
                ret.y -= 1;
                break;
            case Orientation.SE:
                ret.x += 1;
                break;
            case Orientation.SS:
                ret.y += 1;
                break;
            case Orientation.SW:
                ret.x -= 1;
                break;
            case Orientation.NW:
                ret.x -= 1;
                ret.y -= 1;
                break;
        }

        return ret;
    }
}