// Implement System.Random
// C# system random을 사용한 난수 생성기
public class SystemRng : IRng
{
    private readonly System.Random random;

    public SystemRng(int seed)
    {
        random = new System.Random(seed);
    }
    
    public int Range(int min, int max)
    {
        return random.Next(min, max);
    }

    public float Value()
    {
        return (float)random.NextDouble();
    }
}
