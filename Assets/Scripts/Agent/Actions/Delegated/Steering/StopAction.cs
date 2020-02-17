using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopAction : Action
{
    public StopAction(float expiryTime, int priority, AgentNPC agent) : base(expiryTime, priority, agent)
    {
    }

    public override bool CanDoBoth(Action otherAction)
    {
        if (otherAction is AttackAction)
            return true;
        if (otherAction is FaceAction)
            return true;
        return false;
    }

    public override bool CanInterrupt()
    {
        return false;
    }

    public override void Execute()
    {
        _agent.Velocity /= 1.071f;
    }

    public override bool IsComplete()
    {
        return false;
    }

    public override void Cancel()
    {
    }
}
