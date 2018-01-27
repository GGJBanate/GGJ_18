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

    public float deadEndBreakageProbability = .8f;

    public Dictionary<Pos, TrackPieceData> map = new Dictionary<Pos, TrackPieceData>(new Pos.EqualityComparer());

    private static TrackServer instance;

    public static TrackServer Instance
    {
        get { return instance; }
    }

    [SyncVar] public string serializedTrack;

    [SyncVar] public string serializedMap;

    private TrackData track;

    public void Awake()
    {
        instance = this;
    }

    public override void OnStartServer()
    {
        track = GenerateTrack(0);
        serializedTrack = track.SerializeForNetwork();

        Debug.Log(serializedTrack.Length);
        Debug.Log(serializedTrack);

        serializedMap = JsonConvert.SerializeObject(map, Formatting.Indented);
    
        Debug.Log(map);
        Debug.Log(serializedMap.Length);
        Debug.Log(serializedMap);
    }

    private TrackData GenerateTrack(int depth)
    {
        TrackData generatedTrack = new TrackData(Orientation.NN);
        generatedTrack.type = TrackType.Start;
        this.GenerateTrackStep(depth, new Pos(), Orientation.NN, false, generatedTrack);

        return generatedTrack;
    }


    private void GenerateTrackStep(int depth, Pos pos, Orientation o, bool broken, TrackData parent)
    {
        TrackData generatedTrack = new TrackData(o);
        parent.track.Add(generatedTrack);


        if (!broken && depth > trackLength)
        {
            generatedTrack.type = TrackType.Finish;
            map.Add(pos, new TrackPieceData(generatedTrack.o, generatedTrack.type));
            return;
        }


        if (broken && Random.value < deadEndBreakageProbability)
        {
            generatedTrack.type = TrackType.DeadEnd;
            map.Add(pos, new TrackPieceData(generatedTrack.o, generatedTrack.type));
            return;
        }


        Pos pL = pos.Go(o, -1);
        Pos pC = pos.Go(o, 0);
        Pos pR = pos.Go(o, +1);

        switch (Random.Range(0, 4))
        {
            case 0:
            case 1:
                if (map.ContainsKey(pL))
                {
                    generatedTrack.type = TrackType.DeadEnd;
                    break;
                }

                generatedTrack.type = TrackType.Straight;
                GenerateTrackStep(depth + 1, pos.Go(o, 0), o, broken, generatedTrack);
                break;


            case 2:
                if (map.ContainsKey(pL) || map.ContainsKey(pR))
                    goto case 1;

                int brokenDir = Random.Range(0, 2);
                generatedTrack.type = TrackType.TwoWayJunction;
                GenerateTrackStep(depth + 1, pL, (Orientation) ((int) (o + 5) % 6), brokenDir == 0 ? true : broken,
                    generatedTrack);
                GenerateTrackStep(depth + 1, pR, (Orientation) ((int) (o + 1) % 6), brokenDir == 1 ? true : broken,
                    generatedTrack);
                break;


            case 3:
                if (map.ContainsKey(pL) || map.ContainsKey(pC) || map.ContainsKey(pR))
                    goto case 2;

                brokenDir = Random.Range(0, 3);
                generatedTrack.type = TrackType.ThreeWayJunction;
                GenerateTrackStep(depth + 1, pL, (Orientation) ((int) (o + 5) % 6), brokenDir == 0 ? true : broken,
                    generatedTrack);
                GenerateTrackStep(depth + 1, pC, o, brokenDir == 1 ? true : broken, generatedTrack);
                GenerateTrackStep(depth + 1, pR, (Orientation) ((int) (o + 1) % 6), brokenDir == 2 ? true : broken,
                    generatedTrack);
                break;
        }

        map.Add(pos, new TrackPieceData(generatedTrack.o, generatedTrack.type));
    }
}

[Serializable]
public struct TrackPieceData
{
    public Orientation o;
    public TrackType type;

    public TrackPieceData(Orientation o, TrackType type)
    {
        this.o = o;
        this.type = type;
    }
}

public class PosConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context,
        Type sourceType)
    {

        if (sourceType == typeof(string))
        {
            return true;
        }
        return base.CanConvertFrom(context, sourceType);
    }
    // Overrides the ConvertFrom method of TypeConverter.
    public override object ConvertFrom(ITypeDescriptorContext context,
        CultureInfo culture, object value)
    {
        if (value is string)
        {
            string[] v = ((string)value).Split(',');
            return new Pos(int.Parse(v[0]), int.Parse(v[1]), int.Parse(v[2]));
        }
        return base.ConvertFrom(context, culture, value);
    }
    // Overrides the ConvertTo method of TypeConverter.
    public override object ConvertTo(ITypeDescriptorContext context,
        CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType == typeof(string))
        {
            return ((Pos)value).x + "," + ((Pos)value).y + "," + ((Pos)value).z;
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
}

[TypeConverter(typeof(PosConverter))]
[Serializable]
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

    public class EqualityComparer : IEqualityComparer<Pos> {

        public bool Equals (Pos a, Pos b) {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public int GetHashCode(Pos a) {
            return 100 + a.x + 100 * (100 + a.y) + 10000 * (100 + a.z);
        }

    }
}