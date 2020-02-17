using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowingAction : SteeringAction
{
    private Path _path;

    public PathFollowingAction(float expiryTime, int priority, AgentNPC agent, Path path) : base(expiryTime, priority, agent, null)
    {
        _path = path;
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

    public override void Execute()
    {
        if (_started) return;

        PathFollowing following = _agent.GetComponent<PathFollowing>();
        following.Path = _path;

        _started = true;
    }

    public override void Cancel() {
        _agent.CancelPathFollowing();
    }
}
