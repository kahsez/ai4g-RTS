using System;
using UnityEngine;

public class Face : Align
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    protected Agent _faceTarget;

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    protected virtual void Start()
    {
        _faceTarget = _target;
        GameObject obj = new GameObject(this.name + " invisible target (FACE)");
        _target = obj.AddComponent<Agent>();
    }

    public override Steering GetSteering(AgentNPC agent)
    {
        if (_faceTarget == null) return new Steering();

        // 1. Calculate the target to delegate to align

        // Work out the direction to target
        Vector3 direction = _faceTarget.Position - agent.Position;

        // Check for a zero direction, and make no change if so
        if (direction.sqrMagnitude > 0.1)
        {
            // Put the target together
            _target.Orientation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }

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

    protected override void ChangeTarget(Agent agent)
    {
        if (_target != null) Destroy(_target);
        _faceTarget = agent;
        GameObject obj = new GameObject(this.name + " invisible target (FACE)");
        _target = obj.AddComponent<Agent>();
    }

}
