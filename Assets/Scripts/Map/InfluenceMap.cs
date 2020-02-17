using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InfluenceMap : MonoBehaviour
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// The influence tilemap
    /// </summary>
    [SerializeField] private Tilemap _tilemap;

    /// <summary>
    /// The map
    /// </summary>
    [SerializeField] private Map _map;

    /// <summary>
    /// The radius of influence
    /// </summary>
    private static int RADIUS = 6;

    /// <summary>
    /// The ally influence map
    /// </summary>
    private GameGrid<float> _allyInfluenceMap;

    /// <summary>
    /// The enemy influence map
    /// </summary>
    private GameGrid<float> _enemyInfluenceMap;

    /// <summary>
    /// The coroutine controller
    /// </summary>
    private CoroutineController _coroutineController;

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    private void Start()
    {
        _allyInfluenceMap = new GameGrid<float>(_map.Height, _map.Width, 0f);
        _enemyInfluenceMap = new GameGrid<float>(_map.Height, _map.Width, 0f);
        _coroutineController =
            GameObject.FindGameObjectWithTag("CoroutineController").GetComponent<CoroutineController>();
        _coroutineController.StartChildCoroutine(UpdateCoroutine());
    }

    /// <summary>
    /// Resets the influence map.
    /// </summary>
    private void ResetInfluenceMap()
    {
        _allyInfluenceMap.FillGrid(0f);
        _enemyInfluenceMap.FillGrid(0f);
        for (int i = 0; i < _map.Width; i++)
        {
            for (int j = 0; j < _map.Height; j++)
            {
                Vector2Int pos = new Vector2Int(i, j);
                Vector3Int pos3 = new Vector3Int(i, j, 0);
                _tilemap.SetTileFlags(pos3, TileFlags.None);
                _tilemap.SetColor(pos3, _map.Get(pos) == TerrainType.MOUNTAIN ? Color.clear : Color.white);
            }
        }
    }

    /// <summary>
    /// Coroutine to update the influence map.
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.75f);
            UpdateInfluenceMap();
        }
    }

    /// <summary>
    /// Gets the influence exerted by a unit in the given position.
    /// </summary>
    /// <param name="unit">The unit.</param>
    /// <param name="position">The position.</param>
    /// <param name="distance">The distance.</param>
    /// <returns></returns>
    private float GetUnitInfluence(AgentNPC unit, Vector2Int position, out float distance)
    {
        distance = Vector2Int.Distance(unit.MapPosition, position);
        float baseInfluence = 1f;
        float terrainInfluence = unit.GetTerrainFactor(position);
        // Influence decays with distance
        return (baseInfluence * terrainInfluence) / ((1 + distance));
    }

    /// <summary>
    /// Gets the influence of a position. A positive influence means there is more ally influence.
    /// A negative influence means there is more enemy influence.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns></returns>
    public float GetInfluence(Vector2Int position)
    {
        return _allyInfluenceMap.Get(position) - _enemyInfluenceMap.Get(position);
    }

    public float GetConflict(Vector2Int position)
    {
        return _allyInfluenceMap.Get(position) + _enemyInfluenceMap.Get(position);
    }

    public float GetAllyInfluence(Vector2Int position)
    {
        return _allyInfluenceMap.Get(position);
    }

    public float GetEnemyInfluence(Vector2Int position)
    {
        return _enemyInfluenceMap.Get(position);
    }

    /// <summary>
    /// Updates the influence map.
    /// </summary>
    /// <param name="npcs">The NPCS.</param>
    /// <param name="faction">The faction.</param>
    private void UpdateInfluenceMap(GameObject[] npcs, NPCProperties.Faction faction)
    {
        GameGrid<float> influenceMap = (faction == NPCProperties.Faction.ALLY) ? _allyInfluenceMap : _enemyInfluenceMap;
        foreach (GameObject npc in npcs)
        {
            AgentNPC agent = npc.GetComponent<AgentNPC>();
            if (agent == null) continue;

            for (int i = -RADIUS; i <= RADIUS; i++)
            {
                for (int j = -RADIUS; j <= RADIUS; j++)
                {
                    Vector2Int pos = new Vector2Int(i, j) + agent.MapPosition;
                    Vector2Int center = agent.MapPosition;
                    bool insideCircle = ((pos.x - center.x) * (pos.x - center.x)) + ((pos.y - center.y) * (pos.y - center.y)) <
                                        (RADIUS * RADIUS);
                    if (!_map.IsValid(pos) || (_map.Get(pos) == TerrainType.MOUNTAIN) || !insideCircle) continue;

                    float dist = 0f;
                    float influence = GetUnitInfluence(agent, pos, out dist);

                    influenceMap.Set(pos, influence + influenceMap.Get(pos));
                }
            }
        }
    }

    /// <summary>
    /// Updates the influence map.
    /// </summary>
    private void UpdateInfluenceMap()
    {
        ResetInfluenceMap();
        GameObject[] allies = GameObject.FindGameObjectsWithTag("AllyAgent");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("EnemyAgent");

        UpdateInfluenceMap(allies, NPCProperties.Faction.ALLY);
        UpdateInfluenceMap(enemies, NPCProperties.Faction.ENEMY);

        UpdateColor();
    }

    /// <summary>
    /// Updates the color of the influence map.
    /// </summary>
    private void UpdateColor()
    {
        for (int i = 0; i < _map.Width; i++)
        {
            for (int j = 0; j < _map.Height; j++)
            {
                Vector2Int pos = new Vector2Int(i, j);

                // The mountains remains with no color
                if (_map.Get(pos) == TerrainType.MOUNTAIN) continue;

                Vector3Int pos3 = new Vector3Int(i, j, 0);

                float influence = GetInfluence(pos);
                float absInfluence = Mathf.Abs(influence);

                Color blueColor = new Color(1f - absInfluence, 1f - absInfluence, 1f, 1f);
                Color redColor = new Color(1f, 1f - absInfluence, 1f - absInfluence, 1f);

                Color color = (influence > 0) ? blueColor : redColor;

                _tilemap.SetColor(pos3, color);
            }
        }
    }

    public void SetAlpha(float a)
    {
        _tilemap.color = new Color(1f, 1f, 1f, a);
    }

}
