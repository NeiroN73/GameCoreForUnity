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
                : null;
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

    [Serializable]
    public class BlackboardField<T> : BlackboardField
    {
        public T Value;
        public BlackboardField(string name)
        {
            Name = name;
        }
    }
    
    [Serializable]
    public class BlackboardField
    {
        public string Name;
    }

    [Serializable]
    public class BlackboardFieldHandle<T, U> : BlackboardFieldHandle
    {
        [Dropdown(nameof(GetFields))] public string FieldName;

        protected List<string> GetFields()
        {
            return BlackboardFieldsUtils.GetFields(typeof(U));
        }
        
        private BlackboardField<T> _field;
        public T Value
        {
            get => _field.Value;
            set => _field.Value = value;
        }

        public override void Initialize(Blackboard blackboard)
        {
            _field = blackboard.GetField<T>(FieldName);
        }
    }

    public abstract class BlackboardFieldHandle
    {
        public abstract void Initialize(Blackboard blackboard);
    }
    
    public static class BlackboardFieldsUtils
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
    
#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        public static void ClearCache()
        {
            _cachedFields.Clear();
        }
#endif
    }
}