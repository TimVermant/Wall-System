using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int _gridSize = 5;
    [SerializeField] private float _tileSize = 5.0f;
    [SerializeField] GameObject _wall;

    public List<Tile> Tiles { get; private set; } = new List<Tile>();


    private void Awake()
    {
        InitializeGrid(_gridSize, _gridSize);
    }

    private void InitializeGrid(int rows, int columns)
    {
        Vector3 startPos = Vector3.zero;
        startPos.x = -(rows / 2.0f) * _tileSize;
        startPos.z = -(columns / 2.0f) * _tileSize;
        Vector3 currentPos = startPos;

        for (int column = 0; column < columns; column++)
        {
            currentPos.x = startPos.x;
            for (int row = 0; row < rows; row++)
            {
                Tile tile = new Tile();
                tile.Position = currentPos;
                //temp
                //tile.Building = Instantiate(_wall,tile.Position,Quaternion.identity);
                Tiles.Add(tile);

                currentPos.x +=  _tileSize;
            }
            currentPos.z += _tileSize;
        }
    }


}
