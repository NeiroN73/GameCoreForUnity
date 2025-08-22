using R3;

namespace GameCore.Services
{
    public interface ITickable
    {
        ReactiveCommand<float> Ticked { get; }
        void Tick();
    }
}