using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSystem : MonoBehaviour
{
    public List<Edge> Walls { get; } = new List<Edge>();


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

    public bool HasAdjacentWall(Corner corner)
    {
        int count = 0;
        
        foreach(Edge edge in corner.EdgeNeighbours)
        {
            if(Walls.Find(wallEdge => wallEdge.EdgePosition == edge.EdgePosition) != null)
            {
                count++;
            }
        }
        // Needs more then 1 adjacent wall to place the pillar(s)
        return count > 1;
    }
}
