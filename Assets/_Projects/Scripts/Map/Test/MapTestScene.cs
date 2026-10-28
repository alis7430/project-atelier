using UnityEngine;
using URDev.GameFramework;

public class MapTestScene : BaseScene
{    
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private int _seed = 12345;

    protected override void Init()
    {
        base.Init();
        SceneName = "MapTestScene";
    }

    public void Start()
    {
        GenerateNewMap();
    }
    public override void Clear()
    {
        if (_mapManager == null)
            return;

        _mapManager.Clear();
    }

    [ContextMenu("Map Test/Generate New Map")]
    private void GenerateNewMap()
    {
        if (!Application.isPlaying)
            return;

        if (_mapManager == null)
        {
            Debug.LogError("MapTestScene references are not assigned.", this);
            return;
        }

        if (!_mapManager.GenerateMap(_seed))
            return;

        Map map = _mapManager.CurrentMap;

        // 생성에 실패하면 현재 맵과 진행은 유지합니다.
        if (map == null)
            return;

        _mapManager.DisplayMap();
    }

    // MapView 갱신 테스트
    [ContextMenu("Map Test/Redisplay Current Map")]
    private void RedisplayCurrentMap()
    {
        if (!Application.isPlaying)
            return;

        if (_mapManager == null)
            return;

        Map map = _mapManager.CurrentMap;
        if (map == null)
            return;

        _mapManager.DisplayMap();
    }

    // MapView 지우기 테스트
    [ContextMenu("Map Test/Clear Displayed Map")]
    private void ClearDisplayedMap()
    {
        if (!Application.isPlaying)
            return;

        if (_mapManager != null)
            _mapManager.ClearDisplayedMap();
    }
}
