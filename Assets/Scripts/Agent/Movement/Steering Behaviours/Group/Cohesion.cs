using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohesion : Seek
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

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    void Start()
    {
        _targets = GameObject.FindGameObjectsWithTag("Agent");
        _target = new GameObject(this.name + " invisible target (COHESION)").AddComponent<Agent>();
    }


    public override Steering GetSteering(AgentNPC agent)
    {
        int count = 0;
        Vector3 centerOfMass = Vector3.zero;

        // Loop through each target
        foreach (GameObject target in _targets)
        {
            if (target != this.gameObject)
            {
                // Check if the target is close
                Vector3 direction = agent.Position - target.transform.position;
                float sqrDistance = direction.sqrMagnitude;

                if (sqrDistance < _threshold * _threshold)
                {
                    centerOfMass += target.transform.position;
                    count++;
                }
            }
        }

        if (count == 0)
        {
            //centerOfMass = agent.transform.position;
            //count++
            return new Steering();
        }

        centerOfMass /= count;
        _target.Position = centerOfMass;

        return base.GetSteering(agent);
    }

    private void OnDestroy()
    {
        if (_target != null)
        {
            Destroy(_target.gameObject);
        }
    }
}
