using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI 
{
    public enum Type { HQ, Hotspot}
    public Type MyType;
    public Tile Center;
    public int Radius;

    public POI(Type type, Tile tile, int radius)
    {
        this.Center = tile;
        this.Radius = radius;
        this.MyType = type;
    }
}
