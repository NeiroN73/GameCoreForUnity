using System;
using TMPro;
using UnityEngine;

namespace Content.Scripts.UI.Binders
{
    [Serializable]
    public class TextViewBinder : ViewBinder<string>
    {
        [SerializeField] private TMP_Text _testText;
        
        public TextViewBinder(string id) : base(id)
        {

        }
        
        public override void Parse(string value)
        {
            _testText.text = value;
        }
    }
}