using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MapView : MonoBehaviour
{   
    [Header("References")]
    [SerializeField] private RectTransform _content;
    [SerializeField] private RectTransform _nodeRoot;
    [SerializeField] private RectTransform _lineRoot;

    [SerializeField] private MapNode _nodePrefab;
    [SerializeField] private MapLine _linePrefab;

    [Header("Layout")]
    [SerializeField] private float _unitsToPixels = 100f;
    [SerializeField] private float _padding = 100f;
    [SerializeField] private float _lineThickness = 4f;

    [Header("Node Colors")]
    [SerializeField] private Color _lockedColor = Color.gray;
    [SerializeField] private Color _visitedColor = Color.white;
    [SerializeField] private Color _visitableColor = Color.yellow;
        
    [Header("Line Colors")]
    [SerializeField] private Color _lineLockedColor = Color.gray;
    [SerializeField] private Color _lineVisitedColor = Color.white;
    [SerializeField] private Color _lineAvailableColor = Color.yellow;

    private readonly Dictionary<int, MapNode> _nodes = new();
    private readonly List<MapLine> _lines = new();

    private Map _displayedMap;
    private readonly HashSet<int> _visitedNodeIds = new();
    private readonly HashSet<(int FromNodeId, int ToNodeId)> _visitedEdges = new();

    public event Action<MapNode> OnNodeClicked;

    public void ShowMap(Map map, MapConfigSO config)
    {
        if (map == null || config == null)
            return;

        if (_content == null ||_nodeRoot == null || _lineRoot == null || _nodePrefab == null || _linePrefab == null)
        {
            Debug.LogError("MapView references are not assigned.", this);
            return;
        }

        Clear();

        _displayedMap = map;
        var activeNodes = map.Nodes;

        if (activeNodes.Count == 0)
            return;

        // 노드와 연결선을 같은 로컬 좌표계에 배치합니다.
        float minX = activeNodes.Min(n => n.position.x);
        float maxX = activeNodes.Max(n => n.position.x);
        float minY = activeNodes.Min(n => n.position.y);
        float maxY = activeNodes.Max(n => n.position.y);

        Vector2 origin = new Vector2((minX + maxX) * 0.5f, minY);

        _content.sizeDelta = new Vector2(
            (maxX - minX) * _unitsToPixels + _padding * 2f,
            (maxY - minY) * _unitsToPixels + _padding * 2f);

        var nodeSOs = config.MapNodeSOList
            .Where(so => so != null)
            .GroupBy(so => so.NodeType)
            .ToDictionary(g => g.Key, g => g.First());
        
         // 1. 노드 생성
        foreach (Node node in activeNodes)
        {
            MapNode mapNode = Instantiate(_nodePrefab, _nodeRoot);

            nodeSOs.TryGetValue(node.NodeType, out MapNodeSO nodeSO);

            mapNode.SetData(node, nodeSO);
            mapNode.SetState(MapNodeStates.Locked, _lockedColor, _visitedColor, _visitableColor);

            RectTransform rect = (RectTransform)mapNode.transform;

            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);

            rect.anchoredPosition = (node.position - origin) * _unitsToPixels + new Vector2(0f, _padding);

            mapNode.OnClicked += OnNodeClick;

            _nodes.Add(node.Id, mapNode);
        }

        // 2. 연결선 생성
        foreach (Node node in activeNodes)
        {
            foreach (int nextId in node.NextNodeIds)
            {
                if (!_nodes.TryGetValue(
                    node.Id, out MapNode from))
                    continue;

                if (!_nodes.TryGetValue(
                    nextId, out MapNode to))
                    continue;

                MapLine line = Instantiate(_linePrefab, _lineRoot);

                Vector2 fromPos = ((RectTransform)from.transform).anchoredPosition;
                Vector2 toPos = ((RectTransform)to.transform).anchoredPosition;

                line.SetData(node.Id, nextId, fromPos, toPos, _lineThickness);

                line.SetColor(_lineLockedColor);
                _lines.Add(line);
            }
        }
    }

    public void RefreshStates(Map map, IReadOnlyList<int> visitedNodeIds, Func<int, bool> canVisit)
    {
        if (map == null)
            return;
        
        if (visitedNodeIds == null)
            throw new ArgumentNullException(nameof(visitedNodeIds));

        if (canVisit == null)
            throw new ArgumentNullException(nameof(canVisit));
        
        if (!ReferenceEquals(_displayedMap, map))
        {
            Debug.LogWarning("Call ShowMap before refreshing a different map.", this);

            return;
        }

        _visitedNodeIds.Clear();
        _visitedEdges.Clear();

        for(int index = 0; index < visitedNodeIds.Count; index++)
        {
            int nodeId = visitedNodeIds[index];
            _visitedNodeIds.Add(nodeId);

            Node node = map.GetNode(nodeId);
            if (node == null)
                continue;

            if(index > 0)
            {
                int prevNodeId = visitedNodeIds[index - 1];
                _visitedEdges.Add((prevNodeId, nodeId));
            }
        }

        foreach (var node in map.Nodes)
        {
            if (!_nodes.TryGetValue(node.Id, out MapNode mapNode))
                continue;
            
            MapNodeStates state;

            if (_visitedNodeIds.Contains(node.Id))
            {
                state = MapNodeStates.Visited;
            }
            else if (canVisit(node.Id))
            {
                state = MapNodeStates.Visitable;
            }
            else
            {
                state = MapNodeStates.Locked;
            }

            // Assuming you have a way to get the corresponding MapNode for each Node
            mapNode.SetState(state, _lockedColor, _visitedColor, _visitableColor);
        }

        int? currentNodeId = visitedNodeIds.Count > 0 ? visitedNodeIds[visitedNodeIds.Count - 1] : (int?)null;

        foreach (MapLine line in _lines)
        {
            bool wasTraversed = _visitedEdges.Contains((line.FromNodeId, line.ToNodeId));

            bool isAvailable =
                currentNodeId.HasValue &&
                line.FromNodeId == currentNodeId.Value &&
                _nodes.TryGetValue(line.ToNodeId, out MapNode target) &&
                target.State == MapNodeStates.Visitable;

            Color color = wasTraversed ? _lineVisitedColor : isAvailable
                    ? _lineAvailableColor
                    : _lineLockedColor;

            line.SetColor(color);
        }
    }

    public void Clear()
    {
        foreach (MapNode node in _nodes.Values)
        {
            if (node == null)
                continue;

            node.OnClicked -= OnNodeClick;
            node.gameObject.SetActive(false);
            Destroy(node.gameObject);
        }

        foreach (MapLine line in _lines)
        {
            if (line == null)
                continue;

            line.gameObject.SetActive(false);
            Destroy(line.gameObject);
        }

        _nodes.Clear();
        _lines.Clear();
        _visitedNodeIds.Clear();
        _visitedEdges.Clear();

        _displayedMap = null;
    }

    private void OnNodeClick(MapNode mapNode)
    {
        if (mapNode == null || mapNode.Node == null)
            return;

        if (!_nodes.TryGetValue(mapNode.Node.Id, out MapNode currentView))
        {
            return;
        }

        if (currentView != mapNode)
            return;

        OnNodeClicked?.Invoke(mapNode);
    }

    private void OnDestroy()
    {
        Clear();
        OnNodeClicked = null;
    }
}
