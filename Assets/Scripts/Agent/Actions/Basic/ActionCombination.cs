using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class ActionCombination : Action
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Holds the sub-actions
    /// </summary>
    private List<Action> _actions;


    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////


    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCombination" /> class.
    /// </summary>
    /// <param name="expiryTime">The expiry time.</param>
    /// <param name="priority">The priority.</param>
    /// <param name="agent">The agent.</param>
    /// <param name="actions">The actions.</param>
    public ActionCombination(float expiryTime, int priority, AgentNPC agent, List<Action> actions) : base(expiryTime, priority, agent)
    {
        _actions = actions;
    }

    /// <summary>
    /// Determines whether this action can interrupt.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance can interrupt; otherwise, <c>false</c>.
    /// </returns>
    public override bool CanInterrupt()
    {
        //  We can interrupt if any of our sub-actions can
        foreach (Action action in _actions)
        {
            if (action.CanInterrupt()) return true;
        }

        return false;
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
        //  We can do both if all of our sub-actions can
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
        foreach (Action action in _actions)
        {
            if (!action.IsComplete()) return false;
        }

        return true;
    }

    /// <summary>
    /// Executes this action.
    /// </summary>
    public override void Execute()
    {
        // Execute all of our sub - actions
        foreach (Action action in _actions)
        {
            action.Execute();
        }
    }

    /// <summary>
    /// Cancels this action.
    /// </summary>
    public override void Cancel()
    {
        // Cancel all of our sub - actions
        foreach (Action action in _actions)
        {
            action.Cancel();
        }
    }
}
