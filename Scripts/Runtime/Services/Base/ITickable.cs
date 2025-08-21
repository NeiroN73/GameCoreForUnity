using R3;

namespace Content.Scripts.Services
{
    public interface ITickable
    {
        ReactiveCommand<float> Ticked { get; }
        void Tick();
    }
}