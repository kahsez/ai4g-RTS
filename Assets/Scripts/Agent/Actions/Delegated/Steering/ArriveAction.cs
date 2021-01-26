using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArriveAction : SteeringAction
{
    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="ArriveAction" /> class.
    /// </summary>
    /// <param name="expiryTime">The expiry time.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="agent">The agent.</param>
    /// <param name="target">The target.</param>
    public ArriveAction(float expiryTime, int priority, AgentNpc agent, Agent target) : base(expiryTime, priority, agent, target)
    {
    }

    public override void Execute()
    {
        if (!_started)
        {
            _agent.SetTarget<Arrive>(_target);
            _started = true;
        }      
    }

    public override bool CanInterrupt()
    {
        return true;
    }

    public override bool CanDoBoth(Action otherAction)
    {
        return false;
    }

    public override bool IsComplete()
    {
        return false;
    }

    public override void Cancel()
    {
        _agent.SetTarget<Arrive>(null);
    }
}
