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
                tile.TileIndex = GetIndex(column, row);

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
            int column = GetColumn(i);
            int row = GetRow(i);

            Tiles[i].Edges = FindEdges(column, row);


        }
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


    private void InitializeCorners()
    {
        for (int i = 0; i < Tiles.Count; i++)
        {
            AddCornersToTile(i);
        }
    }

    private void AddCornersToTile(int index)
    {
        // Get nearby tiles
        List<Tile> tiles = GetAdjacentTiles(index);

        Tile middleTile = Tiles[index];
        foreach (Tile tile in tiles)
        {
            // Get overlapping edge with main tile 
            Edge overlappingEdge = GetOverlappingEdge(tile, middleTile);

            // Generate corners
            if (overlappingEdge != null)
            {
                GenerateCorners(overlappingEdge);
            }

        }


        // Combine corners tile
        CombineCorners(index);
    }

    private void GenerateCorners(Edge edge)
    {
        Corner cornerA = new Corner();
        Corner cornerB = new Corner();
        Vector3 cornerPosA = edge.EdgePosition;
        Vector3 cornerPosB = edge.EdgePosition;
        switch (edge.CurrentEdgeOrientation)
        {

            case Edge.EdgeOrientation.Upwards:
                cornerPosA.x -= _tileSize * 0.5f;
                cornerPosB.x += _tileSize * 0.5f;
                break;
            case Edge.EdgeOrientation.Sideways:
                cornerPosA.z -= _tileSize * 0.5f;
                cornerPosB.z += _tileSize * 0.5f;
                break;
        }

        cornerA.Position = cornerPosA;
        cornerB.Position = cornerPosB;

        edge.Corners[0] = cornerA;
        edge.Corners[1] = cornerB;

    }

    private void CombineCorners(int index)
    {
        Tile tile = Tiles[index];
        for (int i = 0; i < tile.Edges.Count; i++)
        {
            Edge currentEdge = tile.Edges[i];
            int nextIndex = i + 1;
            if (nextIndex >= tile.Edges.Count)
            {
                nextIndex = 0;
            }
            Edge nextEdge = tile.Edges[nextIndex];
            if (currentEdge.CornersSet && nextEdge.CornersSet)
            {
                CombineCornersEdges(currentEdge, nextEdge);
            }
        }
    }

    private void CombineCornersEdges(Edge edge1, Edge edge2)
    {
        for (int edge1Index = 0; edge1Index < edge1.Corners.Count; edge1Index++)
        {
            for (int edge2Index = 0; edge2Index < edge2.Corners.Count; edge2Index++)
            {
                if (edge1.Corners[edge1Index].Position == edge2.Corners[edge2Index].Position)
                {
                    edge1.Corners[edge1Index] = edge2.Corners[edge2Index];
                    edge1.Corners[edge1Index].EdgeNeighbours.Add(edge1);
                    edge1.Corners[edge1Index].EdgeNeighbours.Add(edge2);
                }
            }
        }
    }

    // Tile helpers

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


    private Edge GetOverlappingEdge(Tile tile1, Tile tile2)
    {
        foreach (Edge edge1 in tile1.Edges)
        {
            foreach (Edge edge2 in tile2.Edges)
            {
                if (edge1.EdgePosition == edge2.EdgePosition)
                {
                    return edge1;
                }

            }
        }

        return null;
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

    private List<Tile> GetAdjacentTiles(int index)
    {
        List<Tile> adjacentTiles = new List<Tile>();

        int column = GetColumn(index);
        int row = GetRow(index);

        for (int columnDif = -1; columnDif < 2; columnDif++)
        {
            for (int rowDif = -1; rowDif < 2; rowDif++)
            {
                // Don't add middle tile
                if (rowDif == 0 && columnDif == 0)
                {
                    continue;
                }
                Tile tile = GetTile(column + columnDif, row + rowDif);
                if (tile != null)
                {
                    adjacentTiles.Add(tile);
                }

            }
        }


        return adjacentTiles;
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


    // Math helpers

    private Vector3 GetMidpoint(Vector3 pointA, Vector3 pointB)
    {
        return (pointA + pointB) * 0.5f;
    }

    private int GetColumn(int index)
    {
        int column = index / _columnSize;
        return column;
    }

    private int GetRow(int index)
    {

        int row = index % _rowSize;
        return row;
    }

    private int GetIndex(int column, int row)
    {
        int index = column * _columnSize + row;
        return index;
    }


}

