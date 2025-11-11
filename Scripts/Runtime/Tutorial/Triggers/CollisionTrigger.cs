using System;
using UnityEngine;

namespace Game.Scripts.Game.Tutorial.Triggers
{
    [Serializable]
    public class CollisionTrigger : TutorialTrigger
    {
        [SerializeField] private int Test;
        public override void Check()
        {
            Trigger();
        }
    }
}