using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NpcMode
{
    OFFENSE,
    DEFENSE
}

[RequireComponent(typeof(AgentNPC))]
public class TacticDecisionSystem : MonoBehaviour
{
    public enum NpcState
    {
        OFFENSE_ATTACK,
        DEFENSE_ATTACK,
        OFFENSE_CHASE,
        DEFENSE_CHASE,
        RETREAT,
        PATROL,
        DEFEND,
        ADVANCE
    }

    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    private AgentNPC _agent;

    private NpcMode _mode;

    private NpcState _state;

    private float _timer;

    private InfluenceMap _influenceMap;

    private Vector2Int _currentStrategicTarget;

    private BlackboardTask _possibleStrategicTask;

    private Map _map;

    private Vector2Int _healingPoint;

    private Agent _currentAttackTarget;


    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    public void ChangeMode(NpcMode mode, Vector2Int newTarget)
    {
        bool different = (_mode != mode) || (newTarget != _currentStrategicTarget);
        _mode = mode;
        _currentStrategicTarget = newTarget;

        CheckMode(different);
        _agent.ChangeModeColor(_mode == NpcMode.OFFENSE ? new Color(1f, 0.5f, 0f) : Color.green);
    }

    private void ChangeState(NpcState state)
    {
        _state = state;
        String stateStr = null;
        switch (state) {
            case NpcState.DEFENSE_ATTACK:
            case NpcState.OFFENSE_ATTACK:
                stateStr = "ATTACK";
                break;
            case NpcState.OFFENSE_CHASE:
            case NpcState.DEFENSE_CHASE:
                stateStr = "CHASE";
                break;
            default:
                stateStr = _state.ToString();
                break;
        }
        _agent.ChangeModeText(stateStr);
    }

    private void StateUpdate()
    {
        if (_mode == NpcMode.OFFENSE)
        {
            OffenseState();
        }
        else
        {
            DefenseState();
        }
    }

    private void OffenseState()
    {
        switch (_state)
        {
            case NpcState.RETREAT:
                RetreatState();
                break;
            case NpcState.OFFENSE_ATTACK:
                OffenseAttackState();
                break;
            case NpcState.OFFENSE_CHASE:
                OffenseChaseState();
                break;
            default:
                AdvanceState();
                break;
        }
    }

    private void DefenseState()
    {
        switch (_state)
        {
            case NpcState.PATROL:
                PatrolState();
                break;
            case NpcState.DEFENSE_ATTACK:
                DefenseAttackState();
                break;
            case NpcState.DEFENSE_CHASE:
                DefenseChaseState();
                break;
            default:
                DefendState();
                break;
        }
    }

    private void DefendState()
    {
        Agent enemyInRange = _agent.GetBestEnemyInRange();

        if ((enemyInRange != null) && (IsInAttackRange(enemyInRange))) 
        {
            TransitionToAttack(NpcState.DEFENSE_ATTACK, enemyInRange);
        } 
        else if (enemyInRange != null) 
        {
            TransitionToChase(NpcState.DEFENSE_CHASE, enemyInRange);
        }
        else if (_agent.MapPosition == _currentStrategicTarget)
        {
            GameObject[] waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
            string strTag = (_agent.Faction == NPCProperties.Faction.ALLY) ? "AllyBase" : "EnemyBase";
            GameObject myBase = GameObject.FindGameObjectWithTag(strTag);

            Path path = null;

            if (myBase.GetComponent<Base>().MapPosition == _currentStrategicTarget)
            {
                path = myBase.GetComponent<FixedPath>();
            }

            foreach (var waypoint in waypoints)
            {
                if (_map.WorldToGrid(waypoint.transform.position) == _currentStrategicTarget)
                {
                    path = waypoint.GetComponent<FixedPath>();
                }
            }

            TransitionToPatrol(path);
        }
    }

    private void PatrolState()
    {
        Agent enemyInRange = _agent.GetBestEnemyInRange();

        if ((enemyInRange != null) && IsInAttackRange(enemyInRange)) {
            TransitionToAttack(NpcState.DEFENSE_ATTACK, enemyInRange);
        } else if (enemyInRange != null) {
            TransitionToChase(NpcState.DEFENSE_CHASE, enemyInRange);
        }
    }

    private void DefenseAttackState()
    {
        Agent enemyInRange = _agent.GetBestEnemyInRange();

        if (enemyInRange == null)
        {
            TransitionToDefend();
        } else if ((enemyInRange != _currentAttackTarget) && IsInAttackRange(enemyInRange)) {
            TransitionToAttack(NpcState.DEFENSE_ATTACK, enemyInRange);
        } else if(!IsInAttackRange(enemyInRange)){
            TransitionToChase(NpcState.DEFENSE_CHASE, enemyInRange);
        }
    }
    
