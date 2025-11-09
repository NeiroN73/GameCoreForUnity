using System.Collections.Generic;

namespace GameCore.Services
{
    public class TickService : Service, ITickable
    {
        private List<ITickable> _tickables = new();
        
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
        }

        public void UnregisterTick(ITickable tickable)
        {
            _tickables.Remove(tickable);
        }
    }
}