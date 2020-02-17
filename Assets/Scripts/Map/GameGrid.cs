using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Class representing a generic grid
/// </summary>
/// <typeparam name="T"></typeparam>
public class GameGrid <T>
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Number of rows and columns of the grid
    /// </summary>
    private int _rows, _columns;

    /// <summary>
    /// Table representing the grid
    /// </summary>
    private T[,] grid;

    /////////////////////////////////////////////////// 
    ///////////////////// ACCESS //////////////////////
    ///////////////////////////////////////////////////

    public int Rows 
    {
        get { return _rows; }
    }

    public int Columns 
    {
        get { return _columns; }
    }

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rows"></param>
    /// <param name="columns"></param>
    /// <param name="defaultValue"></param>
    public GameGrid(int rows, int columns, T defaultValue = default(T))
    {
        this._rows = rows;
        this._columns = columns;

        grid = new T[rows, columns];
        FillGrid(defaultValue);
    }

    /// <summary>
    /// Fills the grid with a value
    /// </summary>
    /// <param name="value"></param>
    public void FillGrid(T value)
    {
        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                grid[i, j] = value;
            }
        }         
    }

    /// <summary>
    /// Print the grid
    /// </summary>
    public void Print()
    {
        StringBuilder str = new StringBuilder();
        for(int i = 0; i < _rows; i++)
        {
            for(int j = 0; j < _columns; j++)
            {
                str.Append(Get(i, j) + " ");
            }

            str.AppendLine();
        }

        Debug.Log(str.ToString());
    }

    /// <summary>
    /// Checks if the position i, j is out of bounds
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    private bool IsOutOfBounds(int i, int j)
    {
        return ((i < 0) || (i >= _rows) || (j < 0) || (j >= _columns));
    }

    /// <summary>
    /// Get the element in the position i, j
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public T Get(int i, int j)
    {
        return (!IsOutOfBounds(i, j)) ? grid[i, j] : default(T);
    }

    public T Get(Vector2Int pos)
    {
        // Converts (x,y) to (i,j)
        return Get(_rows - pos.y - 1, pos.x);
    }

    /// <summary>
    /// Set the value of the position i, j
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Set(int i, int j, T value)
    {
        if (!IsOutOfBounds(i, j))
        {
            grid[i, j] = value;
            return true;
        }

        Debug.Log("srt fuera");
        return false;          
    }

    public bool Set(Vector2Int pos, T value)
    {
        // Converts (x,y) to (i,j)
        return Set(_rows - pos.y - 1, pos.x, value);
    }


}
