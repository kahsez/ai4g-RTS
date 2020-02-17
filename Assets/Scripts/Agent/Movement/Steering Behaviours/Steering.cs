using UnityEngine;

/// <summary>
/// Steering
/// </summary>
public class Steering
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////
    
    /// <summary>
    /// Angular acceleration
    /// </summary>
    private float _angular;

    /// <summary>
    /// Linear acceleration
    /// </summary>
    private Vector3 _linear;

    ///////////////////////////////////////////////////
    ///////////////////// ACCESS //////////////////////
    ///////////////////////////////////////////////////

    public float Angular
    { 
        get { return _angular; }
        set { _angular = value; }
    }

    public Vector3 Linear
    { 
        get { return _linear; }
        set { _linear = value; }
    }

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Constructor
    /// </summary>
    public Steering()
    {
        _angular = 0.0f;
        _linear = Vector3.zero;
    }

}

