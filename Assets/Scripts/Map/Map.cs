using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Type of terrain
/// </summary>
public enum TerrainType
{
    MOUNTAIN,
    DESERT,
    FOREST,
    PLAINS,
    ROAD
}

/// <summary>
/// Map
/// </summary>
public class Map : MonoBehaviour
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// The cell size
    /// </summary>
    private float _cellSize;

    /// <summary>
    /// The width of the map
    /// </summary>
    [SerializeField] private int _width;

    /// <summary>
    /// The height of the map
    /// </summary>
    [SerializeField] private int _height;

    /// <summary>
    /// The tilemap
    /// </summary>
    private Tilemap _tilemap;


    ///////////////////////////////////////////////////
    ///////////////////// ACCESS //////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Gets the width.
    /// </summary>
    /// <value>
    /// The width.
    /// </value>
    public int Width 
    {
        get { return _width; }
    }

    /// <summary>
    /// Gets the height.
    /// </summary>
    /// <value>
    /// The height.
    /// </value>
    public int Height 
    {
        get { return _height; }
    }

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////


    /// <summary>
    /// Called when [enable].
    /// </summary>
    private void OnEnable()
    {
        _tilemap = GetComponent<Tilemap>();
        _cellSize = _tilemap.cellSize.x;
    }

    /// <summary>
    /// Gets the terrain type in the position x, y of the map
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public TerrainType Get(int x, int y)
    {
        //Debug.Log("Get: " + x +", " + y);
        //return _grid.Get(_height - y - 1, x);
        return _tilemap.GetTile<TerrainTile>(new Vector3Int(x, y, 0)).Type;
    }

    /// <summary>
    /// Gets the terrain type in the position
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns></returns>
    public TerrainType Get(Vector2Int position)
    {
        return Get(position.x, position.y);
    }

    /// <summary>
    /// Transforms a world position into a map position
    /// </summary>
    /// <param name="position">World position</param>
    /// <returns>Map position</returns>
    public Vector2Int WorldToGrid(Vector3 position)
    {
        int x = (int) (position.x / _cellSize);
        int y = (int) (position.y / _cellSize);
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// Transforms a map position into a world position
    /// </summary>
    /// <param name="position">Map position</param>
    /// <returns>World position</returns>
    public Vector3 GridToWorld(Vector2Int position)
    {
        float x = position.x * _cellSize + _cellSize / 2f;
        float y = position.y * _cellSize + _cellSize / 2f;
        return new Vector3(x, y);
    }

    /// <summary>
    /// Check if a position in the map is inside bounds (not out of the map)
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool IsValid(Vector2Int position)
    {
        return (position.x >= 0) && (position.x < _width) && (position.y >= 0) && (position.y < _height);
    }

}
