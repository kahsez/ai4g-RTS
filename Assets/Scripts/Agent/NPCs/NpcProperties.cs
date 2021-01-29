using System;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New NPCProperties", menuName = "IADeJ/NPCProperties")]
public class NpcProperties : ScriptableObject
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    [Header("NPCProperties Properties")]
    /// <summary>
    /// The type
    /// </summary>
    [SerializeField] private NPCType _type = default(NPCType);

    /// <summary>
    /// The sprite
    /// </summary>
    [SerializeField] private Sprite _sprite;

    /// <summary>
    /// The attack range
    /// </summary>
    [SerializeField] private float _perceptionRadius;

    /// <summary>
    /// The attack range
    /// </summary>
    [SerializeField] private float _attackRange;

    /// <summary>
    /// The attack range
    /// </summary>
    [SerializeField] private float _attackDamage;

    /// <summary>
    /// The attack cooldown
    /// </summary>
    [SerializeField] private float _attackCooldown;

    /// <summary>
    /// The maximum health
    /// </summary>
    [SerializeField] protected float _maxHealth;


    [Header("Matchups")]
    /// <summary>
    /// The soldier adf
    /// </summary>
    [SerializeField] private ADF _soldierADF = new ADF();
    
    /// <summary>
    /// The knight adf
    /// </summary>
    [SerializeField] private ADF _knightADF = new ADF();
   
    /// <summary>
    /// The saboteur adf
    /// </summary>
    [SerializeField] private ADF _saboteurADF = new ADF();
   
    /// <summary>
    /// The archer adf
    /// </summary>
    [SerializeField] private ADF _archerADF = new ADF();
    
    /// <summary>
    /// The catapult adf
    /// </summary>
    [SerializeField] private ADF _catapultADF = new ADF();

    
    [Header("Terrain Factors")]
    /// <summary>
    /// The plains factor
    /// </summary>
    [SerializeField] private TerrainFactors _plainsFactor = new TerrainFactors(0.5f);
    
    /// <summary>
    /// The forest factor
    /// </summary>
    [SerializeField] private TerrainFactors _forestFactor = new TerrainFactors(0.5f);
    
    /// <summary>
    /// The desert factor
    /// </summary>
    [SerializeField] private TerrainFactors _desertFactor = new TerrainFactors(0.5f);
    
    /// <summary>
    /// The road factor
    /// </summary>
    [SerializeField] private TerrainFactors _roadFactor = new TerrainFactors(0.5f);
    
    /// <summary>
    /// The mountain factor
    /// </summary>
    [SerializeField] private TerrainFactors _mountainFactor = new TerrainFactors(Mathf.Infinity);

    
    [Header("Pathfinding Properties")]
    /// <summary>
    /// The heuristic function
    /// </summary>
    [SerializeField] private HeuristicFunction _heuristicFunction = NpcProperties.HeuristicFunction.CHEBYCHEV;


    ///////////////////////////////////////////////////
    ///////////////////// ACCESS //////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Gets the NPC sprite.
    /// </summary>
    /// <value>
    /// The NPC sprite.
    /// </value>
    public Sprite NpcSprite
    {
        get { return _sprite; }
    }

    public NPCType Type
    {
        get { return _type; }
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

    public float AttackRange
    {
        get { return _attackRange; }
    }

    public float AttackDamage 
    {
        get { return _attackDamage; }
    }

    public float AttackCooldown
    {
        get { return _attackCooldown; }
    }

    public float PerceptionRadius {
        get { return _perceptionRadius; }
    }

    /// <summary>
    /// Gets the terrain speed.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public float GetTerrainSpeed(TerrainType type)
    {
        switch (type)
        {
            case TerrainType.MOUNTAIN:
                return _mountainFactor.speedFactor;
            case TerrainType.DESERT:
                return _desertFactor.speedFactor;
            case TerrainType.FOREST:
                return _forestFactor.speedFactor;
            case TerrainType.PLAINS:
                return _plainsFactor.speedFactor;
            case TerrainType.ROAD:
                return _roadFactor.speedFactor;
            default:
                return 1f;
        }
    }


    /// <summary>
    /// Gets the heuristic function.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception">NPCProperties ScriptableObject is missing a Heuristic Function value</exception>
    public Func<Vector2Int, Vector2Int, float> GetHeuristicFunc()
    {
        switch (_heuristicFunction)
        {
            case HeuristicFunction.EUCLIDEAN:
                return MathAI.EuclideanDistance;
            case HeuristicFunction.CHEBYCHEV:
                return MathAI.ChebychevDistance;
            case HeuristicFunction.MANHATTAN:
                return MathAI.ManhattanDistance;
            default:
                throw new Exception("NPCProperties ScriptableObject is missing a Heuristic Function value");
        }
    }

    /// <summary>
    /// Gets the attack factor.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public float GetAttackFactor(NPCType type)
    {
        switch (type)
        {
            case NPCType.SOLDIER:
                return _soldierADF.attackFactor;
            case NPCType.KNIGHT:
                return _knightADF.attackFactor;
            case NPCType.SABOTEUR:
                return _saboteurADF.attackFactor;
            case NPCType.ARCHER:
                return _archerADF.attackFactor;
            case NPCType.CATAPULT:
                return _catapultADF.attackFactor;
        }

        return 1f;
    }

    /// <summary>
    /// Gets the defense factor.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public float GetDefenseFactor(NPCType type)
    {
        switch (type)
        {
            case NPCType.SOLDIER:
                return _soldierADF.defenseFactor;
            case NPCType.KNIGHT:
                return _knightADF.defenseFactor;
            case NPCType.SABOTEUR:
                return _saboteurADF.defenseFactor;
            case NPCType.ARCHER:
                return _archerADF.defenseFactor;
            case NPCType.CATAPULT:
                return _catapultADF.defenseFactor;
        }

        return 1f;
    }

    public float GetTerrainAttackFactor(TerrainType type)
    {
        switch (type)
        {
            case TerrainType.MOUNTAIN:
                return _mountainFactor.attackDefenseFactor.attackFactor;
            case TerrainType.DESERT:
                return _desertFactor.attackDefenseFactor.attackFactor;
            case TerrainType.FOREST:
                return _forestFactor.attackDefenseFactor.attackFactor;
            case TerrainType.PLAINS:
                return _plainsFactor.attackDefenseFactor.attackFactor;
            case TerrainType.ROAD:
                return _roadFactor.attackDefenseFactor.attackFactor;
        }
        return 1f;
    }

    public float GetTerrainDefenseFactor(TerrainType type)
    {
        switch (type)
        {
            case TerrainType.MOUNTAIN:
                return _mountainFactor.attackDefenseFactor.defenseFactor;
            case TerrainType.DESERT:
                return _desertFactor.attackDefenseFactor.defenseFactor;
            case TerrainType.FOREST:
                return _forestFactor.attackDefenseFactor.defenseFactor;
            case TerrainType.PLAINS:
                return _plainsFactor.attackDefenseFactor.defenseFactor;
            case TerrainType.ROAD:
                return _roadFactor.attackDefenseFactor.defenseFactor;
        }
        return 1f;
    }


    ///////////////////////////////////////////////////
    //////////////////// STRUCTURES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Heuristic Function
    /// </summary>
    public enum HeuristicFunction
    {
        EUCLIDEAN,
        CHEBYCHEV,
        MANHATTAN
    }

    /// <summary>
    /// NPC Type
    /// </summary>
    public enum NPCType
    {
        SOLDIER,
        KNIGHT,
        SABOTEUR,
        ARCHER,
        CATAPULT
    }

    /// <summary>
    /// Faction
    /// </summary>
    public enum Faction
    {
        ALLY,
        ENEMY
    }

    /// <summary>
    /// Class representing the Attack/Defense Factor
    /// </summary>
    [System.Serializable]
    private class ADF
    {
        /// <summary>
        /// The attack factor
        /// </summary>
        public float attackFactor = 1f;

        /// <summary>
        /// The defense factor
        /// </summary>
        public float defenseFactor = 1f;
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    private class TerrainFactors
    {
        /// <summary>
        /// The speed factor
        /// </summary>
        public float speedFactor = 0.5f;

        /// <summary>
        /// The attack defense factor
        /// </summary>
        public ADF attackDefenseFactor = new ADF();

        /// <summary>
        /// Initializes a new instance of the <see cref="TerrainFactors"/> class.
        /// </summary>
        /// <param name="speedFactor">The speed factor.</param>
        public TerrainFactors(float speedFactor)
        {
            this.speedFactor = speedFactor;
        }
    }
}
