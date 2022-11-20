using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corner 
{
    public List<Edge> EdgeNeighbours { get; set; } = new List<Edge>();
    public Vector3 Position { get; set; }
    public GameObject CornerObject { get; set; }
}
