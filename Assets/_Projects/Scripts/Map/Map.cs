using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Map : MonoBehaviour
{
    public static Color ColorLocked = Color.gray;
    public static Color ColorVisited = Color.white;
    public static Color ColorVisitable = Color.yellow;

    public readonly string ConfigName;
    public readonly string BossNodeName;

    // _nodes[layer][index]
    private readonly List<List<Node>> _nodes = new List<List<Node>>();

    public Map(List<List<Node>> nodes, string configName)
    {
        _nodes = nodes;
        ConfigName = configName;
        //BossNodeName = GetBossNode().name; // 이름 설정하는 법 고민중
    }

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
