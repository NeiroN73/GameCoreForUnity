using System.Collections.Generic;
using Game.Scripts.Game.Tutorial.Triggers;
using UnityEngine;

namespace Game.Scripts.Game.Tutorial
{
    [CreateAssetMenu(fileName = "TutorialConfig", menuName = "Config/TutorialConfig", order = 0)]
    public class TutorialConfig : ScriptableObject
    {
        [field: SerializeField] public List<TutorialStep> TutorialSteps { get; private set; }
    }
}