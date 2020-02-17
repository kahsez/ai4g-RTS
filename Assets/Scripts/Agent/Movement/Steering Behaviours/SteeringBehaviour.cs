using UnityEngine;


/// <summary>
/// Behaviour that will determine a steering
/// </summary>
public abstract class SteeringBehaviour : MonoBehaviour
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Weight to use multiple steerings
    /// </summary>
    [SerializeField] protected float _weight = 1;

    /// <summary>
    /// Priority to use multiple steerings
    /// </summary>
    [SerializeField] protected int _priority = 0;

    /// <summary>
    /// Target of the behaviour
    /// </summary>
    [SerializeField] protected Agent _target;


    ///////////////////////////////////////////////////
    ///////////////////// ACCESS //////////////////////
    ///////////////////////////////////////////////////

    public float Weight
    {
        get { return _weight; }
        set { _weight = value; }
    }

    public int Priority 
    {
        get { return _priority; }
        set { _priority = value; }
    }

    public Agent Target
    {
        get { return _target; }
        set { ChangeTarget(value); }
    }


    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Get a steering depending on the behaviour.
    /// </summary>
    /// <param name="agent">Agent that will receive the steering</param>
    /// <returns>Resulting steering</returns>
    public abstract Steering GetSteering(AgentNPC agent);

    /// <summary>
    /// Changes the target
    /// </summary>
    /// <param name="target"></param>
    protected virtual void ChangeTarget(Agent target)
    {
        _target = target;
    }
}
