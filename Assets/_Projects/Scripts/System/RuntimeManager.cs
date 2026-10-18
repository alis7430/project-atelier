using UnityEngine;
using URDev.GameFramework;

// 현재 게임의 시드 등의 런타임에 필요한 처리 관리
public class RuntimeManager : IGameSystem
{
    public int Seed { get; private set; }
    public IRandom Random{ get; private set; }
    
    public void Init()
    {
        
    }

    public void Clear()
    {
        Random = null;
        Seed = 0;   
    }

    public void GenerateSeed()
    {
        SetSeed(RandomSeedGenerator.Generate());
    }

    private void SetSeed(int seed)
    {
        Seed = seed;
        Random = new SystemRng(seed); 
    }
}
