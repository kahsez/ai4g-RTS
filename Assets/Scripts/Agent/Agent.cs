using TMPro;
using UnityEngine;

/// <summary>
/// Class that represents an agent
/// </summary>
public class Agent : Body
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////
    
    [Header("Perception")]
    
    /// <summary>
    /// The interior radius
    /// </summary>
    [SerializeField] protected float _interiorRadius = 2;

    /// <summary>
    /// The exterior radius
    /// </summary>
    [SerializeField] protected float _exteriorRadius = 5;

    /// <summary>
    /// The interior angle
    /// </summary>
    [SerializeField] protected float _interiorAngle = 5;

    /// <summary>
    /// The exterior angle
    /// </summary>
    [SerializeField] protected float _exteriorAngle = 20;

    [Header("Gizmos")]

    /// <summary>
    /// The show interior radius
    /// </summary>
    [SerializeField] protected bool _showInteriorRadius;

    /// <summary>
    /// The show exterior radius
    /// </summary>
    [SerializeField] protected bool _showExteriorRadius;

    /// <summary>
    /// The show interior angle
    /// </summary>
    [SerializeField] protected bool _showInteriorAngle;

    /// <summary>
    /// The show exterior angle
    /// </summary>
    [SerializeField] protected bool _showExteriorAngle;

    /// <summary>
    /// The angle length
    /// </summary>
    [SerializeField] protected float _angleLength = 3f;

    [Header("Ingame Debug")]

    /// <summary>
    /// The line renderer
    /// </summary>
    protected LineRenderer _lineRenderer;

    /// <summary>
    /// Health of the body
    /// </summary>
    protected float _health;

    /// <summary>
    /// The maximum health
    /// </summary>
    protected float _maxHealth;

    protected Map _map;
    protected DebugController _debugController;

    protected GameObject _healingParticles;
    protected GameObject _damageParticles;

    [SerializeField] protected TextMeshPro _healthText;

    ///////////////////////////////////////////////////
    ///////////////////// ACCESS //////////////////////
    ///////////////////////////////////////////////////


    /// <summary>
    /// Gets or sets the interior radius.
    /// </summary>
    /// <value>
    /// The interior radius.
    /// </value>
    public float InteriorRadius
    {
        get { return _interiorRadius; }
        set { _interiorRadius = value; }
    }

    /// <summary>
    /// Gets or sets the exterior radius.
    /// </summary>
    /// <value>
    /// The exterior radius.
    /// </value>
    public float ExteriorRadius
    {
        get { return _exteriorRadius; }
        set { _exteriorRadius = value; }
    }

    /// <summary>
    /// Gets or sets the interior angle.
    /// </summary>
    /// <value>
    /// The interior angle.
    /// </value>
    public float InteriorAngle
    {
        get { return _interiorAngle; }
        set { _interiorAngle = value; }
    }

    /// <summary>
    /// Gets or sets the exterior angle.
    /// </summary>
    /// <value>
    /// The exterior angle.
    /// </value>
    public float ExteriorAngle
    {
        get { return _exteriorAngle; }
        set { _exteriorAngle = value; }
    }

    /// <summary>
    /// Gets the health.
    /// </summary>
    /// <value>
    /// The health.
    /// </value>
    public float Health 
    {
        get { return _health; }
    }

    /// <summary>
    /// Gets the maximum health.
    /// </summary>
    /// <value>
    /// The maximum health.
    /// </value>
    public float MaxHealth
    {
        get { return _maxHealth; }
    }

    public float HealthPercent
    {
        get { return _health / _maxHealth; }
    }

    public float LostHealthPercent
    {
        get { return 1 - HealthPercent; }
    }

    /// <summary>
    /// Gets the map position.
    /// </summary>
    /// <value>
    /// The map position.
    /// </value>
    public Vector2Int MapPosition 
    {
        get
        {
            return _map.WorldToGrid(Position);             
        }
    }

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Draws the gizmo interior radius.
    /// </summary>
    private void DrawGizmoInteriorRadius()
    {
        if (!_showInteriorRadius) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Position, _interiorRadius);
    }

    /// <summary>
    /// Draws the gizmo exterior radius.
    /// </summary>
    private void DrawGizmoExteriorRadius()
    {
        if (!_showExteriorRadius) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Position, _exteriorRadius);
    }

    /// <summary>
    /// Draws the angle.
    /// </summary>
    /// <param name="angle">The angle.</param>
    /// <param name="color">The color.</param>
    private void DrawAngle(float angle, Color color)
    {
        Vector3 rotation = new Vector3(0, 0f, angle / 2f);
        Vector3 direction = Quaternion.Euler(rotation) * MathAI.OrientationAsVector(Orientation);
        direction.Normalize();
        Debug.DrawRay(Position, direction * _angleLength, color);
    }

    /// <summary>
    /// Draws the gizmo interior angle.
    /// </summary>
    private void DrawGizmoInteriorAngle()
    {
        if (!_showInteriorAngle) return;

        DrawAngle(_interiorAngle, Color.black);
        DrawAngle(-_interiorAngle, Color.black);
    }

    /// <summary>
    /// Draws the gizmo exterior angle.
    /// </summary>
    private void DrawGizmoExteriorAngle()
    {
        if (!_showExteriorAngle) return;

        DrawAngle(_exteriorAngle, Color.gray);
        DrawAngle(-_exteriorAngle, Color.gray);
    }

    void OnDrawGizmos()
    {
        DrawGizmoInteriorRadius();
        DrawGizmoExteriorRadius();
        DrawGizmoInteriorAngle();
        DrawGizmoExteriorAngle();
    }

    public void Heal(float healthPoints)
    {
        _health = Mathf.Min(_health + healthPoints, _maxHealth);
        GameObject particles = Instantiate(_healingParticles);
        particles.transform.position = Position;
    }

    public void Damage(float healthPoints)
    {
        _health = Mathf.Max(_health - healthPoints, 0f);

        GameObject particles = Instantiate(_damageParticles);
        particles.transform.position = Position;

        if (_health <= 0f)
        {
            DeathAction();
        }
    }

    protected virtual void DeathAction()
    { 
    }

    protected virtual void Start()
    {
        if (_healthText != null)
        {
            _healthText.color = Color.white;
        }

        _lineRenderer = gameObject.GetComponent<LineRenderer>();
        if (_lineRenderer != null)
        {
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.enabled = false;
        }
       
        _debugController = GameObject.FindGameObjectWithTag("DebugController").GetComponent<DebugController>();
        _healingParticles = Resources.Load<GameObject>("Effects/HealingParticle");
        _damageParticles = Resources.Load<GameObject>("Effects/DamageParticle");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
    }


    protected override void Update()
    {
        base.Update();
        if (_healthText != null)
        {
            if (_healthText != null) _healthText.SetText(Mathf.CeilToInt(_health) + "/" + _maxHealth);
        }
    }
}