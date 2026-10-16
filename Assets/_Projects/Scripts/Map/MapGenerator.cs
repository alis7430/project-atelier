using UnityEngine;
public class MapGenerator
{
    private readonly MapConfigSO _mapConfig;

    public MapGenerator(MapConfigSO config)
    {
        _mapConfig = config;
    }

    // Implementation for generating the map
    public Map GetMap()
    {
        if(_mapConfig == null)
        {
            Debug.LogError("Cannot generate map. MapConfigSO is null.");
            return null;
        }
        
        return new Map();
    }

    private void GeneratePaths()
    {
        
    }
}