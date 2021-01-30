using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringAction : Action
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// The target
    /// </summary>
    protected Agent _target;

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="SteeringAction"/> class.
    /// </summary>
    /// <param name="expiryTime">The expiry time.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="agent">The agent.</param>
    /// <param name="target">The target.</param>
    protected SteeringAction(float expiryTime, int priority, AgentNpc agent, Agent target) : base(expiryTime, priority, agent)
    {
        _target = target;
    }
}
