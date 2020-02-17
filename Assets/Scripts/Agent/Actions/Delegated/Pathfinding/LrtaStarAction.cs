using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LrtaStarAction : PathfindingAction
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// The LRTA
    /// </summary>
    private LRTAStar _lrta;

    /// <summary>
    /// The next lrta position
    /// </summary>
    private Vector2Int _nextLrtaPos;

    /// <summary>
    /// Determines if the action is finished
    /// </summary>
    private bool _finished;

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="LrtaStarAction" /> class.
    /// </summary>
    /// <param name="expiryTime">The expiry time.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="agent">The agent.</param>
    /// <param name="targetPos">The target position.</param>
    public LrtaStarAction(float expiryTime, int priority, AgentNPC agent, Vector2Int targetPos) 
        : base(expiryTime, priority, agent, targetPos)
    {
        _lrta = new LRTAStar(agent.CurrentMap, agent, agent.MapPosition, targetPos);
    }

    /// <summary>
    /// Executes this action.
    /// </summary>
    public override void Execute()
    {
        if (!_started)
        {
            _started = true;
            _finished = !NextLrta();
        }
        else if (_agent.MapPosition == _nextLrtaPos)
        {
            // The action is finished if there is no new steps in the lrta
            _finished = !NextLrta();
        }
    }

    /// <summary>
    /// Determines whether this action is complete.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this action is complete; otherwise, <c>false</c>.
    /// </returns>
    public override bool IsComplete()
    {
        return _finished;
    }


    /// <summary>
    /// Calculates the next step in the LRTA.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if there is a new path to follow; otherwise, <c>false</c>. 
    /// </returns>
    private bool NextLrta()
    {
        Path path = _lrta.FindPath(_lrta.Pathfinder, _agent.Position, _agent.CurrentMap.GridToWorld(_lrta.Objective));

        if ((path == null) || path.IsEmpty()) return false;

        _agent.DebugPath = path;

        PathFollowing pf = _agent.GetComponent<PathFollowing>();
        pf.Path = path;
        _nextLrtaPos = _agent.CurrentMap.WorldToGrid(path.NodePath[0].transform.position);
        return true;
    }
}
