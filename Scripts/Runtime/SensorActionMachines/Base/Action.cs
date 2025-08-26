using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameCore.SensorActionMachine
{
    [Serializable]
    public abstract class Action
    {
        public string Id;
        public abstract UniTask Execute();
        public virtual void Enter() {}
        public virtual void Tick(float deltaTime) {}
        public virtual void Exit() {}
    }
    
    [Serializable]
    public abstract class Action<TEntity> : Action
        where TEntity : MonoBehaviour
    {
        protected TEntity Entity;
        protected SensorActionMachine<TEntity> SensorActionMachine;

        public virtual void Initialize(TEntity entity, SensorActionMachine<TEntity> sensorActionMachine)
        {
            Entity = entity;
            SensorActionMachine = sensorActionMachine;
        }
    }

    public class Sequence<TEntity> where TEntity : MonoBehaviour
    {
        private readonly List<Action<TEntity>> _actions;
        private int _currentActionIndex;
        
        public Sequence(List<Action<TEntity>> actions)
        {
            _actions = actions;
        }

        public async UniTaskVoid Execute()
        {
            _currentActionIndex = 0;
            if (_currentActionIndex < _actions.Count)
            {
                var currentAction = _actions[_currentActionIndex];
                currentAction.Enter();
                await currentAction.Execute();
                currentAction.Exit();
                _currentActionIndex++;
            }
        }

        public void Tick(float deltaTime)
        {
            _actions[_currentActionIndex].Tick(deltaTime);
        }
    }
}