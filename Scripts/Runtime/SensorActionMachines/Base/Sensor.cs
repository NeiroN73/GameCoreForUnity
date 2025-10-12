using System;
using UnityEngine;

namespace GameCore.SensorActionMachine
{
    [Serializable]
    public abstract class Sensor
    {
        public abstract string Id { get; }
        public abstract void Initialize(MonoBehaviour entity, SensorActionMachine actionMachine, Blackboard blackboard);
        
        public virtual void Disable() {}
        public abstract bool Check();
        protected virtual void Initialize() {}
    }
}