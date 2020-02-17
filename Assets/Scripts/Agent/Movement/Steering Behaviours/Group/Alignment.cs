using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alignment : Align
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
        _target = new GameObject(this.name + " invisible target (ALIGNMENT)").AddComponent<Agent>();
    }


    public override Steering GetSteering(AgentNPC agent)
    {
        int count = 0;
        Vector3 heading = Vector3.zero;

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
                    float orientation = target.GetComponent<Agent>().Orientation;
                    heading += MathAI.OrientationAsVector(orientation);
                    count++;
                }
            }
        }

        if (count > 0)
        {
            heading /= count;
            heading -= MathAI.OrientationAsVector(agent.Orientation);
        }     
        
        _target.Orientation = Mathf.Atan2(heading.y, heading.x) * Mathf.Rad2Deg;

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
