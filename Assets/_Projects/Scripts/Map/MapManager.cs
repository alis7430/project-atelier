using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    [SerializeField] private MapConfigSO _config;
    [SerializeField] private MapView _view;

    public Map CurrentMap { get; private set; }
    public int? CurrentNodeId { get; private set; }

    public IReadOnlyList<int> VisitedNodeIds => _visitedNodeIds.AsReadOnly();
    private readonly List<int> _visitedNodeIds = new List<int>();

    public event System.Action<Node> OnNodeSelected;

    public void GenerateMap()
    {
        if (_config == null) return;

        //MapGenerator mapGenerator = new MapGenerator(config, Managers.GetSystem<RuntimeManager>().Random);
        MapGenerator mapGenerator = new MapGenerator(_config, new SystemRng(12345));

        Map map = mapGenerator.GetMap();

        if (map == null)
        {
            Debug.LogError("Failed to generate map.");
            return;
        }

        SetMap(map);
    }

    public void SetMap(Map map)
    {
        if (map == null)
            return;

        CurrentMap = map;
        CurrentNodeId = null;
        _visitedNodeIds.Clear();
    }

    public bool CanVisit(int nodeId)
    {
        if (CurrentMap == null)
            return false;

        Node target = CurrentMap.GetNode(nodeId);

        if (target == null)
            return false;

        // 맵에서 첫 번째 노드를 선택하는 경우
        if (!CurrentNodeId.HasValue)
        {
            return target.Layer == 0 &&
                   target.NextNodeIds.Count > 0;
        }

        Node current = CurrentMap.GetNode(CurrentNodeId.Value);

        return current != null &&
               target.Layer == current.Layer + 1 &&
               current.NextNodeIds.Contains(nodeId);
    }

    public bool TryVisitNode(int nodeId)
    {
        if (!CanVisit(nodeId))
            return false;

        Node node = CurrentMap.GetNode(nodeId);

        CurrentNodeId = nodeId;
        _visitedNodeIds.Add(nodeId);

        RefreshView();

        OnNodeSelected?.Invoke(node);

        return true;
    }

    private void OnNodeClicked(int nodeId)
    {
        if (!TryVisitNode(nodeId))
        {
            Debug.Log($"Cannot visit node: {nodeId}");
        }
    }

    private void RefreshView()
    {
        if (_view == null || CurrentMap == null)
            return;

        _view.RefreshStates(CurrentMap, _visitedNodeIds, CanVisit);
    }

    private void OnDestroy()
    {
        OnNodeSelected = null;
    }

#if UNITY_EDITOR
    [ContextMenu("Validate Map Seeds")]
    private void ValidateMapSeeds()
    {
        if (_config == null)
        {
            Debug.LogError("MapConfigSO is not assigned.", this);
            return;
        }

        const int testCount = 10000;

        for (int i = 0; i < testCount; i++)
        {
            var seed = RandomSeedGenerator.Generate();
            var generator = new MapGenerator(_config, new SystemRng(seed));

            Map map;
            try
            {
                map = generator.GetMap();
            }
            catch (System.Exception exception)
            {
                Debug.LogError($"Map generation threw an exception. Seed: {seed}", this);
                Debug.LogException(exception, this);
                return;
            }

            if (map == null)
            {
                Debug.LogError(
                    $"Map validation failed. Seed: {seed}",
                    this);

                return;
            }
        }

        Debug.Log($"Map validation passed for seeds. TestCount: {testCount}.", this);
    }
#endif
}
