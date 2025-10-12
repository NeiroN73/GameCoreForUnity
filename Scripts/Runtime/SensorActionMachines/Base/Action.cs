using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameCore.SensorActionMachine
{
    [Serializable]
    public abstract class Action 
    {
        public abstract void Initialize(MonoBehaviour entity, SensorActionMachine sensorActionMachine,
            Blackboard blackboard);
        public abstract UniTask Execute();
        
        protected virtual void Initialize() {}
        public virtual void Enter() {}
        public virtual void Tick(float deltaTime) {}
        public virtual void Exit() {}
    }
}