using UnityEngine;

[System.Serializable]
public class MapLayer
{
    [Tooltip("The type of node this layer represents as default")]
    public NodeType NodeType;
    public float NodeApartDistance;

    [Tooltip("The probability of generating a random node in this layer, used for procedural generation")]
    [Range(0f, 1f)] public float randomNodeValue;

    [Tooltip("If this is set to 0, nodes on this layer will appear in a straight line. Closer to 1f = more position randomization")]
    [Range(0f, 1f)] public float randomizePosition;
}
