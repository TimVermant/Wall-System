using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSystem : MonoBehaviour
{
    public List<Edge> Walls { get; } = new List<Edge>();
    public List<Corner> Corners { get; } = new List<Corner>();

    public void AddWall(Edge edge)
    {
        if(edge == null)
        {
            return;
        }
        if(!Walls.Contains(edge))
        {
            Walls.Add(edge);
        }
    }
}
