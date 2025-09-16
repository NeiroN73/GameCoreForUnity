using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameCore.SensorActionMachine
{
    public abstract class SensorActionMachine
    {
        protected abstract List<SensorActionState> StateDatas { get; set;  }
        protected abstract Blackboard Blackboard { get; set; }
        
        private SequenceActions _currentSequenceActions;
        private Dictionary<string, Sensor> _currentSensors = new();

        private MonoBehaviour _entity;
        
        public virtual void Initialize(MonoBehaviour entity)
        {
            _entity = entity;
            
            foreach (var stateData in StateDatas)
            {
                foreach (var parallelAction in stateData.ActionSequence.ParallelActions)
                {
                    foreach (var action in parallelAction.Actions)
                    {
                        action.Initialize(entity, this, Blackboard);
                    }
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
        
        public bool HasReachedTarget(Vector3 targetPosition, float threshold = 0.1f)
        {
            if (_entity == null)
                return false;
    
            var sqrThreshold = threshold * threshold;
            var sqrDistance = (_entity.transform.position - targetPosition).sqrMagnitude;
    
            return sqrDistance <= sqrThreshold;
        }
        
        public void Tick()
        {
            ProcessSensors();
            
            if (_currentSequenceActions != null)
            {
                _currentSequenceActions.Tick(Time.deltaTime);
            }
        }

        private void ProcessSensors()
        {
            _currentSensors.Clear();
            
            foreach (var stateData in StateDatas)
            {
                CheckSensors(stateData.Sensors);
            }
            
            var matchingStates = new List<SensorActionState>();
            
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
                
                TryChangeState(highestPriorityState.ActionSequence);
            }
        }

        private void CheckSensors(List<Sensor> sensors)
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

        public void TryChangeState(SequenceActions sequenceActions)
        {
            if (_currentSequenceActions == sequenceActions)
                return;
            
            _currentSequenceActions = sequenceActions;
            _currentSequenceActions.Execute().Forget();

            Debug.Log($"State changed to: {(sequenceActions != null ? sequenceActions.GetType().Name : "None")}");
        }
    }
}