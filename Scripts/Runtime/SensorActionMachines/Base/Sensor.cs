using System;
using UnityEngine;

namespace GameCore.SensorActionMachine
{
    [Serializable]
    public abstract class Sensor
    {
        public abstract string Id { get; }
        public abstract bool Check();
        public virtual void Initialize() {}
    }

    [Serializable]
    public abstract class Sensor<TEntity> : Sensor
        where TEntity : MonoBehaviour
    {
        protected TEntity Entity;
        protected SensorActionMachine<TEntity> SensorActionMachine;
        protected Blackboard Blackboard;

        public virtual void Initialize(TEntity entity, SensorActionMachine<TEntity> actionMachine, Blackboard blackboard)
        {
            Entity = entity;
            SensorActionMachine = actionMachine;
            Blackboard = blackboard;

            Initialize();
        }
        
        public virtual void Disable() {}
    }
}