using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityMatching : SteeringBehaviour
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Holds the time over which to achieve target speed
    /// </summary>
    [SerializeField] private float _timeToTarget = 0.25f;

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    public override Steering GetSteering(AgentNpc agent)
    {
        if (_target == null) return new Steering();

        // Create the structure to hold our output
        Steering steering = new Steering();

        // Acceleration tries to get to the target velocity
        steering.Linear = _target.Velocity - agent.Velocity;
        steering.Linear /= _timeToTarget;
        
        // Check if the acceleration is too fast
        if (steering.Linear.magnitude > agent.MaxAcceleration)
        {
            steering.Linear.Normalize();
            steering.Linear *= agent.MaxAcceleration;
        }

        // Output the steering
        steering.Angular = 0f;
        return steering;
    }
}
