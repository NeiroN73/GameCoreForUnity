using System;

namespace Game.Scripts.Game.Tutorial.Triggers
{
    [Serializable]
    public abstract class TutorialTrigger
    {
        public event Action<TutorialTrigger> OnTriggered;
        public bool IsTriggered { get; protected set; }

        public abstract void Check();

        protected void Trigger()
        {
            IsTriggered = true;
            OnTriggered?.Invoke(this);
        }

        public virtual void Reset()
        {
            IsTriggered = false;
        }
    }
}