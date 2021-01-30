using UnityEngine;

public class ObstacleAvoidance : Seek
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// The terrain layer
    /// </summary>
    [SerializeField] private LayerMask _obstacleLayer;

    /// <summary>
    /// Holds the minimum distance to a wall
    /// </summary>
    [SerializeField] private float _avoidDistance;

    /// <summary>
    /// Holds the whiskers that will detect future collisions
    /// </summary>
    [SerializeField] private Whisker[] _whiskers;

    /// <summary>
    /// Determine if we should draw whiskers
    /// </summary>
    [SerializeField] private bool _drawGizmos;

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    protected void Start()
    {
        _target = new GameObject(this.name + " invisible target (OBSTACLE AVOIDANCE)").AddComponent<Agent>();
        _target.Position = transform.position;
    }

    public override Steering GetSteering(AgentNpc agent)
    {
        // 1. Calculate the target to delegate to seek

        // Calculate the collision ray vector 
        Vector3 mainDirection = agent.Velocity.normalized;
        //Vector3 mainDirection = MathIAVJ.OrientationAsVector(agent.Orientation);
        RaycastHit hit = new RaycastHit();
        bool collision = false;

        for (int i = 0; i < _whiskers.Length && !collision; i++)
        {
            // Calculate the collision ray vector 
            Vector3 rotation = new Vector3(0f, 0f, _whiskers[i].Angle);
            Vector3 direction = Quaternion.Euler(rotation) * mainDirection;
            direction.Normalize();

            // Find the collision           
            collision = Physics.Raycast(agent.Position, direction, out hit, _whiskers[i].Lookahead, _obstacleLayer);  
        }

        if (_drawGizmos)
        {
            DrawGizmos(mainDirection);
        }      

        // If have no collision, do nothing
        if (!collision)
        {
            return new Steering();
            //_target.transform.position = transform.position;
        }
        // Otherwise create a target
        else
        {
            _target.Position = hit.point + hit.normal * _avoidDistance;
        }
        

        // 2. Delegate to seek
        return base.GetSteering(agent);
    }

    /// <summary>
    /// Draw the whiskers
    /// </summary>
    /// <param name="mainDirection"></param>
    private void DrawGizmos(Vector3 mainDirection)
    {
        for (int i = 0; i < _whiskers.Length; i++)
        {
            // Calculate the collision ray vector 
            Vector3 rotation = new Vector3(0, 0f, _whiskers[i].Angle);
            Vector3 direction = Quaternion.Euler(rotation) * mainDirection;
            direction.Normalize();
            Debug.DrawRay(transform.position, direction * _whiskers[i].Lookahead, Color.cyan);
        }
    }

    private void OnDestroy()
    {
        if (_target != null)
        {
            Destroy(_target.gameObject);
        }
    }
}

/// <summary>
/// A whisker contains the information to detect collisions using rays
/// </summary>
[System.Serializable]
class Whisker
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Holds the distance to look ahead for a collision
    /// (length of the collision ray)
    /// </summary>
    [SerializeField] private float _lookahead;

    /// <summary>
    /// Angle of the whisker
    /// </summary>
    [SerializeField] private float _angle;

    ///////////////////////////////////////////////////
    ///////////////////// ACCESS //////////////////////
    ///////////////////////////////////////////////////

    public float Lookahead
    {
        get { return _lookahead; }
        set { _lookahead = value; }
    }

    public float Angle 
    {
        get { return _angle; }
        set { _angle = value; }
    }
}
