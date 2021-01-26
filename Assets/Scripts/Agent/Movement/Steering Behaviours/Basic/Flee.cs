using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Flee : SteeringBehaviour
{
    [SerializeField] private float _fleeDistance = 10;

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    public override Steering GetSteering(AgentNpc agent)
    {
        if (_target == null) return new Steering();

        Steering steering = new Steering();

        // Get the direction to the target
        steering.Linear = agent.Position - _target.Position;

        if (steering.Linear.sqrMagnitude > _fleeDistance * _fleeDistance)
        {
            return new Steering();
        }

        // The velocity is along this direction, at full speed
        steering.Linear.Normalize();
        steering.Linear *= agent.MaxAcceleration;

        // Output the steering
        steering.Angular = 0f;
        return steering;
    }
}
