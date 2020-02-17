using TMPro;
using UnityEngine;

/// <summary>
/// Class that represents a physical body
/// </summary>
public class Body : MonoBehaviour
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////
    
    [Header("Body")]
    
    /// <summary>
    /// Maximum linear acceleration
    /// </summary>
    [SerializeField] protected float _maxAcceleration;

    /// <summary>
    /// Maximum linear speed
    /// </summary>
    [SerializeField] protected float _maxSpeed;

    /// <summary>
    /// Maximum angular speed
    /// </summary>
    [SerializeField] protected float _maxRotation;

    /// <summary>
    /// Maximum angular acceleration
    /// </summary>
    [SerializeField] protected float _maxAngularAcceleration;


    /// <summary>
    /// Angular speed of the body
    /// </summary>
    protected float _rotation = 0f;
    
    /// <summary>
    /// Holds the static information of the body
    /// </summary>
    protected Static _static = new Static();

    /// <summary>
    /// Velocity vector of the body
    /// </summary>
    protected Vector3 _velocity = Vector3.zero;




    ///////////////////////////////////////////////////
    ///////////////////// ACCESS //////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Gets the maximum acceleration.
    /// </summary>
    /// <value>
    /// The maximum acceleration.
    /// </value>
    public float MaxAcceleration
    {
        get { return _maxAcceleration; }
        set { _maxAcceleration = value; }
    }

    /// <summary>
    /// Gets the maximum speed.
    /// </summary>
    /// <value>
    /// The maximum speed.
    /// </value>
    public float MaxSpeed
    {
        get { return _maxSpeed; }
        set { _maxSpeed = value; }
    }

    /// <summary>
    /// Gets the maximum rotation.
    /// </summary>
    /// <value>
    /// The maximum rotation.
    /// </value>
    public float MaxRotation
    {
        get { return _maxRotation; }
    }

    /// <summary>
    /// Gets the maximum angular acceleration.
    /// </summary>
    /// <value>
    /// The maximum angular acceleration.
    /// </value>
    public float MaxAngularAcceleration 
    {
        get { return _maxAngularAcceleration; }
    }
  
    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    /// <value>
    /// The position.
    /// </value>
    public Vector3 Position
    {
        get { return _static.Position; }
        set { _static.Position = value; }
    }

    /// <summary>
    /// Gets or sets the orientation.
    /// </summary>
    /// <value>
    /// The orientation.
    /// </value>
    public float Orientation
    {
        get { return _static.Orientation; }
        set { _static.Orientation = value;}
    }

    /// <summary>
    /// Gets the velocity.
    /// </summary>
    /// <value>
    /// The velocity.
    /// </value>
    public Vector3 Velocity
    {
        get { return _velocity; }
        set { if (value.magnitude <= MaxSpeed) { _velocity = value; } ; }
    }

    /// <summary>
    /// Gets the rotation.
    /// </summary>
    /// <value>
    /// The rotation.
    /// </value>
    public float Rotation
    {
        get { return _rotation; }
    }


    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    protected virtual void OnEnable()
    {
        Position = transform.position;
    }


    protected virtual void Update()
    {  
        transform.rotation = new Quaternion();
        // Change orientation
        transform.Rotate(Vector3.forward, Orientation, Space.World);

        transform.position = Vector3.zero;
        transform.Translate(Position, Space.World);     
    }
}

/// <summary>
/// Class representing the static information of a body
/// </summary>
[System.Serializable]
public class Static
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////
    
    /// <summary>
    /// Current position of the body
    /// </summary>
    private Vector3 _position;

    /// <summary>
    /// Current orientation of the body in degrees
    /// </summary>
    private float _orientation;

    ///////////////////////////////////////////////////
    ///////////////////// ACCESS //////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    /// <value>
    /// The position.
    /// </value>
    public Vector3 Position
    {
        get { return _position; }
        set { _position = value; }
    }

    /// <summary>
    /// Gets or sets the orientation.
    /// </summary>
    /// <value>
    /// The orientation.
    /// </value>
    public float Orientation
    {
        get { return _orientation; }
        set { _orientation = value; }
    }

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="Static"/> class.
    /// </summary>
    public Static()
    {
        _orientation = 0f;
        _position = Vector3.zero;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Static"/> class.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="orientation">The orientation.</param>
    public Static(Vector3 position, float orientation)
    {
        _orientation = orientation;
        _position = position;
    }
}
