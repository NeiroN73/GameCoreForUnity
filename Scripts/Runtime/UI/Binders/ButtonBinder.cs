using System;
using GameCore.ReactiveObservers;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI
{
    [Serializable]
    public class ButtonBinder : Binder
    {
        [SerializeField] private Button _button;

        public ReactiveObserver Clicked = new();
        
        public override void Initialize()
        {
            _button.onClick.AddListener(OnClicked);
        }
        
        public override void Dispose()
        {
            base.Dispose();
            
            _button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked()
        {
            Clicked.Execute();
        }
    }
}