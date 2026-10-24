using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapGenerator
{
    private readonly MapConfigSO _config;
    private readonly List<List<Node>> _nodes = new List<List<Node>>();
    private readonly IRandom _random;

    // 교차 처리 후에도 선택한 끝점들이 유지되는지 검증합니다.
    private readonly List<Node> _startingNodes = new();
    private readonly List<Node> _preBossNodes = new();

    private Node _bossNode;
    private int _nextNodeId;

#region Static Method
    private static bool IsValidRange(IntRange range)
    {
        return range != null &&
               range.Min >= 1 &&
               range.Max >= range.Min;
    }

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }

    private static bool IsUnitRange(float value)
    {
        return IsFinite(value) && value >= 0f && value <= 1f;
    }
#endregion

    public MapGenerator(MapConfigSO config, IRandom random)
    {
        _config = config;
        _random = random;
    }

    public void Clear()
    {
        _nodes.Clear();
        _startingNodes.Clear();
        _preBossNodes.Clear();

        _bossNode = null;
        _nextNodeId = 0;
    }

    // Implementation for generating the map
    public Map GetMap()
    {
        string error = Vaildate();

        if(error != null)
            return Fail(error);

        Clear();

        for (int layer = 0; layer < _config.layers.Count; layer++)
            PlaceLayer(layer);

        if (!GeneratePaths())
            return Fail("Failed to generate a complete path.");

        RandomizeNodePositions();
        RemoveCrossConnections();
        
        List<Node> availableNodes = _nodes.SelectMany(layer => layer)
                    .Where(node => node.NextNodeIds.Count > 0 || node.PrevNodeIds.Count > 0)
                    .ToList();
        // List<Node> availableNodes = _nodes.SelectMany(layer => layer).ToList();

        // path 검증        
        error = ValidatePaths(availableNodes);
        if(error != null)
            return Fail(error);

        return new Map(availableNodes, _config.name);
    }

    private Map Fail(string message)
    {
        Debug.LogError($"Map generation failed: {message}");
        return null;
    }

    private string Vaildate()
    {
       if (_config == null)
            return "MapConfigSO is null.";

        if (_random == null)
            return "IRandom is null.";

        if (_config.layers == null || _config.layers.Count < 3)
            return "At least three layers are required.";

        if (!IsValidRange(_config.numOfStartingNodes))
            return "Invalid starting node count range.";

        if (!IsValidRange(_config.numOfPreBossNodes))
            return "Invalid pre-boss node count range.";

        if (_config.extraPaths < 0)
            return "extraPaths must be non-negative.";

        int width = _config.GridWidth;

        // IntRange.GetSample에서 Max + 1을 사용
        if (width == int.MaxValue)
            return "Node count range is too large.";

        long pathCount = (long)width + _config.extraPaths;

        if (pathCount > int.MaxValue)
            return "Path count is too large.";

        // 모든 시작점/목적지 조합이 한 레이어당 한 칸 이동으로 연결될 수 있도록 제한하는 조건
        int transitionsToPreBoss = _config.layers.Count - 2;

        if (width - 1 > transitionsToPreBoss)
        {
            return "Too few layers for the grid width. " +
                   "Required: GridWidth - 1 <= layers.Count - 2.";
        }

        if (!IsFinite(_config.LayerDistance) ||
            _config.LayerDistance <= 0f)
        {
            return "LayerDistance must be finite and positive.";
        }

        if (_config.MapNodeSOList == null ||
            _config.MapNodeSOList.Any(so => so == null))
        {
            return "MapNodeSOList contains a missing reference.";
        }

        if (_config.RandomNodeTypes == null)
            return "RandomNodeTypes is null.";

        if (_config.RandomNodeTypes.Contains(NodeType.BossEnemy))
            return "BossEnemy cannot be a random node type.";

        for (int i = 0; i < _config.layers.Count; i++)
        {
            MapLayer layer = _config.layers[i];

            if (layer == null)
                return $"Layer {i} is null.";

            if (!IsFinite(layer.NodeApartDistance) ||
                layer.NodeApartDistance <= 0f)
            {
                return $"Layer {i}: invalid NodeApartDistance.";
            }

            if (!IsUnitRange(layer.randomNodeValue) ||
                !IsUnitRange(layer.randomizePosition))
            {
                return $"Layer {i}: random values must be within 0..1.";
            }

            if (i < _config.layers.Count - 1 &&
                layer.NodeType == NodeType.BossEnemy)
            {
                return $"Layer {i}: BossEnemy is reserved for the final layer.";
            }
        }

        return null;
    }

    private string ValidatePaths(List<Node> nodes)
    {
        if (_bossNode == null || !nodes.Contains(_bossNode))
            return "The selected boss is missing.";

        if (_bossNode.NodeType != NodeType.BossEnemy ||
            _bossNode.Layer != _config.layers.Count - 1 ||
            _bossNode.NextNodeIds.Count != 0)
        {
            return "Invalid boss node.";
        }

        if (_startingNodes.Any(node => !nodes.Contains(node)))
            return "A selected starting node is missing.";

        if (_preBossNodes.Any(node => !nodes.Contains(node)))
            return "A selected pre-boss node is missing.";

        var nodesById = nodes.ToDictionary(node => node.Id);

        foreach (Node node in nodes)
        {
            if (node.Layer < 0 || node.Layer > _bossNode.Layer)
                return $"Invalid layer: {node.Id}.";

            if (node.Layer == 0)
            {
                if (!_startingNodes.Contains(node) ||
                    node.PrevNodeIds.Count != 0)
                {
                    return $"Invalid starting node: {node.Id}.";
                }
            }
            else if (node.PrevNodeIds.Count == 0)
            {
                return $"Node has no incoming path: {node.Id}.";
            }

            if (node != _bossNode)
            {
                if (node.Layer == _bossNode.Layer ||
                    node.NodeType == NodeType.BossEnemy)
                {
                    return $"Unexpected final node: {node.Id}.";
                }

                if (node.NextNodeIds.Count == 0)
                    return $"Dead end: {node.Id}.";
            }

            foreach (int nextId in node.NextNodeIds)
            {
                if (!nodesById.TryGetValue(nextId, out Node next))
                    return $"Missing node: {nextId}.";

                if (next.Layer != node.Layer + 1)
                    return $"Invalid direction: {node.Id} -> {nextId}.";

                if (!next.PrevNodeIds.Contains(node.Id))
                    return $"Inconsistent connection: {node.Id} -> {nextId}.";

                if (!IsFinite(node.position.y) ||
                    !IsFinite(next.position.y) ||
                    next.position.y <= node.position.y)
                {
                    return $"Non-forward position: {node.Id} -> {nextId}.";
                }
            }

            foreach (int prevId in node.PrevNodeIds)
            {
                if (!nodesById.TryGetValue(prevId, out Node previous) ||
                    !previous.NextNodeIds.Contains(node.Id))
                {
                    return $"Inconsistent previous connection: {prevId} -> {node.Id}.";
                }
            }
        }

        return null;
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

    private bool GeneratePaths()
    {
        _bossNode = GetFinalNode();

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

        _startingNodes.AddRange(startingNodes);

        candidateIndices.Shuffle(_random);

        int preBossLayer = _bossNode.Layer - 1;

        List<Node> preBossNodes = candidateIndices
                    .Take(numOfPreBossNodes)
                    .Select(index => GetNode(preBossLayer, index))
                    .ToList();

        _preBossNodes.AddRange(preBossNodes);

        int numOfPaths = Mathf.Max(numOfStartingNodes, numOfPreBossNodes)
                        + Mathf.Max(0, _config.extraPaths);

        for (int i = 0; i < numOfPaths; i++)
        {
            Node start = startingNodes[i % startingNodes.Count];
            Node end = preBossNodes[i % preBossNodes.Count];

            if(!GeneratePath(start, end))
                return false;

            Connect(end, _bossNode);
        }
        return true;
    }

    private bool GeneratePath(Node from, Node to)
    {
        int targetLayer = to.Layer;
        int targetIndex = to.Index;
        int lastIndex = from.Index;

        int transitions = to.Layer - from.Layer;
        if (transitions <= 0 || Mathf.Abs(to.Index - from.Index) > transitions)
        {
            return false;
        }
        
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

            // 진행할 후보가 없다면 생성 실패
            if (candidateIndices.Count == 0)
                return false;

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

        return true;
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

            // 마지막 레이어는 무작위 선택 결과와 상관없이 Boss 타입 보장
            if(layerIndex == _config.layers.Count - 1)
                nodeType = NodeType.BossEnemy;

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

                Vector2 offset = new Vector2(x, y) * layer.randomizePosition;

                // y축의 순서 역전 방지 코드
                offset.y = Mathf.Clamp(offset.y, _config.LayerDistance * 0.4f, _config.LayerDistance * 0.4f);

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

                // At first, connect vertical connection
                Connect(node, top);
                Connect(right, topRight);
                
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