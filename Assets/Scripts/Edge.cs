using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{ 

      

    public Vector3 EdgePosition { get; set; }
    public GameObject EdgeBuilding { get; set; }

    public List<Corner> Corners { get; set; } = new List<Corner>();

    
    public void InitializeEdge()
    {

    }

}
