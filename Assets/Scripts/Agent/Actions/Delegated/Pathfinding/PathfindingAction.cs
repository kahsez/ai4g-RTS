using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathfindingAction : Action
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// The target position
    /// </summary>
    protected Vector2Int _targetPos;


    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    protected PathfindingAction(float expiryTime, int priority, AgentNpc agent, Vector2Int targetPos) : base(expiryTime, priority, agent)
    {
        _targetPos = targetPos;
    }

    public override void Cancel()
    {
        _agent.CancelPathFollowing();
    }

    /// <summary>
    /// Determines whether this action can be done with the specified other action.
    /// </summary>
    /// <param name="otherAction">The other action.</param>
    /// <returns>
    ///   <c>true</c> if this instance can be done with the specified other action; otherwise, <c>false</c>.
    /// </returns>
    public override bool CanDoBoth(Action otherAction)
    {
        return false;
    }

    /// <summary>
    /// Determines whether this action can interrupt.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance can interrupt; otherwise, <c>false</c>.
    /// </returns>
    public override bool CanInterrupt()
    {
        return true;
    }


}
