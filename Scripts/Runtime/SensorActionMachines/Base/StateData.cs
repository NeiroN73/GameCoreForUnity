using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.SensorActionMachine
{
    [Serializable]
    public class StateData<TEntity>
        where TEntity : MonoBehaviour
    {
        [field: SerializeReference] public List<Sensor<TEntity>> Sensors { get; private set; } = new();
        [field: SerializeReference] public List<Action<TEntity>> Actions { get; private set; }
        [field: SerializeField] public int Priority { get; private set; }
        
        public Dictionary<string, Sensor<TEntity>> SensorsById = new();
    }
}