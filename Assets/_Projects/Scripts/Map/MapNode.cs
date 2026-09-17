using UnityEngine;
using UnityEngine.UI;

public enum MapNodeStates
{
    Locked,
    Visited,
    Visitable,
}

// Node.cs => data, MapNode.cs => Intraction
public class MapNode : MonoBehaviour
{
    public Image Icon;
    public MapNodeStates State;
    public Node Node{ get; private set; }
    public MapNodeSO NodeSO { get; private set; }

    public void SetData(Node node, MapNodeSO mapNodeSO)
    {
        this.Node = node;
        NodeSO = mapNodeSO;
        SetData();
    }

    public void SetState(MapNodeStates newState)
    {
        State = newState;
        switch(State)
        {
            case MapNodeStates.Locked:
                Icon.color = Map.ColorLocked;
                break;
            case MapNodeStates.Visited:
                Icon.color = Map.ColorVisited;
                break;
            case MapNodeStates.Visitable:
                Icon.color = Map.ColorVisitable;
                break;
            default:
                Icon.color = Color.black;
                break;
        }
    }

    private void SetData()
    {
        if(NodeSO != null && Icon != null)
        {
            Icon.sprite = NodeSO.SpriteIcon;
        }

    }
}
