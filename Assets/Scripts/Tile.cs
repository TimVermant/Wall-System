using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public enum EdgeDirection
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3,
    }

    public Vector3 Position { get; set; }
    //public GameObject TileObject { get; set; }
    public GameObject Building { get; set; }
    public List<Edge> Edges { get; set; } = new List<Edge>();
    public List<Corner> Corners { get; set; } = new List<Corner> { null, null, null, null };


}
