using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameCore.SensorActionMachine
{
    [CreateAssetMenu(fileName = "SensorActionState", menuName = "Config/SensorActionState")]
    public class SensorActionState : ScriptableObject
    {
        [field: SerializeField] public SequenceActions ActionSequence { get; private set; }
        [field: SerializeReference] public List<Sensor> Sensors { get; private set; }
        [field: SerializeField] public int Priority { get; private set; }
        
        public Dictionary<string, Sensor> SensorsById = new();
    }
    
    [Serializable]
    public class SequenceActions
    {
        [field: SerializeField] public List<ParallelActions> ParallelActions { get; private set; }
        private int _currentActionIndex;

        public async UniTaskVoid Execute()
        {
            _currentActionIndex = 0;
            if (_currentActionIndex < ParallelActions.Count)
            {
                foreach (var action in ParallelActions[_currentActionIndex].Actions)
                {
                    action.Enter();
                    await action.Execute();
                    action.Exit();
                }
                _currentActionIndex++;
            }
        }

        public void Tick(float deltaTime)
        {
            foreach (var action in ParallelActions[_currentActionIndex].Actions)
            {
                action.Tick(deltaTime);
            }
        }
    }
    
    [Serializable]
    public class ParallelActions
    {
        [field: SerializeReference] public List<Action> Actions { get; private set; }
    }
}