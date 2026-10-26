using UnityEngine;
using URDev.GameFramework;

public class MapTestScene : BaseScene
{
    [SerializeField] private MapConfigSO _config;
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private MapView _mapView;
    [SerializeField] private int _seed = 12345;

    protected override void Init()
    {
        base.Init();
        SceneName = "MapTestScene";
    }

    private void OnEnable()
    {
        if (_mapView != null)
            _mapView.OnNodeClicked += HandleNodeClicked;
    }

    private void OnDisable()
    {
        if (_mapView != null)
            _mapView.OnNodeClicked -= HandleNodeClicked;
    }

    public void Start()
    {
        GenerateNewMap();
    }

    private void HandleNodeClicked(MapNode mapNode)
    {
        if (mapNode == null || mapNode.Node == null)
            return;

        _mapManager.TryVisitNode(mapNode.Node.Id);
    }

    public override void Clear()
    {
        if (_mapView == null)
            return;

        _mapView.OnNodeClicked -= HandleNodeClicked;
        _mapView.Clear();
    }

    [ContextMenu("Map Test/Generate New Map")]
    private void GenerateNewMap()
    {
        if (!Application.isPlaying)
            return;

        if (_config == null || _mapManager == null || _mapView == null)
        {
            Debug.LogError("MapTestScene references are not assigned.", this);
            return;
        }

        var generator = new MapGenerator(_config, new SystemRng(_seed));
        Map map = generator.GetMap();

        // 생성에 실패하면 현재 맵과 진행은 유지합니다.
        if (map == null)
            return;

        _mapManager.SetMap(map);
        RedisplayCurrentMap();
    }

    // MapView 갱신 테스트
    [ContextMenu("Map Test/Redisplay Current Map")]
    private void RedisplayCurrentMap()
    {
        if (!Application.isPlaying)
            return;

        if (_config == null || _mapManager == null || _mapView == null)
            return;

        Map map = _mapManager.CurrentMap;
        if (map == null)
            return;

        _mapView.ShowMap(map, _config);
        _mapView.RefreshStates(
            map,
            _mapManager.VisitedNodeIds,
            _mapManager.CanVisit);
    }

    // MapView 지우기 테스트
    [ContextMenu("Map Test/Clear Displayed Map")]
    private void ClearDisplayedMap()
    {
        if (!Application.isPlaying)
            return;

        if (_mapView != null)
            _mapView.Clear();
    }
}
