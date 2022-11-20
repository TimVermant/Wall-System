using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public List<Tile> Tiles { get; private set; } = new List<Tile>();


    [SerializeField] private GameObject _gridObject;
    [SerializeField] private WallSystem _wallSystem;
    [SerializeField] private GameObject _tile;
    [SerializeField] private GameObject _wall;

    private float _tileSize = 2.5f;


    [SerializeField] private int _rowSize = 5;
    [SerializeField] private int _columnSize = 5;
    private int _gridSize = 5;


    public void GetBuildInfo(Vector3 position, out Tile tile, out Edge edge)
    {
        tile = GetNearestTile(position);
        edge = GetNearestTileEdge(position, tile);

        _tileSize = _tile.transform.localScale.x * 10.0f;

    }


    private void Awake()
    {
        _gridSize = _rowSize * _columnSize;
        InitializeGrid();
        InitializeEdges();
        InitializeCorners();
        //foreach(Tile tile in Tiles)
        //{
        //    foreach (Edge edge in tile.Edges)
        //    {
        //        foreach(Corner corner in edge.Corners)
        //        {
        //            Debug.Log(corner.EdgeNeighbours.Count);
        //        }
        //    }
        //}
    }

    private void InitializeGrid()
    {


        Vector3 startPos = Vector3.zero;
        startPos.x = -(_rowSize / 2.0f) * _tileSize;
        startPos.z = -(_columnSize / 2.0f) * _tileSize;
        Vector3 currentPos = startPos;


        for (int column = 0; column < _columnSize; column++)
        {
            currentPos.x = startPos.x;
            for (int row = 0; row < _rowSize; row++)
            {
                // Initialize tile
                Tile tile = new();

                tile.Position = currentPos;

                Instantiate(_tile, tile.Position, Quaternion.identity, _gridObject.transform);

                Tiles.Add(tile);

                currentPos.x += _tileSize;
            }
            currentPos.z += _tileSize;
        }

    }

    private void InitializeEdges()
    {
        for (int i = 0; i < Tiles.Count; i++)
        {
            int column = i / _columnSize;
            int row = i % _rowSize;

            Tiles[i].Edges = FindEdges(column, row);

            foreach (Edge edge in Tiles[i].Edges)
            {
                if (edge != null)
                {
                    GenerateCorners(edge);
                }
            }
            AddTileCorners(Tiles[i]);
        }
    }

    private void InitializeCorners()
    {
        for (int i = 0; i < Tiles.Count; i++)
        {
            int column = i / _columnSize;
            int row = i % _rowSize;
            CombineCorners(column, row);

        }
    }


    private void AddEdgeToTile(Tile.EdgeDirection direction, Tile currentTile, Tile neighbouringTile, List<Edge> edgeList)
    {
        Edge.EdgeOrientation orientation = Edge.EdgeOrientation.Upwards;
        List<Tile> tiles = new List<Tile> { null, null };

        Vector3 edgePosition = currentTile.Position;
        switch (direction)
        {
            case Tile.EdgeDirection.North:
                edgePosition.z += _tileSize * 0.5f;
                tiles[0] = currentTile;
                tiles[1] = neighbouringTile;
                orientation = Edge.EdgeOrientation.Upwards;
                break;
            case Tile.EdgeDirection.South:
                edgePosition.z -= _tileSize * 0.5f;
                tiles[1] = currentTile;
                tiles[0] = neighbouringTile;
                orientation = Edge.EdgeOrientation.Upwards;
                break;
            case Tile.EdgeDirection.West:
                edgePosition.x -= _tileSize * 0.5f;
                tiles[1] = currentTile;
                tiles[0] = neighbouringTile;
                orientation = Edge.EdgeOrientation.Sideways;
                break;
            case Tile.EdgeDirection.East:
                edgePosition.x -= _tileSize * 0.5f;
                tiles[0] = currentTile;
                tiles[1] = neighbouringTile;
                orientation = Edge.EdgeOrientation.Sideways;
                break;
        }

        Edge newEdge = new();
        newEdge.AdjacentTiles = tiles;
        newEdge.CurrentEdgeOrientation = orientation;
        edgeList[(int)direction] = newEdge;
        if (neighbouringTile == null || neighbouringTile.Edges.Count == 0)
        {
            newEdge.EdgePosition = edgePosition;


        }
        else
        {
            Vector3 midPoint = GetMidpoint(currentTile.Position, neighbouringTile.Position);
            Edge neighboursEdge = neighbouringTile.Edges[(int)InverseDirection(direction)];
            if (neighboursEdge == null)
            {

                newEdge.EdgePosition = midPoint;
            }
            else
            {
                edgeList[(int)direction] = neighboursEdge;
            }
        }
    }



    private void GenerateCorners(Edge edge)
    {
        Corner cornerA = new();
        Corner cornerB = new();


        Vector3 edgePosA = edge.EdgePosition;
        Vector3 edgePosB = edge.EdgePosition;

        switch (edge.CurrentEdgeOrientation)
        {
            case Edge.EdgeOrientation.Upwards:
                edgePosA.x -= _tileSize * 0.5f;
                edgePosB.x += _tileSize * 0.5f;
                break;
            case Edge.EdgeOrientation.Sideways:
                edgePosA.z -= _tileSize * 0.5f;
                edgePosB.z += _tileSize * 0.5f;
                break;
        }

        cornerA.Position = edgePosA;
        cornerB.Position = edgePosB;

        cornerA.EdgeNeighbours.Add(edge);
        cornerB.EdgeNeighbours.Add(edge);

        edge.Corners[0] = cornerA;
        edge.Corners[1] = cornerB;


    }

    private void AddTileCorners(Tile tile)
    {
        List<Corner> cornerList = new List<Corner>();
        List<Corner> commonCornerList = new List<Corner>();

        for (int i = 0; i < tile.Edges.Count; i++)
        {
            foreach (Corner corner in tile.Edges[i].Corners)
            {
                if (cornerList.Find(cor => corner.Position == cor.Position) == null)
                {

                    cornerList.Add(corner);
                }

            }


        }

        for (int i = 0; i < cornerList.Count; i++)
        {
            int addIndex = i + 1;
            if (addIndex >= tile.Edges.Count)
            {
                addIndex = 0;
            }
            cornerList[i].EdgeNeighbours.Add(tile.Edges[addIndex]);

        }
        tile.Corners = cornerList;
    }

    private void CombineCorners(int column, int row)
    {

        Tile startTile = GetTile(column, row);
        List<Tile> adjacentTiles = GetAdjacentTiles(column, row);
        foreach (Corner corner in startTile.Corners)
        {
            foreach (Tile tile in adjacentTiles)
            {

                // Only set overlap when there is overlap
                if (tile.Corners.Find(cor => corner.Position == cor.Position) != null)
                {
                    SetOverlappingCorners(tile, corner);
                }
            }

        }

    }

    private void SetOverlappingCorners(Tile tile, Corner corner)
    {
        foreach (Corner tileCorner in tile.Corners)
        {
            if (tileCorner.Position == corner.Position)
            {

                corner.EdgeNeighbours = FindOverlappingEdges(corner, tileCorner);
                corner = tileCorner;
            }

        }
    }

    private List<Edge> FindOverlappingEdges(Corner corner1, Corner corner2)
    {
        List<Edge> edgeList = new List<Edge>();

        edgeList = corner1.EdgeNeighbours;
        foreach(Edge edge in corner2.EdgeNeighbours)
        {
            if (edgeList.Find(edg => edg.EdgePosition == edge.EdgePosition) == null)
            {
                edgeList.Add(edge);
            }
        }

        return edgeList;
    }
    

    private List<Tile> GetAdjacentTiles(int column, int row)
    {
        List<Tile> adjacentTiles = new List<Tile>();
        Tile northTile = GetTile(column + 1, row);
        Tile southTile = GetTile(column - 1, row);
        Tile eastTile = GetTile(column, row + 1);
        Tile westTile = GetTile(column, row - 1);


        adjacentTiles.Add(northTile);
        adjacentTiles.Add(eastTile);
        adjacentTiles.Add(westTile);
        adjacentTiles.Add(southTile);

        Tile southEastTile = GetTile(column - 1, row + 1);
        Tile soutWestTile = GetTile(column - 1, row - 1);
        Tile northEastTile = GetTile(column + 1, row + 1);
        Tile northWestTile = GetTile(column + 1, row - 1);


        adjacentTiles.Add(northEastTile);
        adjacentTiles.Add(northWestTile);
        adjacentTiles.Add(southEastTile);
        adjacentTiles.Add(soutWestTile);

        adjacentTiles.RemoveAll(tile => tile == null);


        return adjacentTiles;
    }


    private Edge GetCommonEdge(Tile tile1, Tile tile2)
    {
        if (tile1 == null || tile2 == null)
        {
            return null;
        }
        foreach (Edge edge1 in tile1.Edges)
        {
            if (edge1 == null)
            {
                continue;
            }
            foreach (Edge edge2 in tile2.Edges)
            {
                if (edge2 == null)
                {
                    continue;
                }
                if (edge1.EdgePosition == edge2.EdgePosition)
                {
                    return edge1;
                }

            }
        }
        return null;
    }

    private Corner GetCommonCorner(Edge edge1, Edge edge2)
    {
        if (edge1 == null || edge2 == null)
        {
            return null;
        }
        foreach (Corner corner1 in edge1.Corners)
        {
            foreach (Corner corner2 in edge2.Corners)
            {
                if (corner1.Position == corner2.Position)
                {
                    return corner1;
                }
            }
        }
        return null;
    }

    // Helpers

    private Tile GetTile(int column, int row)
    {

        // Out of bounds tile
        if (row > _rowSize || column > _columnSize || row < 0 || column < 0)
        {
            // Debug.LogError("Out of bounds tile");
            return null;
        }

        int index = (_columnSize * column) + row;
        // Safety check
        if (index < 0 || index >= _gridSize)
        {
            return null;
        }

        return Tiles[index];
    }


    private List<Edge> FindEdges(int column, int row)
    {
        Tile currentTile = GetTile(column, row);
        if (currentTile == null)
        {
            Debug.LogError("Invalid tile: GridManager.FindEdges()");
        }
        List<Edge> edgeList = new List<Edge> { null, null, null, null };
        // NORTH-SOUTH-EAST-WEST

        Tile northTile = GetTile(column + 1, row);
        Tile southTile = GetTile(column - 1, row);
        Tile eastTile = GetTile(column, row + 1);
        Tile westTile = GetTile(column, row - 1);

        AddEdgeToTile(Tile.EdgeDirection.North, currentTile, northTile, edgeList);
        AddEdgeToTile(Tile.EdgeDirection.South, currentTile, southTile, edgeList);
        AddEdgeToTile(Tile.EdgeDirection.East, currentTile, eastTile, edgeList);
        AddEdgeToTile(Tile.EdgeDirection.West, currentTile, westTile, edgeList);




        return edgeList;
    }


    private Tile GetNearestTile(Vector3 position)
    {
        float distance = float.MaxValue;
        float maxDistance = 50.0f;
        Tile closestTile = new();
        foreach (Tile tile in Tiles)
        {
            float newDistance = Vector3.Distance(position, tile.Position);
            if (newDistance < distance)
            {
                distance = newDistance;
                closestTile = tile;
            }
        }
        if (distance > maxDistance)
        {
            return null;
        }
        return closestTile;
    }

    private Edge GetNearestTileEdge(Vector3 position, Tile tile)
    {
        float distance = float.MaxValue;
        Edge closestEdge = new();
        foreach (Edge edge in tile.Edges)
        {
            if (edge == null)
                continue;

            float newDistance = Vector3.Distance(position, edge.EdgePosition);
            if (newDistance < distance)
            {
                distance = newDistance;
                closestEdge = edge;
            }
        }
        return closestEdge;
    }

    private Tile.EdgeDirection InverseDirection(Tile.EdgeDirection direction)
    {
        switch (direction)
        {
            case Tile.EdgeDirection.North:
                return Tile.EdgeDirection.South;
            case Tile.EdgeDirection.South:
                return Tile.EdgeDirection.North;
            case Tile.EdgeDirection.East:
                return Tile.EdgeDirection.West;
            case Tile.EdgeDirection.West:
                return Tile.EdgeDirection.East;
        }
        Debug.LogError("ERROR: INVALID DIRECTION");
        return Tile.EdgeDirection.North;

    }

    //TODO 
    private List<Edge> GetNearbyPlacedWalls(Edge edge)
    {
        List<Edge> walls = new List<Edge>();

        foreach (Corner corner in edge.Corners)
        {
            foreach (Edge cornerEdge in corner.EdgeNeighbours)
            {

            }
        }

        return walls;
    }


    // Calculating values

    private Vector3 GetMidpoint(Vector3 pointA, Vector3 pointB)
    {
        return (pointA + pointB) * 0.5f;
    }



}

