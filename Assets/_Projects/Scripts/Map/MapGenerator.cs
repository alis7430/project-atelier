using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapGenerator
{
    private readonly MapConfigSO _config;
    private readonly List<List<Node>> _nodes = new List<List<Node>>();
    private IRandom _random;

    public MapGenerator(MapConfigSO config, IRandom random)
    {
        _config = config;
        _random = random;
    }

    // Implementation for generating the map
    public Map GetMap()
    {
        if(_config == null)
        {
            Debug.LogError("Cannot generate map. MapConfigSO is null.");
            return null;
        }
        
        return new Map();
    }

    private Node GetFinalNode()
    {
        int y = _config.layers.Count - 1;
        int index = 0;
    
        if(_config.GridWidth % 2 == 1)
            index = _config.GridWidth / 2;
        else
        {
            index = _random.Range(0, 2) == 0
                    ? _config.GridWidth / 2
                    : _config.GridWidth / 2 - 1;
        }

        return GetNode(y, index);
    }

    private List<List<Node>> GeneratePaths()
    {
        Node finalNode = GetFinalNode();
        List<List<Node>> paths = new List<List<Node>>();

        int numOfStartingNodes = _config.numOfStartingNodes.GetSample(_random);
        int numOfPreBossNodes = _config.numOfPreBossNodes.GetSample(_random);

        List<int> candidateIndices = new List<int>();
        for(int i = 0; i < _config.GridWidth; i++)
        {
            candidateIndices.Add(i);
        }
        candidateIndices.Shuffle(_random);

        List<Node> startingNodes = candidateIndices
                    .Take(numOfStartingNodes)
                    .Select(index => GetNode(0, index))
                    .ToList();

        
        candidateIndices.Shuffle(_random);

        int preBossLayer = finalNode.Layer - 1;

        List<Node> preBossNodes = candidateIndices
                    .Take(numOfPreBossNodes)
                    .Select(index => GetNode(preBossLayer, index))
                    .ToList();

        int numOfPaths = Mathf.Max(numOfStartingNodes, numOfPreBossNodes)
                        + Mathf.Max(0, _config.extraPaths);

        for(int i = 0; i < numOfPaths; i++)
        {
            Node start = startingNodes[i % startingNodes.Count];
            Node end = preBossNodes[i % preBossNodes.Count];

            List<Node> path = GeneratePath(start, end);
            path.Add(finalNode);
            paths.Add(path);
        }
        return null;
    }

    private List<Node> GeneratePath(Node from, Node to)
    {
        int targetLayer = to.Layer;
        int targetIndex = to.Index;
        int lastIndex = from.Index;

        List<Node> path = new List<Node>() { from };
        List<int> candidateIndices = new List<int>();

        for(int layer = from.Layer + 1; layer < targetLayer; layer++)
        {
            candidateIndices.Clear();
            int verticalDistance = targetLayer - layer;

            // forward candidate path
            int forwardIndex = lastIndex;
            int horizontalDistance = Mathf.Abs(targetIndex - forwardIndex);
            if(horizontalDistance <= verticalDistance)
                candidateIndices.Add(forwardIndex);

            // left candidate path
            int leftIndex = lastIndex - 1;
            horizontalDistance = Mathf.Abs(targetIndex - leftIndex);
            if(leftIndex >= 0 && horizontalDistance <= verticalDistance)
                candidateIndices.Add(leftIndex);
            
            // right candidate path
            int rightIndex = lastIndex + 1;
            horizontalDistance = Mathf.Abs(targetIndex - rightIndex);
            if(rightIndex < _config.GridWidth && horizontalDistance <= verticalDistance)
                candidateIndices.Add(rightIndex);

            // choose next
            int randomIndex = _random.Range(0, candidateIndices.Count);
            int candidateIndex = candidateIndices[randomIndex];

            Node next = GetNode(layer, candidateIndex);
            path.Add(next);
            lastIndex = candidateIndex;
        }
        path.Add(to);
        return path;
    }

    private void SetUpConnections(List<List<Node>> paths)
    {
        foreach(var path in paths)
        {
            for(int i = 0; i < path.Count - 1; i++)
            {
                Node curr = path[i];
                Node next = path[i + 1];

                curr.AddNext(next.Id);
                next.AddPrev(curr.Id);
            }
        }
    }

    private Node GetNode(int layer, int index)
    {
        if (layer < 0 || layer >= _nodes.Count)
            return null;

        if (index < 0 || index >= _nodes[layer].Count)
            return null;

        return _nodes[layer][index];
    }
}