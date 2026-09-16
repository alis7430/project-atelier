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

[CreateAssetMenu(fileName = "MapNodeSO", menuName = "ScriptableObjects/MapNodeSO", order = 0)]
public class MapNodeSO : ScriptableObject
{
    public Sprite sprite;
    public NodeType nodeType;
}
