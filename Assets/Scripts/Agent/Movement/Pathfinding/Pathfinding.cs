using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public abstract class Pathfinding
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// The map
    /// </summary>
    protected Map _map;

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="Pathfinding"/> class.
    /// </summary>
    /// <param name="map">The map.</param>
    protected Pathfinding(Map map)
    {
        _map = map;
    }


    protected abstract Vector2Int[] FindPath(IPathfinder pathfinder, Vector2Int start, Vector2Int objective);

    /// <summary>
    /// Finds the path.
    /// </summary>
    /// <param name="pathfinder">The pathfinder.</param>
    /// <param name="currentPos">The current position.</param>
    /// <param name="goal">The goal.</param>
    /// <returns></returns>
    public Path FindPath(IPathfinder pathfinder, Vector3 currentPos, Vector3 goal)
    {
        Vector2Int start = _map.WorldToGrid(currentPos);
        Vector2Int objective = _map.WorldToGrid(goal);
        Vector2Int[] waypoints = FindPath(pathfinder, start, objective);

        return Path.ToPath(waypoints, _map);
    }

    /// <summary>
    /// Determines whether [is impassable terrain] [the specified position].
    /// </summary>
    /// <param name="pos">The position.</param>
    /// <param name="pathfinder">The pathfinder.</param>
    /// <returns>
    ///   <c>true</c> if [is impassable terrain] [the specified position]; otherwise, <c>false</c>.
    /// </returns>
    protected bool IsImpassableTerrain(Vector2Int pos, IPathfinder pathfinder)
    {
        if (!_map.IsValid(pos))
            return true;
        return float.IsPositiveInfinity(pathfinder.GetTerrainFactor(_map.Get(pos.x, pos.y)));
    }

    /// <summary>
    /// Determines whether the specified current position is reachable.
    /// </summary>
    /// <param name="currentPos">The current position.</param>
    /// <param name="targetPos">The target position.</param>
    /// <param name="pathfinder">The pathfinder.</param>
    /// <returns>
    ///   <c>true</c> if the specified current position is reachable; otherwise, <c>false</c>.
    /// </returns>
    protected bool IsReachable(Vector2Int currentPos, Vector2Int targetPos, IPathfinder pathfinder)
    {
        if (IsImpassableTerrain(targetPos, pathfinder)) return false;

        Vector2Int aux = targetPos - currentPos;
        if (aux.x == 0 || aux.y == 0) return true;

        for (int i = -1; i <= 1; i += 2)
        {
            for (int j = -1; j <= 1; j += 2)
            {
                if (aux.x == i && aux.y == j)
                {
                    Vector2Int adj1 = currentPos + new Vector2Int(i, 0);
                    Vector2Int adj2 = currentPos + new Vector2Int(0, j);
                    return !(IsImpassableTerrain(adj1, pathfinder) && IsImpassableTerrain(adj2, pathfinder));
                }
            }
        }

        return false;
    }

}
