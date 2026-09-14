using UnityEngine;
using URDev.GameFramework;

public class Node
{
    private Define.NodeType _type = Define.NodeType.None;

    public Define.NodeType Type
    {
        get { return _type; }
        set { _type = value; }
    }
}
