using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameCore.SensorActionMachine
{
    public abstract class SensorActionMachine<TEntity>
        where TEntity : MonoBehaviour
    {
        protected abstract List<StateData<TEntity>> StateDatas { get; }
        protected abstract List<Target> Targets { get; }
        protected abstract Blackboard Blackboard { get; }
        
        private Sequence<TEntity> _currentSequence;
        private Dictionary<string, Sensor<TEntity>> _currentSensors = new();

        private TEntity _entity;
        
        public virtual void Initialize(TEntity entity)
        {
            _entity = entity;
            
            foreach (var stateData in StateDatas)
            {
                foreach (var action in stateData.Actions)
                {
                    action.Initialize(entity, this);
                }
                foreach (var sensor in stateData.Sensors)
                {
                    sensor.Initialize(entity, this, Blackboard);
                }

                stateData.SensorsById = stateData.Sensors.ToDictionary(s => s.Id);
            }
        }

        public void Disable()
        {
            foreach (var stateData in StateDatas)
            {
                foreach (var sensor in stateData.Sensors)
                {
                    sensor.Disable();
                }
            }
        }
        
        public TSensor GetSensor<TSensor>() where TSensor : Sensor
        {
            foreach (var sensor in _currentSensors)
            {
                if (sensor is TSensor sensorOfType)
                {
                    return sensorOfType;
                }
            }
            return null;
        }
        
        public TTarget GetTarget<TTarget>() where TTarget : Target
        {
            foreach (var target in Targets)
            {
                if (target is TTarget targetOfType)
                {
                    return targetOfType;
                }
            }
            return null;
        }
        
        public Target GetTarget()
        {
            foreach (var target in Targets)
            {
                if (target is Target targetOfType)
                {
                    return targetOfType;
                }
            }
            return null;
        }

        public float GetDistanceToTarget()
        {
            var target = GetTarget();
            return Vector3.Distance(_entity.transform.position, target.Position.Value);
        }
        
        public bool HasReachedTarget(float threshold = 0.1f)
        {
            var target = GetTarget();
            if (target == null || _entity == null || target.Position == null)
                return false;
    
            var sqrThreshold = threshold * threshold;
            var sqrDistance = (_entity.transform.position - target.Position.Value).sqrMagnitude;
    
            return sqrDistance <= sqrThreshold;
        }
        
        public Target GetTarget(Type type)
        {
            foreach (var target in Targets)
            {
                if (target.GetType() == type)
                {
                    return target;
                }
            }
            return null;
        }
        
        public void Tick()
        {
            ProcessSensors();
            
            if (_currentSequence != null)
            {
                _currentSequence.Tick(Time.deltaTime);
            }
        }

        private void ProcessSensors()
        {
            _currentSensors.Clear();
            
            foreach (var stateData in StateDatas)
            {
                CheckSensors(stateData.Sensors);
            }
            
            var matchingStates = new List<StateData<TEntity>>();
            
            foreach (var stateData in StateDatas)
            {
                int count = 0;
                foreach (var (id, sensor) in _currentSensors)
                {
                    if (stateData.SensorsById.ContainsKey(id))
                    {
                        count++;
                    }
                }

                if (stateData.SensorsById.Count == count)
                {
                    matchingStates.Add(stateData);
                }
            }
            
            if (matchingStates.Count > 0)
            {
                var highestPriorityState = matchingStates
                    .OrderByDescending(s => s.Priority)
                    .First();
                
                TryChangeState(new Sequence<TEntity>(highestPriorityState.Actions));
            }
        }

        private void CheckSensors(List<Sensor<TEntity>> sensors)
        {
            foreach (var sensor in sensors)
            {
                if (sensor != null)
                {
                    if (!_currentSensors.ContainsKey(sensor.Id))
                    {
                        if (sensor.Check())
                        {
                            _currentSensors.Add(sensor.Id, sensor);
                        }
                    }
                }
            }
        }

        public void TryChangeState(Sequence<TEntity> sequence)
        {
            if (_currentSequence == sequence)
                return;
            
            _currentSequence = sequence;
            _currentSequence.Execute().Forget();

            Debug.Log($"State changed to: {(sequence != null ? sequence.GetType().Name : "None")}");
        }
    }
}