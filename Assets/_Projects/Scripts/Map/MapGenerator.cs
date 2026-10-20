using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapGenerator
{
    private readonly MapConfigSO _config;
    private readonly List<List<Node>> _nodes = new List<List<Node>>();
    private readonly IRandom _random;

    private int _nextNodeId;

    public MapGenerator(MapConfigSO config, IRandom random)
    {
        _config = config;
        _random = random;
    }

    public void Clear()
    {
        _nodes.Clear();
        _nextNodeId = 0;
    }

    // Implementation for generating the map
    public Map GetMap()
    {
        if(_config == null)
        {
            Debug.LogError("Cannot generate map. MapConfigSO is null.");
            return null;
        }

        Clear();

        for (int layer = 0; layer < _config.layers.Count; layer++)
            PlaceLayer(layer);

        GeneratePaths();

        RandomizeNodePositions();
        RemoveCrossConnections();
        
        return new Map(_nodes, _config.name);
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

    private void GeneratePaths()
    {
        Node finalNode = GetFinalNode();

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

        for (int i = 0; i < numOfPaths; i++)
        {
            Node start = startingNodes[i % startingNodes.Count];
            Node end = preBossNodes[i % preBossNodes.Count];

            GeneratePath(start, end);

            Connect(end, finalNode);
        }
    }

    private void GeneratePath(Node from, Node to)
    {
        int targetLayer = to.Layer;
        int targetIndex = to.Index;
        int lastIndex = from.Index;
        
        Node current = from;
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
            Connect(current, next);

            current = next;
            lastIndex = candidateIndex;
        }

        // connect last node
        Connect(current, to);
    }

    private Node GetNode(int layer, int index)
    {
        if (layer < 0 || layer >= _nodes.Count)
            return null;

        if (index < 0 || index >= _nodes[layer].Count)
            return null;

        return _nodes[layer][index];
    }

    private void PlaceLayer(int layerIndex)
    {
        if(layerIndex < 0 || _config.layers.Count <= layerIndex)
        {
            Debug.LogError("Index error accured in map layer. Please check out map config");
            return;
        }

        MapLayer layer = _config.layers[layerIndex];
        List<Node> nodesOnLayer = new List<Node>();

        int gridWidth = _config.GridWidth;
        float nodeDistance = layer.NodeApartDistance;

        // Align nodes based on x-axis
        float offset = (gridWidth - 1) * nodeDistance * 0.5f;

        // Curr layer y-coordinate 
        float y = layerIndex * _config.LayerDistance;

        // Possible random node types
        List<NodeType> randomNodeTypes = _config.RandomNodeTypes
                        .Where(type => _config.MapNodeSOList.Any(mapNodeSO => mapNodeSO.NodeType == type))
                        .ToList();
        
        for(int index = 0; index < gridWidth; index++)
        {
            NodeType nodeType = layer.NodeType;

            // Generate random node type using layer randomNodeValue
            if(randomNodeTypes.Count > 0 && _random.Value() < layer.randomNodeValue)
            {
                nodeType = randomNodeTypes.RandomElement(_random);
            }

            // Create node!
            Node node = new Node(nodeType, _nextNodeId++, layerIndex, index);
            
            float x = index * nodeDistance - offset;
            node.SetPosition(new Vector2(x,y));
            
            nodesOnLayer.Add(node);
        }

        _nodes.Add(nodesOnLayer);
    }

    private void RandomizeNodePositions()
    {
        for(int layerIndex = 0; layerIndex < _nodes.Count; layerIndex++)
        {
            MapLayer layer = _config.layers[layerIndex];

            foreach(var node in _nodes[layerIndex])
            {
                float randomX = _random.Value() - 0.5f;
                float randomY = _random.Value() - 0.5f;

                float x = randomX * layer.NodeApartDistance;
                float y = randomY * _config.LayerDistance;

                Vector2 offset = new Vector2(x, y) * layer.randomNodeValue;
                node.SetPosition(node.position + offset);
            }
        }
    }

    private void RemoveCrossConnections()
    {
        for(int layer = 0; layer < _nodes.Count - 1; layer++)
        {
            for(int index = 0; index < _config.GridWidth - 1; index++)
            {
                var node = GetNode(layer, index);
                var right = GetNode(layer, index + 1);
                var top = GetNode(layer + 1, index);
                var topRight = GetNode(layer + 1, index + 1);

                if(node == null || right == null || top == null || topRight == null)
                    continue;

                // check cross connections
                if(!node.NextNodeIds.Contains(topRight.Id))
                    continue;
                if(!right.NextNodeIds.Contains(top.Id))
                    continue;

                float randomValue = _random.Value();

                if (randomValue < 0.2f)
                {
                    // 두 대각선 연결 모두 제거
                    Disconnect(node, topRight);
                    Disconnect(right, top);
                }
                else if (randomValue < 0.6f)
                {
                    // 왼쪽에서 오른쪽으로 가는 대각선 제거
                    Disconnect(node, topRight);
                }
                else
                {
                    // 오른쪽에서 왼쪽으로 가는 대각선 제거
                    Disconnect(right, top);
                }
            }
        }
    }

    private void Connect(Node from, Node to)
    {
        if(from == null || to == null)
        {
            Debug.LogError("Cannot connect nodes. Has null node.");
            return;
        }

        from.AddNext(to.Id);
        to.AddPrev(from.Id);
    }

    private void Disconnect(Node from, Node to)
    {
        if(from == null || to == null)
        {
            Debug.LogError("Cannot disconnect nodes. Has null node.");
            return;
        }

        from.RemoveNext(to.Id);
        to.RemovePrev(from.Id);
    }
}