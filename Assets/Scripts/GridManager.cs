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

        if (neighbouringTile == null || neighbouringTile.Edges.Count == 0)
        {

            Edge newEdge = new();
            newEdge.EdgePosition = edgePosition;
            newEdge.AdjacentTiles = tiles;
            newEdge.CurrentEdgeOrientation = orientation;
            edgeList[(int)direction] = newEdge;
            _wallSystem.AddWall(newEdge);
            return;
        }
        else
        {
            Vector3 midPoint = GetMidpoint(currentTile.Position, neighbouringTile.Position);
            Edge neighboursEdge = neighbouringTile.Edges[(int)InverseDirection(direction)];
            if (neighboursEdge == null)
            {

                Edge newEdge = new();
                newEdge.EdgePosition = midPoint;
                newEdge.AdjacentTiles = tiles;
                newEdge.CurrentEdgeOrientation = orientation;
                edgeList[(int)direction] = newEdge;
                _wallSystem.AddWall(newEdge);
            }
            else
            {
                edgeList[(int)direction] = neighboursEdge;
            }
        }
    }





    // Helpers

    private Tile GetTile(int column, int row)
    {

        // Out of bounds tile
        if (row > _rowSize || column > _columnSize)
        {
            Debug.LogError("Out of bounds tile");
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

    private List<Tile> GetCorneringTiles(int tileIndex)
    {


        return new();

    }

    private Tile GetTile(Tile.EdgeDirection direction, int tileIndex)
    {


        return new();
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

