using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MapConfig", menuName = "ScriptableObjects/MapConfig", order = 1)]
public class MapConfig : ScriptableObject
{
    public List<MapNodeSO> mapNodeSOList = new List<MapNodeSO>();
    
    // except Start and Boss nodes, the rest of the node types can be randomly generated
    public List<NodeType> randomNodeTypes = new List<NodeType>()
    {
        NodeType.Enemy,
        NodeType.EliteEnemy,
        NodeType.Treasure,
        NodeType.Store,
        NodeType.Rest,
        NodeType.Event,
    };
}