using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapConfigSO config;

    public void GenerateMap()
    {
        if(config == null) return;
        MapGenerator mapGenerator = new MapGenerator(config);
        Map map = mapGenerator.GetMap();
    }    
}
