﻿using System;
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

    public float deadEndBreakageLengthFactor = .7f;

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
        // if(map.ContainsKey(pos)) return;

        TrackData generatedTrack = new TrackData(o);
        TrackPieceData tpd = map[pos];
        tpd.id = map.Count;
        parent.track.Add(generatedTrack);

        if (depth >= (broken ? trackLength * deadEndBreakageLengthFactor : trackLength) )
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
                if (map.ContainsKey(pC))
                {
                    generatedTrack.type = broken ? TrackType.DeadEnd : TrackType.Finish;
                    break;
                }

                generatedTrack.type = TrackType.Straight;
                map.Add(pC, new TrackPieceData(oC));

                GenerateTrackStep(depth + 1, pC, oC, broken, generatedTrack);

                generatedTrack.track[0].data.switchActive = true;
                generatedTrack.data.activeChild = 0;
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

    private TrackType RandomTrackType(int type = -1) {
        switch(type < 0 ? Random.Range(0, 8) : type) {
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

    public TrackPieceData(Orientation o, TrackType t = TrackType.Straight)
    {
        this.o = o;
        this.type = t;
        this.switchActive = false;
        this.activeChild = 0;
        this.id = -1;
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
                ret.y -= (this.x + 100) % 2 == 0 ? 0 : 1;
                ret.x += 1;
                break;
            case Orientation.SE:
                ret.y += (this.x + 100) % 2 == 0 ? 1 : 0;
                ret.x += 1;
                break;
            case Orientation.SS:
                ret.y += 1;
                break;
            case Orientation.SW:
                ret.y += (this.x + 100) % 2 == 0 ? 1 : 0;
                ret.x -= 1;
                break;
            case Orientation.NW:
                ret.y -= (this.x + 100) % 2 == 0 ? 0 : 1;
                ret.x -= 1;
                break;
        }

        return ret;
    }

    public String ToString() {
        return "(" + x + " " + y + " " + z + ")";
    }

    public class EqualityComparer : IEqualityComparer<Pos> {

        public bool Equals (Pos a, Pos b) {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public int GetHashCode (Pos a) {
            return 10000 * (100 + a.x) + 100 * (100 + a.y) + a.z;
        }

    }
}