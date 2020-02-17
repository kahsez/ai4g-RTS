
using System;
using UnityEngine;

public interface IPathfinder
{
    /// <summary>
    /// Gets the terrain speed.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    float GetTerrainFactor(TerrainType type);

    /// <summary>
    /// Gets the heuristic function.
    /// </summary>
    /// <returns></returns>
    Func<Vector2Int, Vector2Int, float> GetHeuristicFunc();

    NPCProperties.Faction Faction { get; }
}
