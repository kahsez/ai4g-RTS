using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StrategicDecisionSystem : MonoBehaviour
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    private Blackboard _blackboard;

    private Dictionary<Vector2Int, Need> _needs;
    private Dictionary<Vector2Int, Need> _oldNeeds;

    private float _totalNeedValue;
    private float _oldTotalNeedValue;

    public static float InfluenceMultiplier = 1f;
    public static float NeedThreshold = 0.15f;

    [SerializeField] private GameObject[] _waypoints;

    [SerializeField] private NpcProperties.Faction _faction;

    [SerializeField] private Base _allyBase;

    [SerializeField] private Base _enemyBase;

    private InfluenceMap _influenceMap;

    private Map _map;

    private GameObject[] _allyAgents;

    private GameObject[] _enemyAgents;

    private float _timer;

    private bool _firstTime;

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    void OnEnable()
    {
        _influenceMap = GameObject.FindGameObjectWithTag("InfluenceMap").GetComponent<InfluenceMap>();
        _map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        _needs = new Dictionary<Vector2Int, Need>();
        _blackboard = new Blackboard();
        _firstTime = true;
        _timer = 0.75f;
    }

    private float CalcDefendBaseNeed(out Vector2Int target)
    {
        target = GetMyBase().MapPosition;
        float factionMult = (_faction == NpcProperties.Faction.ALLY) ? -1f : 1f;
        float influence = GetInfluence(target);
        float baseHealthRatio = (1f + GetMyBase().LostHealthPercent) / (1f + GetAdversaryBase().LostHealthPercent);
        return baseHealthRatio * Mathf.Max(0.75f, influence * factionMult);
    }

    private float CalcAttackBaseNeed(out Vector2Int target)
    {
        target = GetAdversaryBase().MapPosition;
        float factionMult = (_faction == NpcProperties.Faction.ENEMY) ? -1f : 1f;
        float influence = GetInfluence(target);
        float baseHealthRatio = (1f + GetAdversaryBase().LostHealthPercent) / (1f + GetMyBase().LostHealthPercent);
        return baseHealthRatio * Mathf.Max(1f, influence * factionMult);
    }

    private float CalcPositionNeed(Vector2Int position, out NpcMode needType)
    {
        float factionMult = (_faction == NpcProperties.Faction.ALLY) ? -1f : 1f;
        //float attackNeed = GetInfluence(position) * factionMult * InfluenceMultiplier;
        //float defendNeed = ((1f - Mathf.Min(GetMyInfluence(position), 1f)) - GetAdversaryInfluence(position)) * InfluenceMultiplier;
        float attackNeed = Mathf.Max(0f, GetAdversaryInfluence(position) - GetMyInfluence(position));
        float defendNeed = Mathf.Max(0.25f, GetInfluence(position) * factionMult);

        if (attackNeed >= defendNeed)
        {
            needType = NpcMode.OFFENSE;
            return attackNeed;
        }

        needType = NpcMode.DEFENSE;
        return defendNeed;
    }

    private int GetMyAgentsCount()
    {
        if (_faction == NpcProperties.Faction.ALLY)
            return _allyAgents.Length;
        return _enemyAgents.Length;
    }

    private int GetAdversaryAgentsCount()
    {
        if (_faction == NpcProperties.Faction.ENEMY)
            return _allyAgents.Length;
        return _enemyAgents.Length;
    }

    private float GetInfluence(Vector2Int pos) {
        return _influenceMap.GetInfluence(pos);
    }

    private float GetMyInfluence(Vector2Int pos)
    {
        if (_faction == NpcProperties.Faction.ALLY)
            return _influenceMap.GetAllyInfluence(pos);
        return _influenceMap.GetEnemyInfluence(pos);
    }

    private float GetAdversaryInfluence(Vector2Int pos)
    {
        return _faction == NpcProperties.Faction.ENEMY ? _influenceMap.GetAllyInfluence(pos) : _influenceMap.GetEnemyInfluence(pos);
    }

    private Base GetMyBase()
    {
        return _faction == NpcProperties.Faction.ALLY ? _allyBase : _enemyBase;
    }

    private Base GetAdversaryBase()
    {
        return _faction == NpcProperties.Faction.ALLY ? _enemyBase : _allyBase;
    }

    private Dictionary<Vector2Int, Need> CalcNeeds()
    {
        // Reset the needs
        Dictionary<Vector2Int, Need> needs = new Dictionary<Vector2Int, Need>();
        _totalNeedValue = 0f;
        Vector2Int pos = new Vector2Int();
        float needValue = CalcDefendBaseNeed(out pos);
        _totalNeedValue += needValue;
        needs.Add(pos, new Need(NpcMode.DEFENSE, needValue, pos));
        needValue = CalcAttackBaseNeed(out pos);
        needs.Add(pos, new Need(NpcMode.OFFENSE, needValue, pos));
        _totalNeedValue += needValue;

        foreach (GameObject wp in _waypoints)
        {
            pos = _map.WorldToGrid(wp.transform.position);
            NpcMode needType;
            needValue = CalcPositionNeed(pos, out needType);
            _totalNeedValue += needValue;
            needs.Add(pos, new Need(needType, needValue, pos));
        }

        return needs;
    }

    private void UpdateAgentLists()
    {
        _allyAgents = GameObject.FindGameObjectsWithTag("AllyAgent");
        _enemyAgents = GameObject.FindGameObjectsWithTag("EnemyAgent");
    }


    void Update()
    {
        if (_firstTime)
        {
            UpdateAgentLists();
            _needs = CalcNeeds();
            _firstTime = false;
        }

        _timer += Time.deltaTime;
        // Execute this every 0.75 seconds
        if (_timer >= 0.75f)
        {
            _timer = 0f;

            UpdateAgentLists();
            GameObject[] agentsGO = (_faction == NpcProperties.Faction.ALLY) ? _allyAgents : _enemyAgents;
            List<GameObject> agents = new List<GameObject>(agentsGO);

            _oldNeeds = _needs;

            // Calculate global needs
            _needs = CalcNeeds();

            if (!CheckDifferencesInNeeds())
                return;

            _oldTotalNeedValue = _totalNeedValue;

            List<Need> needs = _needs.Values.ToList();
            needs.Sort((a, b) => b.NeedValue.CompareTo(a.NeedValue));
           
            int availableAgents = agents.Count;

            foreach (Need need in needs)
            {
                float percentage = need.NeedValue / _totalNeedValue;
                int numberOfTasks = Mathf.FloorToInt(percentage * agents.Count);
                
                // We make sure that every agent has a task
                if (numberOfTasks == 0 && availableAgents > 0)
                {
                    numberOfTasks = 1;
                }

                availableAgents -= numberOfTasks;
                for (int i = 0; i < numberOfTasks; i++)
                {
                    BlackboardTask task = new BlackboardTask(need.NeedType, need.Target);
                    _blackboard.InsertTask(task);
                }

                if (availableAgents == 0)
                    break;
            }

            // If there is still available agents we assign the remaining tasks to the first need
            if (availableAgents > 0)
            {
                for (int i = 0; i < availableAgents; i++)
                {
                    BlackboardTask task = new BlackboardTask(needs[0].NeedType, needs[0].Target);
                    _blackboard.InsertTask(task);
                }
            }
            
            /*
             * Esta iteración se puede hacer mientras haya acciones en el blackboard, si fuese muy lento se puede hacer una
             * iteración en cada frame seleccionando uno cada vez, y cada x tiempo hacer la limpieza y resetear la blackboard
             */

            int count = _blackboard.Entries.Count;
            for (int i = 0; i < count; i++)
            {
                GameObject bestExpert = BlackboardIteration(agents);
                agents.Remove(bestExpert);
            }            
        }
    }

    private GameObject BlackboardIteration(List<GameObject> experts)
    {
        // Go through each expert for their insistence
        TacticDecisionSystem bestExpert = null;
        GameObject bestExpertGo = null;
        float highestInsistence = Mathf.NegativeInfinity;
        foreach (GameObject agent in experts)
        {
            TacticDecisionSystem expert = agent.GetComponent<TacticDecisionSystem>();
            // Ask for the expert's insistence
            float insistence = expert.GetInsistence(_blackboard);

            // Check against the highest value so far
            if (insistence >= highestInsistence)
            {
                highestInsistence = insistence;
                bestExpert = expert;         
            }
        }

       
        // Make sure somebody insisted
        if (bestExpert != null)
        {
            // Give control to the most insistent expert
            bestExpert.Run(_blackboard);
            bestExpertGo = bestExpert.gameObject;

            return bestExpertGo;
        }

        return null;
    }

    private bool CheckDifferencesInNeeds()
    {
        // Calculate percentages
        // Returns true whenever a percentage has changed more than 20%
        foreach (Vector2Int key in _oldNeeds.Keys)
        {
            float newPercent = _needs[key].NeedValue / _totalNeedValue;
            float oldPercent = _oldNeeds[key].NeedValue / _oldTotalNeedValue;
            if ((Mathf.Abs(newPercent - oldPercent) > NeedThreshold) || (_needs[key].NeedType != _oldNeeds[key].NeedType))
            {
                return true;
            }
        }

        return false;
    }


    private class Need
    {
        ///////////////////////////////////////////////////
        //////////////////// ATTRIBUTES ///////////////////
        ///////////////////////////////////////////////////

        private NpcMode _needType;

        private float _needValue;

        private Vector2Int _target;

        ///////////////////////////////////////////////////
        ///////////////////// ACCESS //////////////////////
        ///////////////////////////////////////////////////

        public NpcMode NeedType
        {
            get { return _needType; }
        }

        public float NeedValue
        {
            get { return _needValue; }
        }

        public Vector2Int Target
        {
            get { return _target; }
        }

        ///////////////////////////////////////////////////
        ///////////////////// METHODS /////////////////////
        ///////////////////////////////////////////////////

        public Need(NpcMode needType, float needValue, Vector2Int target)
        {
            _needType = needType;
            _needValue = needValue;
            _target = target;
        }
    }

}