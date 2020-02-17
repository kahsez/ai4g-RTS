using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinFormationAction : Action
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// The formation
    /// </summary>
    private FormationManager _formation;


    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="JoinFormationAction"/> class.
    /// </summary>
    /// <param name="expiryTime">The expiry time.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="agent">The agent.</param>
    /// <param name="formation">The formation.</param>
    public JoinFormationAction(float expiryTime, int priority, AgentNPC agent, FormationManager formation) : base(expiryTime, priority, agent)
    {
        _agent = agent;
        _started = false;
        _formation = formation;
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
        return true;
    }

    /// <summary>
    /// Determines whether this action can interrupt.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance can interrupt; otherwise, <c>false</c>.
    /// </returns>
    public override bool CanInterrupt()
    {
        return false;
    }

    /// <summary>
    /// Executes this action.
    /// </summary>
    public override void Execute()
    {
        if (!_started)
        {
            _agent.JoinFormation(_formation);
            _started = true;
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
        return _started;
    }

    public override void Cancel()
    {
    }
}
