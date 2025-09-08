using System;
using UnityEngine;

namespace GameCore.SensorActionMachine
{
    [Serializable]
    public abstract class Target
    {
        [field: SerializeField] public string Id { get; private set; }
        public virtual Vector3? Position { get; protected set; }
        public virtual void Reset() {}
    }
}