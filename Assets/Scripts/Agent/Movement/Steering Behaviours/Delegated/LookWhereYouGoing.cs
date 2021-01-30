using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookWhereYouGoing : Align
{
    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    void Start()
    {
        _target = new GameObject(this.name + " invisible target (LOOK)").AddComponent<Agent>();
    }

    public override Steering GetSteering(AgentNpc agent)
    {
        // 1. Calculate the target to delegate to align

        // Check for a zero direction, and make no change if so
        if (agent.Velocity.sqrMagnitude < 0.1)
        {
            return new Steering();        
        }

        // Otherwise set the target based on the velocity
        _target.Orientation = Mathf.Atan2(agent.Velocity.y, agent.Velocity.x) * Mathf.Rad2Deg;

        // 2. Delegate to align
        return base.GetSteering(agent);
    }

    private void OnDestroy()
    {
        if (_target != null)
        {
            Destroy(_target.gameObject);
        }
    }

}