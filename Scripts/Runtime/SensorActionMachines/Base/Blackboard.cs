using System;
using System.Collections.Generic;
using System.Reflection;
using TriInspector;

namespace GameCore.SensorActionMachine
{
    [Serializable]
    public class Blackboard
    {
        public BlackboardField<float> Distance = new(nameof(Distance));
        public BlackboardField<int> Test = new(nameof(Test));

        private readonly Dictionary<string, BlackboardField> _fields = new();

        public Blackboard()
        {
            Add(Distance, Test);
        }
        
        public T GetValue<T>(string fieldName)
        {
            return _fields.TryGetValue(fieldName, out var field) && field is BlackboardField<T> typedField
                ? typedField.Value
                : default;
        }

        public Dictionary<string, BlackboardField> Add(params BlackboardField[] fields)
        {
            foreach (var field in fields)
            {
                _fields.Add(field.Name, field);
            }

            return _fields;
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
    
    [Serializable]
    public class BlackboardField
    {
        public string Name;
    }
    
    public static class BlackboardFields
    {
        public static List<string> Fields = new();
        
        [UnityEditor.Callbacks.DidReloadScripts]
        public static void CompileBlackboardFields()
        {
            var fields = typeof(Blackboard).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (typeof(BlackboardField).IsAssignableFrom(field.FieldType))
                {
                    Fields.Add(field.Name); 
                }
            }
        }
    }

    [Serializable]
    public class BlackboardDropdown
    {
        [Dropdown(nameof(GetFields))] public string FieldName;
        private List<string> GetFields() => BlackboardFields.Fields;
    }
}