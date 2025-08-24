namespace GameCore.Services
{
    public interface ITickable
    {
        void Tick(float deltaTime);
    }
}