using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MapConfig", menuName = "ScriptableObjects/MapConfig", order = 1)]
public class MapConfigSO : ScriptableObject
{
    // node blueprint mapping, used for rendering the nodes in the map
    public List<MapNodeSO> MapNodeSOList = new List<MapNodeSO>();
    
    // except Start and Boss nodes, the rest of the node types can be randomly generated
    public List<NodeType> RandomNodeTypes = new List<NodeType>()
    {
        NodeType.Enemy,
        NodeType.EliteEnemy,
        NodeType.Treasure,
        NodeType.Store,
        NodeType.Rest,
        NodeType.Event,
    };

    // The distance between layers in the map, used for positioning layers in the map
    public float LayerDistance;

    public int GridWidth => Mathf.Max(numOfPreBossNodes.Max, numOfStartingNodes.Max);
    
    public IntRange numOfPreBossNodes;
    public IntRange numOfStartingNodes;

    public int extraPaths;
    public List<MapLayer> layers;
}