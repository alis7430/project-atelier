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
    public Image image;
    public MapNodeStates state;
    public Node Node{ get; private set; }
    public MapNodeSO NodeSO { get; private set; }

    public void SetData(Node node, MapNodeSO mapNodeSO)
    {
        Node = node;
        NodeSO = mapNodeSO;
        SetData();
    }

    public void SetState(MapNodeStates newState)
    {
        state = newState;
        switch(state)
        {
            case MapNodeStates.Locked:
                image.color = Map.ColorLocked;
                break;
            case MapNodeStates.Visited:
                image.color = Map.ColorVisited;
                break;
            case MapNodeStates.Visitable:
                image.color = Map.ColorVisitable;
                break;
            default:
                image.color = Color.black;
                break;
        }
    }

    private void SetData()
    {
        if(NodeSO != null && image != null)
        {
            image.sprite = NodeSO.sprite;
        }

    }
}
