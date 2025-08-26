using System;
using UnityEngine;

namespace Game.Scripts.Game.Tutorial.Triggers
{
    [Serializable]
    [CreateAssetMenu(fileName = "CollisionTrigger", menuName = "Config/Tutorial/Triggers/CollisionTrigger")]
    public class CollisionTrigger : TutorialTrigger
    {
        [SerializeField] private int Test;
        public override void Check()
        {
            Trigger();
        }
    }
}