    private void RetreatState()
    {
        if ((_agent.HealthPercent >= 0.95f))
        {
            TransitionToAdvance();
        }
    }

    private void OffenseAttackState()
    {
        if ((_agent.HealthPercent <= 0.25f))
        {
            TransitionToRetreat();
            return;
        }

        Agent enemyInRange = _agent.GetBestEnemyInRange();
        if (enemyInRange == null)
        {
            TransitionToAdvance();
        } else if ((enemyInRange != _currentAttackTarget) && IsInAttackRange(enemyInRange)) {
            TransitionToAttack(NpcState.OFFENSE_ATTACK, enemyInRange);
        } else if (!IsInAttackRange(enemyInRange)) {
            TransitionToChase(NpcState.OFFENSE_CHASE, enemyInRange);
        }
    }

    private void OffenseChaseState() {
        if ((_agent.HealthPercent <= 0.25f)) {
            TransitionToRetreat();
            return;
        }

        Agent enemyInRange = _agent.GetBestEnemyInRange();
        if (enemyInRange == null) {
            TransitionToAdvance();
        } else if (IsInAttackRange(enemyInRange)) {
            TransitionToAttack(NpcState.OFFENSE_ATTACK, enemyInRange);
        } else {
            TransitionToChase(NpcState.OFFENSE_CHASE, enemyInRange);
        }
    }

    private void DefenseChaseState() {
        Agent enemyInRange = _agent.GetBestEnemyInRange();
        if (enemyInRange == null) {
            TransitionToDefend();
        } else if (IsInAttackRange(enemyInRange)) {
            TransitionToAttack(NpcState.DEFENSE_ATTACK, enemyInRange);
        } else {
            TransitionToChase(NpcState.DEFENSE_CHASE, enemyInRange);
        }
    }

    private void AdvanceState()
    {
        if (_agent.HealthPercent <= 0.33f)
        {
            TransitionToRetreat();
            return;
        }

        Agent enemyInRange = _agent.GetBestEnemyInRange();

        if ((enemyInRange != null) && IsInAttackRange(enemyInRange))
        {
            TransitionToAttack(NpcState.OFFENSE_ATTACK, enemyInRange);
        } else if((enemyInRange != null)) {
            TransitionToChase(NpcState.OFFENSE_CHASE, enemyInRange);
        }
    }

    private bool IsInState(NpcState state)
    {
        return state == _state;
    }

    private bool IsInAttackRange(Agent target) {
        return Vector3.Distance(_agent.Position, target.Position) <= _agent.AttackRange;
    }

    private void CheckMode(bool hasChanged)
    {
        if (hasChanged || (_state == NpcState.ADVANCE || _state == NpcState.DEFEND))
            EntryPoint();
    }

    private void EntryPoint()
    {
        if (_mode == NpcMode.OFFENSE)
        {
            TransitionToAdvance();
        }
        else
        {
            TransitionToDefend();
        }              
    }

    private void TransitionToAdvance()
    {
        ChangeState(NpcState.ADVANCE);
        Action action = (_agent.PathfindingAlgorithm == PathfindingAlgorithm.A_STAR)
            ? (PathfindingAction) new AStarAction(10f, 2, _agent, _currentStrategicTarget, _influenceMap)
            : (PathfindingAction) new LrtaStarAction(10f, 2, _agent, _currentStrategicTarget);
        _agent.ScheduleAction(action);
    }

    private void TransitionToDefend()
    {
        ChangeState(NpcState.DEFEND);
        Action action = (_agent.PathfindingAlgorithm == PathfindingAlgorithm.A_STAR)
            ? (PathfindingAction) new AStarAction(10f, 2, _agent, _currentStrategicTarget)
            : (PathfindingAction) new LrtaStarAction(10f, 2, _agent, _currentStrategicTarget);
        _agent.ScheduleAction(action);
    }

    private void TransitionToRetreat()
    {
        ChangeState(NpcState.RETREAT);
        Action action = (_agent.PathfindingAlgorithm == PathfindingAlgorithm.A_STAR)
            ? (PathfindingAction) new AStarAction(10f, 2, _agent, _healingPoint, _influenceMap)
            : (PathfindingAction) new LrtaStarAction(10f, 2, _agent, _healingPoint);
        _agent.ScheduleAction(action);
    }

