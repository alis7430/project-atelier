using UnityEngine;
using System.Collections.Generic;
using URDev.GameFramework;

public class Node
{
    public readonly NodeType nodeType = NodeType.None;
    public readonly Vector2Int position = Vector2Int.zero;
    public readonly List<Vector2Int> prevNodes = new List<Vector2Int>();
    public readonly List<Vector2Int> nextNodes = new List<Vector2Int>();
    public readonly string nodeName = string.Empty;

    public Node(NodeType type, Vector2Int pos, string name ="")
    {
        nodeType = type;
        position = pos;
        nodeName = name;
    }

    public void AddPrevNode(Vector2Int pos)
    {
        if (prevNodes.Contains(pos) == false)
            prevNodes.Add(pos);
    }

    public void AddNextNode(Vector2Int pos)
    {
        if (nextNodes.Contains(pos) == false)
            nextNodes.Add(pos);
    }

    public void RemovePrevNode(Vector2Int pos)
    {
        if (prevNodes.Contains(pos))
            prevNodes.Remove(pos);
    }

    public void RemoveNextNode(Vector2Int pos)
    {
        if (nextNodes.Contains(pos))
            nextNodes.Remove(pos);
    }

    public bool IsConnectedTo(Vector2Int pos)
    {
        return prevNodes.Contains(pos) || nextNodes.Contains(pos);
    }

    public bool HasNoConnections()
    {
        return prevNodes.Count == 0 && nextNodes.Count == 0;
    }
}
