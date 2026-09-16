using UnityEngine;
using URDev.GameFramework;

public enum NodeType
{
    None = -1,
    Start,
    Enemy,
    EliteEnemy,
    BossEnemy,
    Treasure,
    Store,
    Rest,
    Event,
    MaxCount,
}

/// <summary>
/// Node type to sprite mapping data.
/// </summary>
[CreateAssetMenu(fileName = "MapNodeSO", menuName = "ScriptableObjects/MapNodeSO", order = 0)]
public class MapNodeSO : ScriptableObject
{
    public Sprite sprite;
    public NodeType nodeType;
}
