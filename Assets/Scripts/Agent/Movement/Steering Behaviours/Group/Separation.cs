using UnityEngine;

public class Separation : SteeringBehaviour
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Holds a list of potential targets
    /// </summary>
    private GameObject[] _targets;

    /// <summary>
    /// Holds the threshold to take action
    /// </summary>
    [SerializeField] private float _threshold;

    /// <summary>
    /// Holds the constant coefficient of decay for
    /// the inverse square law force
    /// </summary>
    [SerializeField] private float _decayCoefficient;

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    void Start()
    {
        _targets = GameObject.FindGameObjectsWithTag("Agent");
    }


    public override Steering GetSteering(AgentNPC agent)
    {
        // The steering variable holds the output
        Steering steering = new Steering();

        // Loop through each target
        foreach (GameObject target in _targets)
        {
            if (target != this.gameObject)
            {
                // Check if the target is close
                Vector3 direction = agent.transform.position - target.transform.position;
                float sqrDistance = direction.sqrMagnitude;

                if (sqrDistance < _threshold * _threshold)
                {
                    // Calculate the strength of repulsion
                    float strength = Mathf.Min(_decayCoefficient / sqrDistance, agent.MaxAcceleration);

                    // Add the acceleration
                    steering.Linear += strength * direction.normalized;
                }
            }        
        }

        // We've gone through all targets, return the result
        return steering;
    }
}
