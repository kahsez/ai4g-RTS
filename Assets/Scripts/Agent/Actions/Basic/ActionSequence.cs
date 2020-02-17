using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class ActionSequence : Action
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Holds the sub-actions
    /// </summary>
    private List<Action> _actions;

    /// <summary>
    /// Holds the index of the currently executing sub-action
    /// </summary>
    private int _activeIndex;

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionSequence"/> class.
    /// </summary>
    /// <param name="expiryTime">The expiry time.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="actions">The actions.</param>
    public ActionSequence(float expiryTime, int priority, AgentNPC agent, List<Action> actions) : base(expiryTime, priority, agent)
    {
        _actions = actions;
        _activeIndex = 0;
    }

    /// <summary>
    /// Determines whether this action can interrupt.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance can interrupt; otherwise, <c>false</c>.
    /// </returns>
    public override bool CanInterrupt()
    {
        //  We can interrupt if our first sub-actions can
        return _actions[0].CanInterrupt();
    }

    /// <summary>
    /// Determines whether this action can be done with the specified other action.
    /// </summary>
    /// <param name="otherAction">The other action.</param>
    /// <returns>
    ///   <c>true</c> if this instance can be done with the specified other action; otherwise, <c>false</c>.
    /// </returns>
    public override bool CanDoBoth(Action otherAction)
    {
        // We can do both actions if all of our sub-actions can
        foreach (Action action in _actions)
        {
            if (!action.CanDoBoth(otherAction)) return false;
        }

        return true;
    }

    /// <summary>
    /// Determines whether this action is complete.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this action is complete; otherwise, <c>false</c>.
    /// </returns>
    public override bool IsComplete()
    {
        // We are complete if all of our sub-actions are
        return _activeIndex >= _actions.Count;
    }

    /// <summary>
    /// Executes this action.
    /// </summary>
    public override void Execute()
    {
        // Execute our current action
        _actions[_activeIndex].Execute();

        // If our current action is complete, go to the next
        if (_actions[_activeIndex].IsComplete())
        {
            _activeIndex++;
        }
    }

    public override void Cancel()
    {
    }

}
