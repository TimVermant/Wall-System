using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corner 
{
    public List<Edge> EdgeNeighbours { get; } = new List<Edge>();
    public Vector3 Position { get; set; }
}
