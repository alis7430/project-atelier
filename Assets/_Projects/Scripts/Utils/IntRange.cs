[System.Serializable]
public class IntRange
{
    public int Min;
    public int Max;

    public int GetSample(IRandom random)
    {
        return random.Range(Min, Max + 1);
    }
}