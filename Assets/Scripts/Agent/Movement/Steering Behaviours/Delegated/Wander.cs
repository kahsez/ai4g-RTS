using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : Face
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Forward offset of the wander
    /// </summary>
    [SerializeField] private float _wanderOffset = 5f;

    /// <summary>
    /// Radius of the wander
    /// </summary>
    [SerializeField] private float _wanderRadius = 1.5f;

    /// <summary>
    /// Holds the maximum rate at which the wander orientation can change
    /// </summary>
    [SerializeField] private float _wanderRate = 10f;

    /// <summary>
    /// Current orientation of the wander target
    /// </summary>
    private float _wanderOrientation = 0f;

    /// <summary>
    /// Allows to draw a gizmo representing the wander circle
    /// </summary>
    [SerializeField] private bool _DrawGizmos;

    private Vector3 wanderCircleCenter;


    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    protected override void Start()
    {
        _target = new GameObject(this.name + " invisible target (WANDER)").AddComponent<Agent>();
        _target.Position = transform.position;
        base.Start();     
    }

    public override Steering GetSteering(AgentNPC agent)
    {
        // 1. Calculate the target to delegate to face

        // Update wander orientation
        _wanderOrientation += MathAI.RandomBinomial() * _wanderRate;

        // Calculate the combined target orientation
        float targetOrientation = _wanderOrientation + agent.Orientation;

        // Calculate the center of the wander circle
        wanderCircleCenter = agent.Position + _wanderOffset * MathAI.OrientationAsVector(agent.Orientation);

        // Calculate the target location
        Vector3 targetPosition = wanderCircleCenter + _wanderRadius * MathAI.OrientationAsVector(targetOrientation);
        _faceTarget.Position = targetPosition;

        // 2. Delegate to face
        Steering steering = base.GetSteering(agent);

        // 3. Now set the linear acceleration to be at full acceleration in the direction of the orientation
        //steering.Linear = agent.MaxAcceleration * (targetPosition - transform.position).normalized;
        steering.Linear = agent.MaxAcceleration * MathAI.OrientationAsVector(agent.Orientation);

        if (_DrawGizmos)
        {           
            Debug.DrawRay(agent.Position, wanderCircleCenter - agent.Position);
            Debug.DrawRay(agent.Position, targetPosition - agent.Position, Color.red);
        }

        return steering;
    }

    void OnDrawGizmos()
    {
        if (_DrawGizmos)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(wanderCircleCenter, _wanderRadius);
        }

    }

    private void OnDestroy()
    {
        if (_target != null)
        {
            Destroy(_target.gameObject);
        }
    }
}
