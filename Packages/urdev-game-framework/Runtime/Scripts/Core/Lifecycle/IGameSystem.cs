namespace URDev.GameFramework
{
    public interface IGameSystem
    {
        void Init();
        void Clear();
    }

    public interface IUpdatableSystem
    {
        void Tick(float deltaTime);
    }
}
