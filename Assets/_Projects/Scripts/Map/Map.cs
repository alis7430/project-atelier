using UnityEngine;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
    public static Color ColorLocked = Color.gray;
    public static Color ColorVisited = Color.white;
    public static Color ColorVisitable = Color.yellow;

    public List<Node> Nodes;
}
