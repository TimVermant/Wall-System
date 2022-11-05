using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    [SerializeField] private int _gridSize = 5;
    [SerializeField] private float _tileSize = 5.0f;
    [SerializeField] GameObject _tile;
    [SerializeField] GameObject _wall;


    public List<Tile> Tiles { get; private set; } = new List<Tile>();

    private int _rowSize = 5;
    private int _columnSize = 5;

    private void Awake()
    {
        _gridSize = _rowSize * _columnSize;
        InitializeGrid();
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
                Tile tile = new Tile();

                tile.Position = currentPos;
                Instantiate(_tile, tile.Position, Quaternion.identity);
                Tiles.Add(tile);

                currentPos.x += _tileSize;
            }
            currentPos.z += _tileSize;
        }

        InitializeEdges();
    }

    private void InitializeEdges()
    {
        for (int i = 0; i < Tiles.Count; i++)
        {
            int column = i / _columnSize;
            int row = i % _rowSize;

            Tiles[i].Edges = FindEdges(column, row);
            Tiles[i].PlaceBuilding(Tile.EdgeDirection.North, _wall);
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

        edgeList[(int)Tile.EdgeDirection.North] = AddEdgeToTile(Tile.EdgeDirection.North, northTile);
        edgeList[(int)Tile.EdgeDirection.South] = AddEdgeToTile(Tile.EdgeDirection.South, southTile);
        edgeList[(int)Tile.EdgeDirection.West] = AddEdgeToTile(Tile.EdgeDirection.West, westTile);
        edgeList[(int)Tile.EdgeDirection.East] = AddEdgeToTile(Tile.EdgeDirection.East, eastTile);


        return edgeList;
    }

    private Edge AddEdgeToTile(Tile.EdgeDirection direction, Tile neighbouringTile)
    {
        if (neighbouringTile == null || neighbouringTile.Edges.Count == 0)
        {
            return null;
        }

        Edge edge = neighbouringTile.Edges[(int)InverseDirection(direction)];
        if (edge == null)
        {
            return new Edge();
        }
        else
        {
            return edge;
        }

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

}
