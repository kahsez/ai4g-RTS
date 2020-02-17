using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAction : PathfindingAction
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// The coroutine controller
    /// </summary>
    protected CoroutineController _coroutineController;

    private InfluenceMap _influenceMap;

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="AStarAction"/> class.
    /// </summary>
    /// <param name="expiryTime">The expiry time.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="agent">The agent.</param>
    /// <param name="targetPos">The target position.</param>
    public AStarAction(float expiryTime, int priority, AgentNPC agent, Vector2Int targetPos, InfluenceMap influenceMap = null) 
        : base(expiryTime, priority, agent, targetPos)
    {
        _coroutineController =
            GameObject.FindGameObjectWithTag("CoroutineController").GetComponent<CoroutineController>();
        _influenceMap = influenceMap;
    }

    /// <summary>
    /// Executes this action.
    /// </summary>
    public override void Execute()
    {
        if (_started) return;

        Map map = _agent.CurrentMap;

        PathFollowing following = _agent.GetComponent<PathFollowing>();
        Vector2Int[] tempPath = {_agent.MapPosition, _targetPos};
        Path temporalPath = Path.ToPath(tempPath, map);
        following.Path = temporalPath;
        _coroutineController.StartChildCoroutine(AStarCoroutine(map, following, _influenceMap));

        _started = true;
    }

    /// <summary>
    /// Determines whether this action is complete.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this action is complete; otherwise, <c>false</c>.
    /// </returns>
    public override bool IsComplete()
    {
        return _agent.GetComponent<PathFollowing>().IsFinished(_agent) && _started;
    }

    /// <summary>
    /// as the star coroutine.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <param name="following">The following.</param>
    /// <param name="influenceMap">The influence map.</param>
    /// <returns></returns>
    private IEnumerator AStarCoroutine(Map map, PathFollowing following, InfluenceMap influenceMap = null)
    {
        Pathfinding pf = new AStar(map, influenceMap);
        Path path = pf.FindPath(_agent, _agent.Position, map.GridToWorld(_targetPos));

        if ((path != null) && !path.IsEmpty())
        {
            _agent.DebugPath = path;
            following.Path = path;
        }
        else
        {
            _agent.DebugPath = null;
        }

        yield return null;
    }
}
