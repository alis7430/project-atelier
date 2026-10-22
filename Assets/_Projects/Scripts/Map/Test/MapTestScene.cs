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
        if (_config == null || _mapManager == null || _mapView == null)
        {
            Debug.LogError("MapTestScene references are not assigned.", this);

            return;
        }

        var generator = new MapGenerator(_config, new SystemRng(_seed));

        Map map = generator.GetMap();

        if (map == null)
        {
            Debug.LogError("Failed to generate map.", this);
            return;
        }
 
        _mapManager.SetMap(map);
        _mapView.ShowMap(map, _config);

        _mapView.RefreshStates(map, _mapManager.VisitedNodeIds, _mapManager.CanVisit);
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
}
