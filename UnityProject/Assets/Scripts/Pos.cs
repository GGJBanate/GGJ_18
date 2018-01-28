using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;


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

    public override String ToString()
    {
        return "(" + x + " " + y + " " + z + ")";
    }

    public class EqualityComparer : IEqualityComparer<Pos>
    {
        public bool Equals(Pos a, Pos b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public int GetHashCode(Pos a)
        {
            return 10000 * (100 + a.x) + 100 * (100 + a.y) + a.z;
        }
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
            string[] v = ((string) value).Split(',');
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
            return ((Pos) value).x + "," + ((Pos) value).y + "," + ((Pos) value).z;
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}