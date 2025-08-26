using System;
using UnityEngine;

namespace Game.Scripts.Game.Tutorial.Actions
{
    [Serializable]
    public abstract class TutorialAction
    {
        public abstract void Execute();
    }
}