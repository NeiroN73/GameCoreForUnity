using System;
using System.Collections.Generic;
using System.Reflection;
using TriInspector;

namespace GameCore.SensorActionMachine
{
    public class Blackboard
    {
        private readonly Dictionary<string, BlackboardField> _fields = new();
        
        public BlackboardField<T> GetField<T>(string fieldName)
        {
            return _fields.TryGetValue(fieldName, out var field) && field is BlackboardField<T> typedField
                ? typedField
                : default;
        }

        protected void SetFields(params BlackboardField[] fields)
        {
            _fields.Clear();
            
            foreach (var field in fields)
            {
                _fields.Add(field.Name, field);
            }
        }
    }

    public class BlackboardField<T> : BlackboardField
    {
        public T Value;
        public BlackboardField(string name)
        {
            Name = name;
        }
    }
    
    public class BlackboardField
    {
        public string Name;
    }
    
    public static class BlackboardFields
    {
        private static readonly Dictionary<Type, List<string>> _cachedFields = new();
    
        public static List<string> GetFields(Type blackboardType)
        {
            if (_cachedFields.TryGetValue(blackboardType, out var cachedFields))
            {
                return cachedFields;
            }
        
            var fields = new List<string>();
            var instanceFields = blackboardType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        
            foreach (var field in instanceFields)
            {
                if (typeof(BlackboardField).IsAssignableFrom(field.FieldType))
                {
                    fields.Add(field.Name);
                }
            }
        
            _cachedFields[blackboardType] = fields;
            return fields;
        }
    
        [UnityEditor.Callbacks.DidReloadScripts]
        public static void ClearCache()
        {
            _cachedFields.Clear();
        }
    }

    [Serializable]
    public abstract class BlackboardDropdown
    {
        [Dropdown(nameof(GetFields))] public string FieldName;
        protected abstract List<string> GetFields();
    }
}