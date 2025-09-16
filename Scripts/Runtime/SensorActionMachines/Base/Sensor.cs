using System;
using UnityEngine;

namespace GameCore.SensorActionMachine
{
    [Serializable]
    public abstract class Sensor
    {
        public abstract string Id { get; }
        protected MonoBehaviour Entity;
        protected SensorActionMachine SensorActionMachine;
        protected Blackboard Blackboard;

        public virtual void Initialize(MonoBehaviour entity, SensorActionMachine actionMachine, Blackboard blackboard)
        {
            Entity = entity;
            SensorActionMachine = actionMachine;
            Blackboard = blackboard;

            Initialize();
        }
        
        public virtual void Disable() {}
        public abstract bool Check();
        public virtual void Initialize() {}
    }
}