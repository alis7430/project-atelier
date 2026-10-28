using System;
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
    [SerializeField] private Image _icon;
    [SerializeField] private Button _button;

    public Node Node{ get; private set; }
    public MapNodeSO NodeSO { get; private set; }

    public MapNodeStates State { get; private set; }

    public event System.Action<MapNode> OnClicked;

    private void Awake()
    {
        if(_button != null)
            _button.onClick.AddListener(OnClick);
    }

    public void SetData(Node node, MapNodeSO mapNodeSO)
    {
        Node = node;
        NodeSO = mapNodeSO;
        
        if(_icon != null)
        {
            _icon.sprite = NodeSO != null ? NodeSO.SpriteIcon : null;
        }
    }

    public void SetState(MapNodeStates newState, 
                        Color? lockedColor = null, 
                        Color? visitedColor = null, 
                        Color? visitableColor = null)
    {
        State = newState;
        switch(State)
        {
            case MapNodeStates.Locked:
                _icon.color = lockedColor ?? Color.gray;
                break;
            case MapNodeStates.Visited:
                _icon.color = visitedColor ?? Color.white;
                break;
            case MapNodeStates.Visitable:
                _icon.color = visitableColor ?? Color.yellow;
                break;
            default:
                _icon.color = Color.black;
                break;
        }

        if (_button != null)
        {
            _button.interactable = State == MapNodeStates.Visitable;
        }
    }

    private void OnClick()
    {
        if(State != MapNodeStates.Visitable) return;
        if(Node == null) return;

        OnClicked?.Invoke(this);
    }

    private void OnDestroy()
    {
        if(_button != null)
            _button.onClick.RemoveListener(OnClick);
    }
}
