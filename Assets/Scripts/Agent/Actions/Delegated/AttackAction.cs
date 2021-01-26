using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : Action
{
    /// <summary>
    /// The target
    /// </summary>
    private Agent _target;

    private float _timeStamp;

    public AttackAction(float expiryTime, int priority, AgentNpc agent, Agent target) : base(expiryTime, priority, agent)
    {
        _target = target;
    }

    public override bool CanDoBoth(Action otherAction)
    {
        if (otherAction is FaceAction)
            return true;
        if (otherAction is StopAction)
            return true;
        return false;
    }

    public override bool CanInterrupt()
    {
        return true;
    }

    public override void Execute()
    {
        if (!_started)
        {
            _timeStamp = Time.time;
            _agent.Attack(_target);
            _started = true;
        }
        else if (Time.time - _timeStamp > _agent.AttackCooldown)
        {
            _timeStamp = Time.time;
            _agent.Attack(_target);
        }
    }

    public override bool IsComplete()
    {
        return _target.Health <= 0f;
    }

    public override void Cancel()
    {
    }
}
