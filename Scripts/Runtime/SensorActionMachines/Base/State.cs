using System;
using UnityEngine;

namespace GameCore.SensorActionMachine
{
    [Serializable]
    public abstract class State
    {
        public virtual bool CanEnter() => true;
        public virtual void Enter() { }
        public virtual void Tick(float deltaTime) { }
        public virtual void Exit() { }
    }

    [Serializable]
    public abstract class State<TEntity> : State
    {
        // protected TEntity Entity;
        // protected SensorActionMachine<TEntity> ActionMachine;
        //
        // public virtual void Initialize(TEntity entity, SensorActionMachine<TEntity> actionMachine)
        // {
        //     Entity = entity;
        //     ActionMachine = actionMachine;
        // }
    }
}