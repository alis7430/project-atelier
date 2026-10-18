using UnityEngine;
using URDev.GameFramework;

public class MapManager : MonoBehaviour
{
    public MapConfigSO config;

    public void GenerateMap()
    {
        if(config == null) return;

        //MapGenerator mapGenerator = new MapGenerator(config, Managers.GetSystem<RuntimeManager>().Random);
        MapGenerator mapGenerator = new MapGenerator(config, new SystemRng(12345));

        Map map = mapGenerator.GetMap();
    }    
}
