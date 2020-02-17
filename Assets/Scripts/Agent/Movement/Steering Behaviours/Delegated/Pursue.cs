using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursue : Seek
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    protected Agent _pursueTarget;

    [SerializeField] protected float _maxPrediction;

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    protected virtual void Start()
    {
        _pursueTarget = _target;
        GameObject obj = new GameObject(this.name + " invisible target (PURSUE)");
        _target = obj.AddComponent<Agent>();
    }

    public override Steering GetSteering(AgentNPC agent)
    {
        if (_pursueTarget == null) return new Steering();

        // 1. Calculate the target to delegate to seek

        // Work out the distance to target
        Vector3 direction = _pursueTarget.Position - agent.Position;
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
        _target.Position = _pursueTarget.Position;
        _target.Position += _pursueTarget.Velocity * prediction;

        // 2. Delegate to seek
        return base.GetSteering(agent);
    }

    private void OnDestroy()
    {
        if (_target != null)
        {
            Destroy(_target.gameObject);
        }
    }

    void OnDrawGizmos()
    {
        if(_target != null) Gizmos.DrawWireSphere(_target.Position, 1f);
    }

    protected override void ChangeTarget(Agent agent)
    {
        if (_target != null) Destroy(_target);
        _pursueTarget = agent;
        GameObject obj = new GameObject(this.name + " invisible target (PURSUE)");
        _target = obj.AddComponent<Agent>();
    }
}
