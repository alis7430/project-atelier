using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Map
{
    public readonly string ConfigName;
    public readonly string BossNodeName;

    public IReadOnlyList<Node> Nodes => _nodes;
    private readonly List<Node> _nodes;
    private readonly Dictionary<int, Node> _nodesById;

    public Map(List<Node> nodes, string configName)
    {
        if(nodes == null)
            throw new System.ArgumentNullException(nameof(nodes));

        _nodes = new List<Node>(nodes);
        _nodesById = _nodes.ToDictionary(node => node.Id);

        ConfigName = configName;
        //BossNodeName = GetBossNode().name; // 이름 설정하는 법 고민중
    }

    public Node GetNode(int id)
    {
        return _nodesById.TryGetValue(id, out Node node) ? node : null;
    }

    public Node GetNode(int layer, int index)
    {
        return _nodes.FirstOrDefault(node =>node.Layer == layer && node.Index == index);
    }


    // Brute-force
    public Node GetBossNode()
    {
        return _nodes.FirstOrDefault(node => node.NodeType == NodeType.BossEnemy);
    }

    public IEnumerable<Node> GetStartNodes()
    {
        return _nodes.Where(node =>
            node.Layer == 0 &&
            node.NextNodeIds.Count > 0);
    }
}
