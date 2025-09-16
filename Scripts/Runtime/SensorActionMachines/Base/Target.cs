using System;

namespace GameCore.SensorActionMachine
{
    [Serializable]
    public abstract class Target
    {
        public virtual void Reset() {}
    }
}