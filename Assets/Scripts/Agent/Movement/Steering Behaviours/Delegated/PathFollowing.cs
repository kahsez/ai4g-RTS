using UnityEngine;

public class PathFollowing : Seek
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Holds the path to follow
    /// </summary>
    [SerializeField] private Path _path;

    /// <summary>
    /// Holds the distance along the path to generate the target
    /// </summary>
    [SerializeField] private float _pathOffset = 1;

    /// <summary>
    /// Holds the last past node on the path
    /// </summary>
    private int _currentNode;

    ///////////////////////////////////////////////////
    ///////////////////// ACCESS //////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Gets or sets the path.
    /// </summary>
    /// <value>
    /// The path.
    /// </value>
    public Path Path
    {
        get { return _path; }
        set
        {
            Reset();
            _path = value;
        }
    }

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    protected void Start()
    {
        _target = new GameObject(this.name + " invisible target (PATH FOLLOWING)").AddComponent<Agent>();
        _target.Position = transform.position;
        _currentNode = 0;
    }

    public void Reset()
    {
        _currentNode = 0;
        if (_path != null)
        {           
            _path.Destroy();
            _path = null;
        }
    }

    public override Steering GetSteering(AgentNPC agent)
    {
        // 1. Calculate the target to delegate to seek

        // Find the current position on the path
        if((_path != null) && IsFinished(agent))
        {
            _path.Destroy();
        }

        if ((_path == null) || _path.IsEmpty())
        {
            return new Steering();
        }

        float currentParam = Path.GetParam(agent.Position, _currentNode);

        // Offset it
        float targetParam = currentParam + _pathOffset;

        // Get the target position
        PathPosition position = Path.GetPosition(_currentNode, targetParam);
        _target.Position = position.Position;

        // Update the current node
        _currentNode = position.Node;

        // 2. Delegate to seek
        return base.GetSteering(agent);
    }

    void OnDestroy()
    {
        if(_target != null)
        {
            Destroy(_target.gameObject);
        }

        if (_path != null)
        {
            _path.Destroy();
        }       
    }

    public bool IsFinished(AgentNPC agent)
    {
        if (_path == null) return true;
        Vector2Int currentMapPos = agent.MapPosition;
        Vector2Int finalPathPosition = agent.CurrentMap.WorldToGrid(_path.NodePath[_path.NodePath.Count - 1].transform.position);
        return currentMapPos.Equals(finalPathPosition);
    }
}