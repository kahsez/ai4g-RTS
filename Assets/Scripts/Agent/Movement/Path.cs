using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class that represents a path made of nodes linked by straight line segments
/// </summary>
public class Path : MonoBehaviour
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Nodes of the path
    /// </summary>
    private List<GameObject> _nodes = new List<GameObject>();

    /// <summary>
    /// Determines if the path is circular
    /// </summary>
    [SerializeField] private bool _isCircular = false;


    [Header("Gizmos")]
    /// <summary>
    /// Determines if the path should be drawn
    /// </summary>
    [SerializeField] private bool _drawPath;

    /// <summary>
    /// Radius of the Gizmo
    /// </summary>
    [SerializeField] private float _nodeGizmoRadius = 0.5f;


    ///////////////////////////////////////////////////
    ///////////////////// ACCESS //////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Gets or sets a value indicating whether this instance is circular.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is circular; otherwise, <c>false</c>.
    /// </value>
    public bool IsCircular 
    {
        get { return _isCircular; }
        set { _isCircular = value; }
    }

    /// <summary>
    /// Gets or sets the node path.
    /// </summary>
    /// <value>
    /// The node path.
    /// </value>
    public List<GameObject> NodePath 
    {
        get { return _nodes; }
        set { _nodes = value; }
    }


    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Adds the node.
    /// </summary>
    /// <param name="node">The node.</param>
    public void AddNode(GameObject node)
    {
        _nodes.Add(node);
    }

    /// <summary>
    /// Determines whether this instance is empty.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
    /// </returns>
    public bool IsEmpty()
    {
        return _nodes.Count == 0;
    }

    /// <summary>
    /// Gets the closest point of the path to the given position
    /// </summary>
    /// <param name="position">Position we want to calculate the closest point</param>
    /// <param name="lastNode">Last visited node of the path</param>
    /// <returns>A param is a position relative to the path and a node</returns>
    public float GetParam(Vector3 position, int lastNode)
    {
        Vector3 a = _nodes[lastNode].transform.position;
        Vector3 b = _nodes[GetNextIndex(_nodes, lastNode)].transform.position;

        // Vector to project the position in the segment
        Vector3 p = position - a;

        // Segment corresponding to the node
        Vector3 q = b - a;

        // Using the magnitude of the projection we get the param 
        Vector3 projection = Vector3.Project(p, q);

        // We check if the projection has the same direction of the path
        // (1 same direction, -1 opposite)
        float direction = (Vector3.Dot(projection, q) > 0 ? 1 : -1);

        return projection.magnitude * direction;
    }

    /// <summary>
    /// Gets a world position in the path and updates the node if necessary
    /// </summary>
    /// <param name="node">Node to which belongs the param</param>
    /// <param name="param">Param we are converting to a position</param>
    /// <returns>Path position</returns>
    public PathPosition GetPosition(int node, float param)
    {
        Vector3 a = _nodes[node].transform.position;
        Vector3 b = _nodes[GetNextIndex(_nodes, node)].transform.position;
        Vector3 segment = b - a;
        float segmentLength = segment.magnitude;

        // If the param is beyond the next node we have to recalculate
        if (param >= segmentLength)
        {
            // We update the node and the rest of the param
            node = GetNextIndex(_nodes, node);
            param -= segmentLength;
            // We recalculate the segment
            a = _nodes[node].transform.position;
            b = _nodes[GetNextIndex(_nodes, node)].transform.position;
            segment = b - a;
        }

        // Transform the param into a position inside the path
        segment.Normalize();
        segment *= param;
        segment += a;

        return new PathPosition(node, segment);
    }


    void OnDrawGizmos()
    {
        if (!_drawPath) return;

        Gizmos.color = Color.magenta;
        foreach (GameObject node in _nodes)
        {
            Gizmos.DrawWireSphere(node.transform.position, _nodeGizmoRadius);
        }

        for (int i = 0; i < _nodes.Count; i++)
        {
            Gizmos.DrawLine(_nodes[i].transform.position, _nodes[GetNextIndex(_nodes, i)].transform.position);
        }
    }

    /// <summary>
    /// Calculates the next node in the path
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private int GetNextIndex(List<GameObject> nodes, int index)
    {       
        if (_isCircular) return (index + 1) % nodes.Count;
        return Mathf.Min(index + 1, nodes.Count - 1);
    }


    /// <summary>
    /// Converts to path.
    /// </summary>
    /// <param name="waypoints">The waypoints.</param>
    /// <param name="map">The map.</param>
    /// <returns></returns>
    public static Path ToPath(Vector2Int[] waypoints, Map map)
    {
        if (waypoints.Length == 0)
            return null;

        Path path = new GameObject("PATH: (" + waypoints[0] + " -> " + waypoints[waypoints.Length - 1] + ")")
            .AddComponent<Path>();
        path.IsCircular = false;

        for (int i = 0; i < waypoints.Length; i++)
        {
            GameObject novo = new GameObject("Waypoint " + i);
            novo.transform.position = map.GridToWorld(waypoints[i]);
            novo.transform.parent = path.gameObject.transform;
            path.AddNode(novo);
        }
        
        return path;
    }

    public virtual void Destroy()
    {
        Destroy(this.gameObject);
    }
}

/// <summary>
/// Represents a position in a path
/// </summary>
public class PathPosition
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Node in the path corresponding to the position
    /// </summary>
    private int _node;

    /// <summary>
    /// Position in the path
    /// </summary>
    private Vector3 _position;

    ///////////////////////////////////////////////////
    ///////////////////// ACCESS //////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Gets the node.
    /// </summary>
    /// <value>
    /// The node.
    /// </value>
    public int Node
    {
        get { return _node; }
    }

    /// <summary>
    /// Gets the position.
    /// </summary>
    /// <value>
    /// The position.
    /// </value>
    public Vector3 Position 
    {
        get { return _position; }
    }

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="node"></param>
    /// <param name="position"></param>
    public PathPosition(int node, Vector3 position)
    {
        _node = node;
        _position = position;
    }
}
