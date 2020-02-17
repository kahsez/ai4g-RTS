using UnityEngine;


public class Seek : SteeringBehaviour
{
    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////
    
    public override Steering GetSteering(AgentNPC agent)
    {
        if (_target == null) return new Steering();

        // Create the structure to hold our output
        Steering steering = new Steering();

        // Get the direction to the target
        steering.Linear = _target.Position - agent.Position;

        /* Millington algorithm
        // Give full acceleration along this direction
        steering.Linear.Normalize();
        steering.Linear *= agent.MaxAcceleration;
        */

        /* Reynolds implementation */
        Vector3 desiredVelocity = _target.Position - agent.Position;
        desiredVelocity = desiredVelocity.normalized * agent.MaxSpeed;
        steering.Linear = desiredVelocity - agent.Velocity;    

        // Output the steering
        steering.Angular = 0f;
        return steering;
    }

}
