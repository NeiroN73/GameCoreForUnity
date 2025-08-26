using System;
using UnityEngine;

namespace Game.Scripts.Game.Tutorial.Triggers
{
    [Serializable]
    public class ButtonTrigger : TutorialTrigger
    {
        [SerializeField] private string Str;
        [SerializeField] private int Test2;
        public override void Check()
        {
            
        }
    }
}