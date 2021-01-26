using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;


/// <summary>
/// Non playable agent that moves depending of its behaviour attached
/// </summary>
[RequireComponent(typeof(Path))]
public class AgentNpc : Agent, IPathfinder, ISelectable, IFormable
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Result steering used to move the agent
    /// </summary>
    private Steering _steering;

    /// <summary>
    /// Holds an ordered by priority list of sets of weighted behaviours
    /// </summary>
    private SortedDictionary<int, List<SteeringBehaviour>> _orderedBehaviourList;


    [SerializeField] private TextMeshPro _stateText;
    [SerializeField] private GameObject _textPivot;

    [Header("NPC Properties")]
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    private GameObject _selectionObject;

    /// <summary>
    /// The NPC properties
    /// </summary>
    [SerializeField] private NPCProperties _npcProperties;

    /// <summary>
    /// The faction
    /// </summary>
    [SerializeField] private NPCProperties.Faction _faction;

    /// <summary>
    /// The pathfinding algorithm
    /// </summary>
    [SerializeField] private PathfindingAlgorithm _pathfindingAlgorithm = PathfindingAlgorithm.A_STAR;
    

    [Header("Priorities")]
    /// <summary>
    /// Holds the epsilon parameter, should be a small value
    /// </summary>
    [SerializeField]
    private float _epsilon = 0.01f;


    /// <summary>
    /// The formation
    /// </summary>
    private FormationManager _formation;


    /// <summary>
    /// The action manager
    /// </summary>
    private ActionManager _actionManager;

    private Quaternion _textRotation;

    /// <summary>
    /// The debug path
    /// </summary>
    protected Path _debugPath;

    protected GameObject _respawnParticles;


    ///////////////////////////////////////////////////
    ///////////////////// ACCESS //////////////////////
    ///////////////////////////////////////////////////


    /// <summary>
    /// Gets or sets a value indicating whether this instance is selected.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
    /// </value>
    public bool IsSelected
    {
        get
        {
            if (_selectionObject == null) return false;
            return _selectionObject.activeInHierarchy;
        }

        set
        {
            if (_selectionObject == null) return;
            _selectionObject.SetActive(value);
        }
    }


    /// <summary>
    /// Gets or sets the formation.
    /// </summary>
    /// <value>
    /// The formation.
    /// </value>
    public FormationManager Formation
    {
        get { return _formation; }

        set { _formation = value; }
    }


    /// <summary>
    /// Gets the faction.
    /// </summary>
    /// <value>
    /// The faction.
    /// </value>
    public NPCProperties.Faction Faction
    {
        get { return _faction; }
    }


    /// <summary>
    /// Gets the NPC properties.
    /// </summary>
    /// <value>
    /// The NPC properties.
    /// </value>
    public NPCProperties NpcProperties
    {
        get { return _npcProperties; }
        set { _npcProperties = value; }
    }

    /// <summary>
    /// Gets the current map.
    /// </summary>
    /// <value>
    /// The current map.
    /// </value>
    public Map CurrentMap
    {
        get { return _map; }
    }

    /// <summary>
    /// Gets the attack cooldown.
    /// </summary>
    /// <value>
    /// The attack cooldown.
    /// </value>
    public float AttackCooldown
    {
        get { return _npcProperties.AttackCooldown; }
    }

    /// <summary>
    /// Gets the attack range.
    /// </summary>
    /// <value>
    /// The attack range.
    /// </value>
    public float AttackRange
    {
        get { return _npcProperties.AttackRange; }
    }

    public float PerceptionRadius 
    {
        get { return _npcProperties.PerceptionRadius; }
    }

    public PathfindingAlgorithm PathfindingAlgorithm
    {
        get { return _pathfindingAlgorithm; }
    }

    private Agent AdversaryBase
    {
        get
        {
            String strTag = (Faction == NPCProperties.Faction.ALLY) ? "EnemyBase" : "AllyBase";
            return GameObject.FindGameObjectWithTag(strTag).GetComponent<Agent>();
        }
    }

    private Agent MyBase 
    {
        get 
        {
            String strTag = (Faction == NPCProperties.Faction.ENEMY) ? "EnemyBase" : "AllyBase";
            return GameObject.FindGameObjectWithTag(strTag).GetComponent<Agent>();
        }
    }

    public NPCProperties.NPCType Type 
    {
        get {
            return _npcProperties.Type;
        }
    }

    /// <summary>
    /// Gets or sets the debug path.
    /// </summary>
    /// <value>
    /// The debug path.
    /// </value>
    public Path DebugPath 
    {
        get { return _debugPath; }
        set 
        {
            _debugPath = value;
            if (_debugPath == null) return;
            _lineRenderer.positionCount = value.NodePath.Count + 1;
            _lineRenderer.SetPosition(0, _map.GridToWorld(MapPosition));
            for (int i = 1; i < _lineRenderer.positionCount; i++)
            {
                _lineRenderer.SetPosition(i, value.NodePath[i - 1].transform.position);
            }
        }
    }

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        _maxHealth = _npcProperties.MaxHealth;
        _health = _maxHealth;
        _steering = new Steering();
        _respawnParticles = Resources.Load<GameObject>("Effects/RespawnParticle");

        Color blueColor = new Color(0f, Random.Range(0.5f, 1f), Random.Range(0.8f, 1f), 0.5f);
        Color redColor = new Color(Random.Range(0.8f, 1f), 0f, Random.Range(0.25f, 0.5f), 0.5f);

        _lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
        _lineRenderer.startColor = _lineRenderer.endColor =
            (_faction == NPCProperties.Faction.ALLY) ? blueColor : redColor;
        _lineRenderer.startWidth = _lineRenderer.endWidth = 0.33f;
        _debugPath = GetComponent<Path>();


        SteeringBehaviour[] listSteeringBehaviour = GetComponents<SteeringBehaviour>();
        _orderedBehaviourList = new SortedDictionary<int, List<SteeringBehaviour>>();
        foreach (SteeringBehaviour behaviour in listSteeringBehaviour)
        {
            if (_orderedBehaviourList.ContainsKey(behaviour.Priority))
            {
                _orderedBehaviourList[behaviour.Priority].Add(behaviour);
            }
            else
            {
                _orderedBehaviourList.Add(behaviour.Priority, new List<SteeringBehaviour> {behaviour});
            }
        }
    }

    protected void Awake() 
    {
        // Check this and LateUpdate()
        // This is needed so the children text won't rotate along the parent
        if (_textPivot != null) _textRotation = _textPivot.transform.rotation;

        _actionManager = new ActionManager();
    }

    /// <summary>
    /// Arbitrator that uses weighted blending
    /// </summary>
    /// <param name="steering">New steering to blend</param>
    /// <param name="weight">Weight of the steering</param>
    private void SetSteering(Steering steering, float weight)
    {
        _steering.Linear += (weight * steering.Linear);
        _steering.Angular += (weight * steering.Angular);
    }

    /// <summary>
    /// Actuator
    /// </summary>
    protected void ApplySteering()
    {
        Position += _velocity * Time.deltaTime;
        Orientation += _rotation * Time.deltaTime;
        _velocity += (_steering.Linear * Time.deltaTime);
        _rotation += _steering.Angular * Time.deltaTime;

        // Actual speed depends of terrain

        TerrainType type = _map.Get(MapPosition.x, MapPosition.y);
        float terrainMult = GetTerrainFactor(type);
        float terrainMaxSpeed = MaxSpeed * terrainMult;

        if ((_steering.Linear == Vector3.zero))
        {
            // Steerings are totally 0, quickly try to stop
            _velocity -= _velocity / 20f;
        }

        if (Math.Abs(_steering.Angular) < 0.01f)
        {
            _rotation -= _rotation / 20f;
        }

        if (_velocity.magnitude > terrainMaxSpeed)
        {
            _velocity.Normalize();
            _velocity *= terrainMaxSpeed;
        }

        if (_rotation > MaxRotation)
        {
            _rotation = MaxRotation;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        DrawDebug();

        _actionManager.Execute();

        // Go through each group
        foreach (int priority in _orderedBehaviourList.Keys)
        {
            // Create the steering structure for accumulation
            _steering = new Steering();
            foreach (SteeringBehaviour behaviour in _orderedBehaviourList[priority])
            {
                Steering str = behaviour.GetSteering(this);
                SetSteering(str, behaviour.Weight);
            }

            // Check if we're above the threshold, if so return
            if (_steering.Linear.magnitude > _epsilon || Mathf.Abs(_steering.Angular) > _epsilon)
            {
                break;
            }
        }

        // When we get here, it means that either one group had a large enough acceleration
        // or we are returning the small acceleration from the final group
        ApplySteering();
        if(_stateText != null) _stateText.gameObject.SetActive(_debugController.ShowDebug);
    }

    private void LateUpdate()
    {
        if(_textPivot != null) _textPivot.transform.rotation = _textRotation;
    }

    /// <summary>
    /// Sets a target to a steering behaviour
    /// </summary>
    /// <typeparam name="T">Type of steering behaviour</typeparam>
    /// <param name="target"></param>
    public void SetTarget<T>(Agent target) where T : SteeringBehaviour
    {
        SteeringBehaviour st = GetComponent<T>();
        if (st != null)
            st.Target = target;
    }

    public float GetAttackDamage()
    {
        return _npcProperties.AttackDamage;
    }


    /// <summary>
    /// Gets the terrain speed factor.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public float GetTerrainFactor(TerrainType type)
    {
        return _npcProperties.GetTerrainSpeed(type);
    }

    /// <summary>
    /// Gets the terrain speed factor of a cell in the map.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns></returns>
    public float GetTerrainFactor(Vector2Int position)
    {
        return _npcProperties.GetTerrainSpeed(_map.Get(position));
    }


    /// <summary>
    /// Gets the heuristic function.
    /// </summary>
    /// <returns></returns>
    public Func<Vector2Int, Vector2Int, float> GetHeuristicFunc()
    {
        return _npcProperties.GetHeuristicFunc();
    }


    /// <summary>
    /// Gets the attack factor.
    /// </summary>
    /// <param name="npcType">Type of the NPC.</param>
    /// <returns></returns>
    public float GetAttackFactor(NPCProperties.NPCType npcType)
    {
        return _npcProperties.GetAttackFactor(npcType);
    }


    /// <summary>
    /// Gets the defense factor.
    /// </summary>
    /// <param name="npcType">Type of the NPC.</param>
    /// <returns></returns>
    public float GetDefenseFactor(NPCProperties.NPCType npcType)
    {
        return _npcProperties.GetDefenseFactor(npcType);
    }

    public float GetAttackFactor(Vector2Int position)
    {
        return _npcProperties.GetTerrainAttackFactor(_map.Get(MapPosition));
    }

    public float GetDefenseFactor(Vector2Int position)
    {
        return _npcProperties.GetTerrainDefenseFactor(_map.Get(MapPosition));
    }

    public float GetAttackDefenseFactor(NPCProperties.NPCType type)
    {
        return GetAttackFactor(type) / GetDefenseFactor(type);
    }


    /// <summary>
    /// Joins the formation.
    /// </summary>
    /// <param name="formation">The formation.</param>
    public void JoinFormation(FormationManager formation)
    {
        AbandonFormation();
        CancelPathFollowing();
        _formation = formation;
        _formation.AddAgent(this);
    }

    /// <summary>
    /// Abandons the formation.
    /// </summary>
    public void AbandonFormation()
    {
        if (_formation == null) return;


        _formation.RemoveAgent(this);
        _formation = null;
    }


    /// <summary>
    /// Cancels the path following.
    /// </summary>
    public void CancelPathFollowing()
    {
        PathFollowing pf = gameObject.GetComponent<PathFollowing>();
        if (pf != null)
        {
            pf.Reset();
        }
    }

    /// <summary>
    /// Schedules the action.
    /// </summary>
    /// <param name="action">The action.</param>
    public void ScheduleAction(Action action)
    {
        _actionManager.ScheduleAction(action);
    }

    private void OnDestroy()
    {
    }

    public void Attack(Agent target)
    {
        float baseDamage = GetAttackDamage();
        float attackDefenseFactor = 1f;
        float terrainADF = 1f;

        AgentNpc oppNpc = target as AgentNpc;
        if (oppNpc != null)
        {
            NPCProperties.NPCType oppType = oppNpc.NpcProperties.Type;
            NPCProperties.NPCType myType = NpcProperties.Type;
            attackDefenseFactor = GetAttackDefenseFactor(oppType);
            terrainADF = GetAttackFactor(MapPosition) / oppNpc.GetDefenseFactor(MapPosition);
        }

        float finalDamage = Mathf.Max(1f, Random.Range(0.75f, 1.25f) * baseDamage * attackDefenseFactor * terrainADF);

        if (Vector3.Distance(Position, target.Position) < AttackRange)
        {
            target.Damage(finalDamage);
        }
            
    }

    public void ChangeModeText(string newText)
    {
        if (_stateText != null) _stateText.SetText(newText);
    }

    public void ChangeModeColor(Color color)
    {
        if (_stateText != null) _stateText.color = color;
    }

    public Agent GetBestEnemyInRange()
    {
        String strTag = (Faction == NPCProperties.Faction.ALLY) ? "EnemyAgent" : "AllyAgent";
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(strTag);
        Agent easiestMatchup = null;

        Agent baseToAttack = AdversaryBase;

        if (Vector3.Distance(Position, baseToAttack.Position) < PerceptionRadius) {
            return baseToAttack;
        }

        float adf = 0f;
        float maxAdf = Mathf.NegativeInfinity;
        foreach (GameObject enemy in enemies) {
            AgentNpc enemyAgent = enemy.GetComponent<AgentNpc>();
            if ((Vector3.Distance(Position, enemyAgent.Position) < PerceptionRadius)) {
                // Gets the type ADF factor
                adf = GetAttackDefenseFactor(enemyAgent.Type);
                // Gets the terrain ADF factor
                adf *= GetAttackFactor(MapPosition) / GetDefenseFactor(enemyAgent.MapPosition);
                if (adf >= maxAdf) {
                    maxAdf = adf;
                    easiestMatchup = enemyAgent;
                }
            }
        }

        return easiestMatchup;
    }

    protected override void DeathAction()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().Respawn(this, 15f);
        gameObject.transform.position = Position = MyBase.Position;
        _health = _maxHealth;
        gameObject.SetActive(false);
    }

    public void Respawn() {
        GameObject particles = Instantiate(_respawnParticles);
        particles.transform.position = Position;
    }


    /// <summary>
    /// Draws the debug lines.
    /// </summary>
    private void DrawDebug()
    {
        if (_debugPath == null)
        {
            _lineRenderer.enabled = false;
            return;
        }

        if (_lineRenderer != null) _lineRenderer.enabled = _debugController.ShowDebug;
    }
}

public enum PathfindingAlgorithm
{
    A_STAR,
    LRTA_STAR
}