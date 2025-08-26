using System;
using Game.Scripts.Utils;

namespace Game.Scripts.Game.SensorStateMachines.Enemy.Sensors
{
    public static class ConditionEvaluator
    {
        public static bool Evaluate<T>(T value, T referenceValue, Condition condition) 
            where T : IComparable<T>
        {
            int comparison = value.CompareTo(referenceValue);

            return condition switch
            {
                Condition.LessThan => comparison < 0,
                Condition.LessOrEqual => comparison <= 0,
                Condition.Equal => comparison == 0,
                Condition.GreaterOrEqual => comparison >= 0,
                Condition.GreaterThan => comparison > 0,
                _ => false
            };
        }
    }
}