    private void TransitionToAttack(NpcState state, Agent enemy)
    {
        ChangeState(state);
        List<Action> actionList = new List<Action>();
        actionList.Add(new AttackAction(10f, 2, _agent, enemy));
        actionList.Add(new FaceAction(10f, 2, _agent, enemy));
        //actionList.Add(new StopAction(10f, 2, _agent));
        ActionCombination combination = new ActionCombination(10f, 2, _agent, actionList);
        _agent.ScheduleAction(combination);
        _currentAttackTarget = enemy;
    }

    private void TransitionToChase(NpcState state, Agent enemy)
    {
        ChangeState(state);
        _agent.ScheduleAction(new ArriveAction(10f, 2, _agent, enemy));
        _currentAttackTarget = enemy;
    }

    private void TransitionToPatrol(Path path)
    {
        ChangeState(NpcState.PATROL);
        _agent.ScheduleAction(new PathFollowingAction(10f, 2, _agent, path));
    }

    private void Start()
    {
        _agent = GetComponent<AgentNPC>();
        ChangeMode(NpcMode.OFFENSE, _agent.MapPosition);
        _state = NpcState.ADVANCE;
        _map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        _currentAttackTarget = null;

        String strTag = (_agent.Faction == NPCProperties.Faction.ALLY) ? "AllyHealingPoint" : "EnemyHealingPoint";
        _healingPoint = _map.WorldToGrid(GameObject.FindGameObjectWithTag(strTag).transform.position);
    }

    private void OnEnable()
    {
        _timer = 0.5f;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        // Execute this every 0.75 seconds
        if (_timer >= 0.75f)
        {
            _timer = 0;
            StateUpdate();

            if (_agent.MapPosition == _healingPoint)
            {
                _agent.Heal(5f);
            }
        }
    }


    // TODO EXPERIMENTAL
    // Esto lo que hace es obtener un número según lo bien que se le de atacar o defender por las unidades que hay en esa zona
    private float CalculateUnitTypeRelevance(Vector2Int target, NpcMode mode) {
        float radius = 12f;
        String strTag = (_agent.Faction == NPCProperties.Faction.ALLY) ? "EnemyAgent" : "AllyAgent";
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(strTag);
        float relevance = 0f;
        foreach(GameObject enemy in enemies) {
            // For each enemy near the target position
            if(Vector3.Distance(enemy.transform.position, _map.GridToWorld(target)) <= radius) {
                AgentNPC enemyAgent = enemy.GetComponent<AgentNPC>();
                if (mode == NpcMode.OFFENSE) {
                    relevance += _agent.GetAttackFactor(enemyAgent.Type);
                } else {
                    relevance += _agent.GetDefenseFactor(enemyAgent.Type);
                }
            }
        }
        return relevance;
    }

    public float GetInsistence(Blackboard blackboard)
    {
        /* 
            Aquí lo que va es que se selecciona la acción con más insistencia y se asigna a posible strategic target
            de momento en función de la distancia, se hace la inversa para que sea más insistencia cuanto más cerca.
            Habrá que mirar de alguna forma que si este turno ya ha escogido, no busque y de insistencia 0
        */
        float maxInsistence = 0;
        float insistence = 0;
        foreach (BlackboardTask action in blackboard.Entries)
        {

            // TODO Cuando se ataca, escoger preferentemente a quien más vida tenga. Si se defiende, todos son igual de importantes.
            insistence = (action.Task == NpcMode.OFFENSE) ? (1f - _agent.LostHealthPercent) : 1f;
            // TODO Cuando se ataca, se busca el que más factor de ataque tiene en esa zona. Lo mismo para la defensa.
            insistence += (action.Task == NpcMode.OFFENSE) ? _agent.GetAttackFactor(action.Target) : _agent.GetDefenseFactor(action.Target);
            // TODO Cuando se ataca, se busca el que más daño pueda producir en esa zona. Cuando se defiende, se busca el que más aguante en esa zona.
            insistence += CalculateUnitTypeRelevance(action.Target, action.Task);
            // Multiply by 1/distance
            insistence *= 1 / Vector3.Distance(_agent.Position, _map.GridToWorld(action.Target));
            

            if (insistence >= maxInsistence)
            {
                maxInsistence = insistence;
                _possibleStrategicTask = action;
            }
        }

        return maxInsistence;
    }

    public void Run(Blackboard blackboard)
    {
        blackboard.PassTask(_possibleStrategicTask);
        ChangeMode(_possibleStrategicTask.Task, _possibleStrategicTask.Target);      
    }



}
