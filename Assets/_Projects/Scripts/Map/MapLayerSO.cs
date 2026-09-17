using UnityEngine;

public class MapLayerSO : ScriptableObject
{
    [Tooltip("The type of node this layer represents as default")]
    public NodeType NodeType;
    public float NodeDistance;

    [Tooltip("The probability of generating a random node in this layer, used for procedural generation")]
    [Range(0f, 1f)] public float randomNodeValue;
}
