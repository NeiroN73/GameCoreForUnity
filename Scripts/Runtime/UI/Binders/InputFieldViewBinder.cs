using System;
using R3;
using TMPro;
using UnityEngine;

namespace Content.Scripts.UI.Binders
{
    [Serializable]
    public class InputFieldTextChangedViewBinder : InputFieldViewBinder<ReactiveCommand<string>>
    {
        private ReactiveCommand<string> _reactiveCommand;
        
        public InputFieldTextChangedViewBinder(string id) : base(id)
        {
        }
        
        public override void Parse(ReactiveCommand<string> value)
        {
            _reactiveCommand = value;
            JoinCodeInputField.onValueChanged.AddListener(OnChanged);
        }

        public override void Dispose()
        {
            base.Dispose();
            
            JoinCodeInputField.onValueChanged.RemoveListener(OnChanged);
            _reactiveCommand = null;
        }

        private void OnChanged(string value)
        {
            _reactiveCommand.Execute(value);
        }
    }
    
    [Serializable]
    public class InputFieldSetTextViewBinder : InputFieldViewBinder<string>
    {
        public InputFieldSetTextViewBinder(string id) : base(id)
        {
        }

        public override void Parse(string value)
        {
            JoinCodeInputField.text = value;
        }
    }
    
    [Serializable]
    public class InputFieldViewBinder<TValue> : ViewBinder<TValue>
    {
        [SerializeField] protected TMP_InputField JoinCodeInputField;
        
        public InputFieldViewBinder(string id) : base(id)
        {
        }

        public override void Parse(TValue value)
        {
            
        }
    }
}