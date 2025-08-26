using System;
using UnityEngine;

namespace GameCore.SensorActionMachine
{
    [Serializable]
    public abstract class Target
    {
        public virtual Vector3? Position { get; protected set; }
        public virtual void Initialize() {}
        public virtual void Reset() {}
    }
}