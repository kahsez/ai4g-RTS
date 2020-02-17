using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class AStar : Pathfinding
{
    private InfluenceMap _influenceMap;

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Initializes a new instance of the <see cref="AStar" /> class.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <param name="influenceMap">The influence map.</param>
    public AStar(Map map, InfluenceMap influenceMap = null) : base(map)
    {
        _influenceMap = influenceMap;
    }
    
    /// <summary>
    /// Performs the A* pathfinding algorithm in this map, from a starting cell to an objective cell.
    /// It will return an empty array if the destination is unreachable (worst case)
    /// </summary>
    /// <param name="pathfinder"></param>
    /// <param name="start"></param>
    /// <param name="objective"></param>
    /// <returns></returns>
    protected override Vector2Int[] FindPath(IPathfinder pathfinder, Vector2Int start, Vector2Int objective)
    {
        List<Vector2Int> path = new List<Vector2Int>();


        // Create the first node. The moment you reach a node whose father is null, you know it's the starting node
        NodeRecord currentNode = new NodeRecord(start, null)
        {
            accumulatedCost = 0f,
            estimatedCost = pathfinder.GetHeuristicFunc()(start, objective)
        };

        // Open nodes. These are sets so we can't have duplicated nodes
        List<NodeRecord> openNodes = new List<NodeRecord>();
        // Already explored nodes
        List<NodeRecord> closedNodes = new List<NodeRecord>();
        // Add the first node to open nodes
        openNodes.Add(currentNode);

        // Stop if there's no nodes to expand
        while ((openNodes.Count > 0))
        {
            // Get the easiest (closest) node to travel
            currentNode = GetClosestNode(openNodes);
            // Remove the node from the frontier 
            openNodes.RemoveAt(FindNode(openNodes, currentNode));
            // Add the node to the closed list
            closedNodes.Add(currentNode);

            // Have we reached the goal?
            if (currentNode.position == objective)
            {
                // Create the path from the objective node to the second node in the path (we don't need to reach the start)
                // We need to reverse it later since it's backwards
                for (NodeRecord n = currentNode; n.father != null; n = n.father)
                {
                    path.Add(n.position);
                }

                path.Reverse();
                break;
            }
            else
            {
                // Expand the node
                foreach (NodeRecord n in ExpandNode(currentNode, objective, openNodes, closedNodes, pathfinder))
                {
                    // Avoid duplicates
                    if (FindNode(openNodes, n) == -1)
                    {
                        openNodes.Add(n);
                    }
                }
            }
        }

        return path.ToArray();
    }

    /// <summary>Finds the node.</summary>
    /// <param name="list">The list.</param>
    /// <param name="node">The node.</param>
    /// <returns></returns>
    private int FindNode(List<NodeRecord> list, NodeRecord node)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (IsSameNode(list[i], node))
            {
                return i;
            }
        }

        return -1;
    }

    private float GetInfluenceCost(IPathfinder pf, Vector2Int pos)
    {
        if (_influenceMap == null) return 1f;
        if (pf.Faction == NPCProperties.Faction.ALLY)
        {
            return Mathf.Max(1f, _influenceMap.GetEnemyInfluence(pos) * 2f);
        }

        return Mathf.Max(1f,_influenceMap.GetAllyInfluence(pos) * 2f);
    }

    /// <summary>
    /// Expand a cell from the grid, getting all neighbours, except the invalid ones.
    /// Calculates the neighbours real distance and estimated distance
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private List<NodeRecord> ExpandNode(NodeRecord node, Vector2Int objective, List<NodeRecord> openNodes, List<NodeRecord> closedNodes, IPathfinder pathfinder)
    {
        List<NodeRecord> nodes = new List<NodeRecord>();
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                // Check that we aren't on the same cell
                if (x == 0 && y == 0) continue;

                Vector2Int neighbour = node.position + new Vector2Int(x, y);

                // Check if this adjacent cell is valid (not out of bounds)
                if (!_map.IsValid(neighbour)) continue;

                NodeRecord neighbourNode = new NodeRecord(neighbour, node);

                // Is the node unexplored?
                if (FindNode(closedNodes, neighbourNode) != -1) continue;

                // Does the node already exist?
                int search = FindNode(openNodes, neighbourNode);
                if (search != -1) neighbourNode = openNodes[search];
                // Calculate the cost to travel to this node. 
                // If we consider every square has a size of 10, then if we
                // travel horizontally or vertically the cost is 10
                // but diagonally is 14,14 (aprox.)
                TerrainType terrain = _map.Get(neighbour.x, neighbour.y);
                float baseCost = ((x == 0) || (y == 0)) ? 1f : MathAI.SqrtTwo;
                // Now we calculate how fast can this pathfinder travel to this node (TACTIC PATHFINDING)
                float travelCost;
                // f'(neighbourNode) = g(node) + travelCost + h(neighbourNode)
                // g(node) + travelCost
                if (IsReachable(node.position, neighbour, pathfinder))
                {
                    travelCost = baseCost / pathfinder.GetTerrainFactor(terrain);
                }
                else
                {
                    travelCost = Mathf.Infinity;
                }               

                float influenceCost = GetInfluenceCost(pathfinder, neighbour);
                float realCost = node.accumulatedCost + travelCost * influenceCost;
                // h(neighbourNode)
                float heuristic = pathfinder.GetHeuristicFunc()(neighbour, objective);

                // TACTIC PATHFINDING: INFLUENCE MATTERS
                //heuristic += influenceCost;

                // f'(neighbourNode)                           
                float estimatedCost = realCost + heuristic;


                if (estimatedCost < neighbourNode.estimatedCost)
                {
                    neighbourNode.estimatedCost = estimatedCost;
                    neighbourNode.accumulatedCost = realCost;
                    // Finally, add the node to the list
                    nodes.Add(neighbourNode);
                }
            }
        }

        return nodes;
    }

    /// <summary>
    /// Determines whether [is same node] [the specified a].
    /// </summary>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns>
    ///   <c>true</c> if [is same node] [the specified a]; otherwise, <c>false</c>.
    /// </returns>
    private bool IsSameNode(NodeRecord a, NodeRecord b)
    {
        if (a == null) return false;
        if (b == null) return false;
        return (a.position.x == b.position.x) && (a.position.y == b.position.y);
    }

    /// <summary>
    /// Gets the closest node.
    /// </summary>
    /// <param name="frontier">The frontier.</param>
    /// <returns></returns>
    private NodeRecord GetClosestNode(List<NodeRecord> frontier)
    {
        NodeRecord closest = null;
        float distance = Mathf.Infinity;
        foreach (NodeRecord n in frontier)
        {
            if (n.estimatedCost < distance)
            {
                distance = n.estimatedCost;
                closest = n;
            }
        }

        return closest;
    }
}

/// <summary>
/// 
/// </summary>
internal class NodeRecord
{
    public NodeRecord(Vector2Int position, NodeRecord father)
    {
        this.position = position;
        this.father = father;
        accumulatedCost = 0;
        estimatedCost = Mathf.Infinity;
    }

    public Vector2Int position;
    public NodeRecord father;
    public float accumulatedCost;
    public float estimatedCost;
}
