using System;
using GameCore.ReactiveObservers;
using TMPro;
using UnityEngine;

namespace GameCore.UI
{
    [Serializable]
    public class InputFieldBinder : Binder
    {
        [SerializeField] private TMP_InputField _inputField;
        
        public ReactiveObserver<string> TextChangeInputed = new();
        public ReactiveObserver<string> TextChanged = new();
        
        public override void Initialize()
        {
            _inputField.onValueChanged.AddListener(OnTextChangeInputed);
            TextChanged.Subscribe(OnTextChanged);
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _inputField.onValueChanged.RemoveListener(OnTextChangeInputed);
        }

        private void OnTextChangeInputed(string value)
        {
            TextChangeInputed.Execute(value);
        }

        private void OnTextChanged(string value)
        {
            _inputField.text = value;
        }
    }
}