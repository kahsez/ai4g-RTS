using UnityEngine;


public class Arrive : SteeringBehaviour
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

    public override Steering GetSteering(AgentNPC agent)
    {
        if (_target == null) return new Steering();

        float targetSpeed;
        Vector3 targetVelocity;

        // Create the structure to hold our output
        Steering steering = new Steering();

        // Get the direction to the target
        Vector3 direction = _target.Position - agent.Position;
        float distance = direction.magnitude;

        // Check if we are there, return no steering
        if (distance < _target.InteriorRadius)
        {
            steering.Linear = -agent.Velocity / _timeToTarget;
            if (steering.Linear.magnitude > agent.MaxAcceleration)
            {
                steering.Linear.Normalize();
                steering.Linear *= agent.MaxAcceleration;
            }

            steering.Angular = 0f;
            return steering;
        }

        // If we are outside the slowRadius (exterior), then go max speed
        if (distance > _target.ExteriorRadius)
        {
            targetSpeed = agent.MaxSpeed;
        }
        else
        {
            // Otherwise calculate a scaled speed
            targetSpeed = (agent.MaxSpeed * distance) / agent.ExteriorRadius;
        }
			

        // The target velocity combines speed and direction
        targetVelocity = direction.normalized;
        targetVelocity *= targetSpeed;

        // Acceleration tries to get to the target velocity
        steering.Linear = targetVelocity - agent.Velocity;
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
