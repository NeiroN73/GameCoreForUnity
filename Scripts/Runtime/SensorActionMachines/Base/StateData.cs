using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace GameCore.SensorActionMachine
{
    [Serializable]
    public class StateData<TEntity>
        where TEntity : MonoBehaviour
    {
        [field: SerializeReference] public List<Sensor<TEntity>> Sensors { get; private set; } = new();
        [field: SerializeField]
        [ListDrawerSettings(AlwaysExpanded = true)]
        public List<ActionGroup<TEntity>> ActionGroups { get; private set; } = new();
        
        [field: SerializeField] public int Priority { get; private set; }
        
        public Dictionary<string, Sensor<TEntity>> SensorsById = new();
        
        [field: SerializeReference] public List<Action<TEntity>> Actions { get; private set; }
    }
    
    [Serializable]
    public class ActionGroup<TEntity>
        where TEntity : MonoBehaviour
    {
        [SerializeReference]
        [TableList(AlwaysExpanded = true)]
        public List<Action<TEntity>> Actions = new();
        
        [Button]
        public void AddAction()
        {
            Actions.Add(null);
        }
    }
}