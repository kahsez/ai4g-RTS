using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;

/// <summary>
/// Class with new math methods
/// </summary>
public static class MathAI
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Square root of two
    /// </summary>
    public static float SqrtTwo = Mathf.Sqrt(2f);

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Transform a rotation to a [-180, 180] range
    /// </summary>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static float MapToRange(float rotation)
    {
        rotation %= 360f;
        if (Mathf.Abs(rotation) > 180f)
        {
            if (rotation < 0f)
                rotation += 360f;
            else
                rotation -= 360f;
        }

        return rotation;
    }

    /// <summary>
    /// Returns a random number between -1 and 1, where values around zero are more likely
    /// </summary>
    /// <returns></returns>
    public static float RandomBinomial()
    {
        return Random.Range(0f, 1f) - Random.Range(0f, 1f);
    }

    /// <summary>
    /// Transforms an orientation into a vector
    /// </summary>
    /// <param name="orientation">Orientation in degrees</param>
    /// <returns></returns>
    public static Vector3 OrientationAsVector(float orientation)
    {
        Vector3 vector = Vector3.zero;
        vector.y = Mathf.Sin(orientation * Mathf.Deg2Rad);
        vector.x = Mathf.Cos(orientation * Mathf.Deg2Rad);
        return vector.normalized;
    }

    /// <summary>
    /// Calculate the manhattan distance from a cell to another one in a grid.
    /// </summary>
    /// <param name="from">Origin</param>
    /// <param name="to">Destination</param>
    /// <returns>Distance</returns>
    public static float ManhattanDistance(Vector2Int from, Vector2Int to)
    {
        return Mathf.Abs(to.x - from.x) + Mathf.Abs(to.y - from.y);
    }

    /// <summary>
    /// Calculate the euclidean distance from a cell to another one in a grid.
    /// </summary>
    /// <param name="from">Origin</param>
    /// <param name="to">Destination</param>
    /// <returns>Distance</returns>
    public static float EuclideanDistance(Vector2Int from, Vector2Int to)
    {
        return Vector2Int.Distance(from, to);
    }

    /// <summary>
    /// Calculate the Chebychev distance from a cell to another one in a grid.
    /// </summary>
    /// <param name="from">Origin</param>
    /// <param name="to">Destination</param>
    /// <returns>Distance</returns>
    public static float ChebychevDistance(Vector2Int from, Vector2Int to)
    {
        return Mathf.Max(Mathf.Abs(to.x - from.x), Mathf.Abs(to.y - from.y));
    }
}
