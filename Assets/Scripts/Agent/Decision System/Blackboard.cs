using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////
    
    private List<BlackboardTask> _entries;

    //private List<BlackboardTask> _passedTasks;

    public List<BlackboardTask> Entries
    {
        get { return _entries; }
    }

    public Blackboard()
    {
        _entries = new List<BlackboardTask>();
        //_passedTasks = new List<BlackboardTask>();
    }

    public void InsertTask(BlackboardTask task)
    {
        _entries.Add(task);
    }

    public void PassTask(BlackboardTask task)
    {
        int i;
        for (i = 0; i < _entries.Count; i++)
        {
            if (_entries[i].Target == task.Target && _entries[i].Task == task.Task)
            {
                break;
            }
        }

        _entries.RemoveAt(i);
    }

}


public class BlackboardTask
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    private NpcMode _task;

    private Vector2Int _target;


    public NpcMode Task
    {
        get { return _task; }
    }

    public Vector2Int Target 
    {
        get { return _target; }
    }

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="BlackboardTask"/> class.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <param name="target">The target.</param>
    public BlackboardTask(NpcMode task, Vector2Int target)
    {
        _task = task;
        _target = target;
    }
}


