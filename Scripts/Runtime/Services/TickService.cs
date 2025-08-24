using System.Collections.Generic;
using R3;

namespace GameCore.Services
{
    public class TickService : Service, ITickable
    {
        private List<ITickable> _tickables = new();
        
        private readonly Subject<float> _ticked = new();
        public Observable<float> Ticked => _ticked;
        
        public void RegisterTick(ITickable tickable)
        {
            _tickables.Add(tickable);
        }

        public void Tick(float deltaTime)
        {
            for (int i = 0; i < _tickables.Count; i++)
            {
                if (_tickables[i] == null)
                {
                    _tickables.RemoveAt(i);
                    continue;
                }
                _tickables[i].Tick(deltaTime);
            }
            
            _ticked.OnNext(deltaTime);
        }
    }
}