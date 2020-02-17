using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class ActionManager
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Holds the queue of pending actions
    /// </summary>
    private SimplePriorityQueue<Action> _queue;

    /// <summary>
    /// The currently executing actions
    /// </summary>
    private SimplePriorityQueue<Action> _active;

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionManager"/> class.
    /// </summary>
    public ActionManager()
    {
        _queue = new SimplePriorityQueue<Action>();
        _active = new SimplePriorityQueue<Action>();
    }


    /// <summary>
    /// Adds an action to the queue.
    /// </summary>
    /// <param name="action">The action.</param>
    public void ScheduleAction(Action action)
    {
        _queue.Enqueue(action, action.Priority);
        action.SetExpiryTimestamp(Time.time);
    }


    /// <summary>
    /// Processes the manager.
    /// </summary>
    public void Execute()
    {
        // Check if we need to interrupt the currently active actions
        CheckInterrupts();

        // Add as many actions to the active set as play with the
        // actions already there.
        AddAllToActive();

        // Execute the active set
        ExecuteActive();
    }


    /// <summary>
    /// Go through the queue to find interrupters
    /// </summary>
    private void CheckInterrupts()
    {
        if (_active.Count == 0) return;
        List<Action> markedToRemove = new List<Action>();
        foreach (Action action in _queue)
        {
            // If we drop below active priority, give up
            if (action.Priority > _active.First.Priority)
                break;

            // If we have an interrupter, interrupt
            if (action.CanInterrupt())
            {
                foreach (Action act in _active)
                {
                    act.Cancel();
                }

                _active.Clear();
                _active.Enqueue(action, action.Priority);
                markedToRemove.Add(action);
            }
        }

        foreach (Action action in markedToRemove)
        {
            _queue.Remove(action);
        }
    }

    /// <summary>r possible
    /// </summary>
    private void AddAllToActive()
    {
        List<Action> markedToRemove = new List<Action>();
        foreach (Action action in _queue)
        {
            // Check if the action has timed out
            if (action.HasExpired)
            {
                //  Remove it from the queue
                markedToRemove.Add(action);
            }
            else
            {
                if (_active.Count <= 0)
                {
                    markedToRemove.Add(action);
                    _active.Enqueue(action, action.Priority);
                }
                else
                {
                    // Check if we can combine
                    foreach (Action activeAction in _active)
                    {
                        if (action.CanDoBoth(activeAction))
                        {
                            // Move the action to the active set
                            markedToRemove.Add(action);
                            _active.Enqueue(action, action.Priority);
                        }
                    }
                }
            }
        }

        foreach (Action action in markedToRemove)
        {
            _queue.Remove(action);
        }
    }

    /// <summary>
    /// Process the active set
    /// </summary>
    private void ExecuteActive()
    {
        List<Action> markedToRemove = new List<Action>();
        foreach (Action action in _active)
        {
            // Remove completed actions
            if (action.IsComplete())
            {
                markedToRemove.Add(action);
            }
            // Execute others
            else
            {
                action.Execute();
            }                
        }

        foreach (Action action in markedToRemove)
        {
            _active.Remove(action);
        }
    }
}