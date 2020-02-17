using UnityEngine;

/// <summary>
/// 
/// </summary>
public abstract class Action
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// The expiry time
    /// </summary>
    protected float _expiryTime;

    /// <summary>
    /// The expiry timestamp
    /// </summary>
    protected float _expiryTimestamp;

    /// <summary>
    /// The priority
    /// </summary>
    protected int _priority;

    /// <summary>
    /// The is completed
    /// </summary>
    protected bool _isCompleted;

    /// <summary>
    /// The agent that performs the action
    /// </summary>
    protected AgentNPC _agent;

    /// <summary>
    /// Determines if the action has started
    /// </summary>
    protected bool _started;


    ///////////////////////////////////////////////////
    ///////////////////// ACCESS //////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Gets the expiry time.
    /// </summary>
    /// <value>
    /// The expiry time.
    /// </value>
    public float ExpiryTime 
    {
        get { return _expiryTime; }
    }

    /// <summary>
    /// Gets the priority.
    /// </summary>
    /// <value>
    /// The priority.
    /// </value>
    public int Priority 
    {
        get { return _priority; }
    }

    /// <summary>
    /// Gets a value indicating whether this action has expired.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this action has expired; otherwise, <c>false</c>.
    /// </value>
    public bool HasExpired 
    {
        get { return (Time.time > _expiryTimestamp + _expiryTime); }
    }

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////


    /// <summary>
    /// Initializes a new instance of the <see cref="Action" /> class.
    /// </summary>
    /// <param name="expiryTime">The expiry time.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="agent">The agent.</param>
    protected Action(float expiryTime, int priority, AgentNPC agent)
    {
        _expiryTime = expiryTime;
        _priority = priority;
        _isCompleted = false;
        _agent = agent;
        _started = false;
    }

    /// <summary>
    /// Determines whether this action can interrupt.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance can interrupt; otherwise, <c>false</c>.
    /// </returns>
    public abstract bool CanInterrupt();

    /// <summary>
    /// Determines whether this action can be done with the specified other action.
    /// </summary>
    /// <param name="otherAction">The other action.</param>
    /// <returns>
    ///   <c>true</c> if this instance can be done with the specified other action; otherwise, <c>false</c>.
    /// </returns>
    public abstract bool CanDoBoth(Action otherAction);

    /// <summary>
    /// Determines whether this action is complete.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this action is complete; otherwise, <c>false</c>.
    /// </returns>
    public abstract bool IsComplete();

    /// <summary>
    /// Executes this action.
    /// </summary>
    public abstract void Execute();

    /// <summary>
    /// Sets the expiry timestamp.
    /// </summary>
    /// <param name="time">The time.</param>
    public void SetExpiryTimestamp(float time)
    {
        _expiryTimestamp = time;
    }

    public abstract void Cancel();
}
