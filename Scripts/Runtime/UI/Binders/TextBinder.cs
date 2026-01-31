using System;
using TMPro;
using UnityEngine;
using GameCore.ReactiveObservers;
using R3;

namespace GameCore.UI
{
    [Serializable]
    public class TextBinder : Binder
    {
        [SerializeField] private TMP_Text _testText;

        public ReactiveObserver<string> TestTextChanged = new();

        public override void Initialize()
        {
            TestTextChanged.Subscribe(OnTextChanged).AddTo(Disposable);
        }

        private void OnTextChanged(string newText)
        {
            _testText.text = newText;
        }
    }
}