using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class LRTAStar : Pathfinding
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////


    /// <summary>
    /// The heuristic grid
    /// </summary>
    protected GameGrid<float> _heuristicGrid;

    protected IPathfinder _pathfinder;
    protected Vector2Int _start;
    protected Vector2Int _objective;

    ///////////////////////////////////////////////////
    ///////////////////// ACCESS //////////////////////
    ///////////////////////////////////////////////////


    /// <summary>
    /// Gets the start.
    /// </summary>
    /// <value>
    /// The start.
    /// </value>
    public Vector2Int Start 
    {
        get { return _start; }
    }


    /// <summary>
    /// Gets the objective.
    /// </summary>
    /// <value>
    /// The objective.
    /// </value>
    public Vector2Int Objective 
    {
        get { return _objective; }
    }


    /// <summary>
    /// Gets the pathfinder.
    /// </summary>
    /// <value>
    /// The pathfinder.
    /// </value>
    public IPathfinder Pathfinder 
    {
        get { return _pathfinder; }
    }

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////


    /// <summary>
    /// Initializes a new instance of the <see cref="LRTAStar"/> class.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <param name="pathfinder">The pathfinder.</param>
    /// <param name="start">The start.</param>
    /// <param name="objective">The objective.</param>
    public LRTAStar(Map map, IPathfinder pathfinder, Vector2Int start, Vector2Int objective) : base(map)
    {
        // Init the heuristic grid
        _heuristicGrid = new GameGrid<float>(map.Height, map.Width);
        _pathfinder = pathfinder;
        _start = start;
        _objective = objective;
       
        for (int x = 0; x < _map.Width; x++)
        {
            for (int y = 0; y < _map.Height; y++)
            {
                // Set the initial heuristic from each node to the goal node

                Vector2Int mapPosition = new Vector2Int(x, y);
                _heuristicGrid.Set(mapPosition, pathfinder.GetHeuristicFunc()(mapPosition, _objective));
            }
        }
    }


    /// <summary>
    /// Finds the path to the objective with lookahead-one
    /// </summary>
    /// <param name="pathfinder">The pathfinder.</param>
    /// <param name="start">The start.</param>
    /// <param name="objective">The objective.</param>
    /// <returns></returns>
    protected override Vector2Int[] FindPath(IPathfinder pathfinder, Vector2Int start, Vector2Int objective)
    {
        Vector2Int currentNode = start;
        Vector2Int neighbour = Vector2Int.zero;

        if (currentNode == objective) return new Vector2Int[0];

        float minCost = Mathf.Infinity;
        Vector2Int closestNeighbour = Vector2Int.zero;

        // For each neighbour
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                neighbour = currentNode + new Vector2Int(x, y);
                // Check that this neighbour is not out of bounds and it's not the same node
                if (_map.IsValid(neighbour) && (neighbour != currentNode))
                {
                    // This neighbour is valid and we can explore it
                    // Get the neighbour's terrain type 
                    TerrainType terrain = _map.Get(neighbour.x, neighbour.y);
                    // g(x)
                    float baseCost = ((x == 0) || (y == 0)) ? 1f : MathAI.SqrtTwo;
                    float travelCost = IsReachable(currentNode, neighbour, pathfinder) ? (baseCost / pathfinder.GetTerrainFactor(terrain)) : Mathf.Infinity;
                    // h(x)
                    float heuristicCost = _heuristicGrid.Get(neighbour);
                    // f(x)
                    float estimatedCost = travelCost + heuristicCost;

                    if (estimatedCost < minCost)
                    {
                        minCost = estimatedCost;
                        closestNeighbour = neighbour;
                    }                       
                }
            }
        }

        // Update cost
        float currentNeighbourCost = _heuristicGrid.Get(closestNeighbour);
        float updatedCost = Mathf.Max(currentNeighbourCost, minCost);
        _heuristicGrid.Set(closestNeighbour, updatedCost);

        // Return the map (it's always the closest neighbour)
        return new Vector2Int[1] {closestNeighbour};
    }


    /// <summary>
    /// Gets the neighbours.
    /// </summary>
    /// <param name="current">The current.</param>
    /// <returns></returns>
    private Vector2Int[] GetNeighbours(Vector2Int current)
    {
        List<Vector2Int> list = new List<Vector2Int>();

        Vector2Int aux = Vector2Int.zero;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                aux = current + new Vector2Int(x, y);
                if (_map.IsValid(aux))
                {
                    list.Add(aux);
                }
            }
        }

        return list.ToArray();
    }
}
