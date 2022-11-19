using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{

    public enum EdgeOrientation
    {
        Upwards,
        Sideways
    }


    public Vector3 EdgePosition { get; set; }
    public Vector3 EdgeDirection { get; set; }
    public EdgeOrientation CurrentEdgeOrientation { get; set; }

    public GameObject EdgeBuilding { get; set; }
    public List<Tile> AdjacentTiles { get; set; } = new List<Tile> { null,null};
    public List<Corner> Corners { get; set; } = new List<Corner>();





}
