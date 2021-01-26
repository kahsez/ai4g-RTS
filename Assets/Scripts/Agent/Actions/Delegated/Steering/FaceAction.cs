using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceAction : SteeringAction
{
    public FaceAction(float expiryTime, int priority, AgentNpc agent, Agent target) : base(expiryTime, priority, agent, target)
    {
    }

    public override bool CanDoBoth(Action otherAction)
    {
        if (otherAction is AttackAction)
            return true;
        if (otherAction is StopAction)
            return true;
        return false;
    }

    public override bool CanInterrupt()
    {
        return false;
    }

    public override void Execute()
    {
        if (_started) return;

        _started = true;
        _agent.SetTarget<Face>(_target);
    }

    public override bool IsComplete()
    {
        return false;
    }

    public override void Cancel()
    {
        _agent.SetTarget<Face>(null);
    }
}
