using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Map : MonoBehaviour
{
    public static Color ColorLocked = Color.gray;
    public static Color ColorVisited = Color.white;
    public static Color ColorVisitable = Color.yellow;

    // _nodes[layer][index]
    private readonly List<List<Node>> _nodes = new List<List<Node>>();

    public Node GetNode(int layer, int index)
    {
        if(layer < 0 || layer >= _nodes.Count) return null;
        if(index < 0 || index >= _nodes[layer].Count) return null;

        return _nodes[layer][index];
    }

    // Brute-force
    public Node GetBossNode()
    {
        return _nodes
            .SelectMany( list => list )
            .FirstOrDefault(x => x.NodeType == NodeType.BossEnemy);
    }
}
