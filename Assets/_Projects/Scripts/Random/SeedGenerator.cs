/// <summary>
/// 게임 시드 생성기
/// 외부환경에 의존하지 않고 독립적인 난수를 얻기위해 RandomNumberGenerator를 사용
/// </summary>
public static class RandomSeedGenerator
{
    private const int Min = 10_000_000;
    private const int Max = 100_000_000;
    public static int Generate()
    {
        return System.Security.Cryptography.RandomNumberGenerator.GetInt32(
            Min,
            Max
        );
    }
}
