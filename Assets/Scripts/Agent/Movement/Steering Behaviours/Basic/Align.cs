using UnityEngine;


public class Align : SteeringBehaviour
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

        float targetRotation = 0f;
        float angularAcceleration = 0f;

        // Create the structure to hold our output
        Steering steering = new Steering();

        // Get the naive direction to the target
        float rotation = _target.Orientation - agent.Orientation;

        // Map the result to the (-pi, pi) interval
        rotation = MathAI.MapToRange(rotation);
        float rotationSize = Mathf.Abs(rotation);

        // Check if we are there, return no steering
        if (rotationSize < agent.InteriorAngle)
        {   
            steering.Angular = -agent.Rotation / _timeToTarget;
            angularAcceleration = Mathf.Abs(steering.Angular);
            if (angularAcceleration > agent.MaxAngularAcceleration)
            {
                steering.Angular /= angularAcceleration;
                steering.Angular *= agent.MaxAngularAcceleration;
            }

            steering.Linear = Vector3.zero;
            
            return steering;
        }

        // If we are outside the slowAngle (exterior), then use max rotation
        if (rotationSize > agent.ExteriorAngle)
            targetRotation = agent.MaxRotation;
        // Otherwise calculate a scaled rotation
        else
            targetRotation = (agent.MaxRotation * rotationSize) / agent.ExteriorAngle;

        // The final target rotation combines speed (alredy in the variable) and direction

        targetRotation *= rotation / rotationSize;

        // Acceleration tries to get to the target rotation
        steering.Angular = targetRotation - agent.Rotation;
        steering.Angular /= _timeToTarget;
        
        // Check if the acceleration is too great
        angularAcceleration = Mathf.Abs(steering.Angular);
        if (angularAcceleration > agent.MaxAngularAcceleration)
        {
            steering.Angular /= angularAcceleration;
            steering.Angular *= agent.MaxAngularAcceleration;
        }

        // Output the steering
        steering.Linear = Vector3.zero;
        return steering;
    }
}
