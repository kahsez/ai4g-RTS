using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evade : Flee
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    protected Agent _evadeTarget;

    [SerializeField] protected float _maxPrediction;

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    protected virtual void Start()
    {
        _evadeTarget = _target;
        GameObject obj = new GameObject(this.name + " invisible target (EVADE)");
        _target = obj.AddComponent<Agent>();
    }

    public override Steering GetSteering(AgentNpc agent)
    {
        if (_evadeTarget == null) return new Steering();

        // 1. Calculate the target to delegate to seek

        // Work out the distance to target
        Vector3 direction = _evadeTarget.Position - agent.Position;
        float distance = direction.magnitude;

        // Work out our current speed
        float speed = agent.Velocity.magnitude;

        float prediction;
        // Check if speed is too small to give a reasonable prediction time
        if (speed <= distance / _maxPrediction)
        {
            prediction = _maxPrediction;
        }
        // Otherwise calculate the prediction time
        else
        {
            prediction = distance / speed;
        }

        // Put the target together
        _target.Position = _evadeTarget.Position;
        _target.Position += _evadeTarget.Velocity * prediction;

        // 2. Delegate to seek
        return base.GetSteering(agent);
    }

    protected override void ChangeTarget(Agent agent)
    {
        if (_target != null) Destroy(_target);
        _evadeTarget = agent;
        GameObject obj = new GameObject(this.name + " invisible target (EVADE)");
        _target = obj.AddComponent<Agent>();
    }
}
