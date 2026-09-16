using System.Collections.Generic;

[System.Serializable]
public class Node
{
    public int Id { get; private set; }

    public int Layer { get; private set; }      // 맵의 몇층인지
    public int Index { get; private set; }      // Layer에서 몇번째 노드인지
    
    public NodeType nodeType { get; private set; }

    public List<int> NextNodeIds { get; private set; } = new List<int>();

    // for editor
    public List<int> PrevNodeIds { get; private set; } = new List<int>();
    
    public Node(NodeType type, int id, int layer, int index)
    {
        nodeType = type;
        Id = id;
        Layer = layer;
        Index = index;
    }

    public void AddNext(int nodeId)
    {
        NextNodeIds.Add(nodeId);
    }

    public void AddPrev(int nodeId)
    {
        PrevNodeIds.Add(nodeId);
    }
}
