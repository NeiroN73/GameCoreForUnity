using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Game.Tutorial.Actions;
using Game.Scripts.Game.Tutorial.Triggers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Scripts.Game.Tutorial
{
    public class TutorialService : IInitializable, IDisposable
    {
        private TutorialConfig _tutorialConfig;
        //private DialoguesService _dialogueService;
        private IObjectResolver _objectResolver;
        
        private int _currentStepIndex = -1;
        private TutorialStep _currentStep;
        private bool _isWaitingForTriggers;
        private List<bool> _triggerStatuses;

        public TutorialService(TutorialConfig tutorialConfig, /*DialoguesService dialogueService,*/
            IObjectResolver objectResolver)
        {
            _tutorialConfig = tutorialConfig;
            //_dialogueService = dialogueService;
            _objectResolver = objectResolver;
        }
        
        public void Initialize()
        {
            //_dialogueService.OnDialogueEnd += OnDialogueEnd;
            StartTutorial();
        }

        public void Dispose()
        {
            //_dialogueService.OnDialogueEnd -= OnDialogueEnd;
            UnsubscribeFromTriggers();
        }

        public void StartTutorial()
        {
            if (_tutorialConfig.TutorialSteps.Count == 0) return;
            _currentStepIndex = 0;
            ExecuteStep(_currentStepIndex);
        }

        private void ExecuteStep(int stepIndex)
        {
            _currentStep = _tutorialConfig.TutorialSteps[stepIndex];
            foreach (var Trigger in _currentStep.Triggers)
            {
                _objectResolver.Inject(Trigger);
            }
            foreach (var Action in _currentStep.Actions)
            {
                _objectResolver.Inject(Action);
            }
            
            // Initialize trigger status tracking
            _triggerStatuses = new List<bool>();
            if (_currentStep.Triggers != null)
            {
                _triggerStatuses.AddRange(new bool[_currentStep.Triggers.Count]);
            }

            // Show dialogues if any
            // if (_currentStep.explanationLines != null && _currentStep.explanationLines.Count > 0)
            // {
            //     _dialogueService.StartDialogue(_currentStep.explanationLines);
            // }
            // else
            // {
            //     // If no dialogues, start checking triggers immediately
            //     CheckTriggers();
            // }
        }

        private void OnDialogueEnd()
        {
            CheckTriggers();
        }

        private void CheckTriggers()
        {
            if (_currentStep == null || _currentStep.Triggers == null || _currentStep.Triggers.Count == 0)
            {
                // If no triggers, execute actions immediately
                ExecuteActions();
                return;
            }

            // Check all triggers
            for (int i = 0; i < _currentStep.Triggers.Count; i++)
            {
                var trigger = _currentStep.Triggers[i];
                trigger.Check();
                
                // We need a way to get trigger status - this depends on your trigger implementation
                // For now assuming triggers have an IsTriggered property
                _triggerStatuses[i] = trigger.IsTriggered;
            }

            // If all triggers are completed
            if (_triggerStatuses.All(status => status))
            {
                ExecuteActions();
            }
            else
            {
                // Start waiting for triggers
                _isWaitingForTriggers = true;
                SubscribeToTriggers();
            }
        }

        private void SubscribeToTriggers()
        {
            if (_currentStep?.Triggers == null) return;

            foreach (var trigger in _currentStep.Triggers)
            {
                // Assuming triggers have an event that fires when they're triggered
                trigger.OnTriggered += OnTriggerCompleted;
            }
        }

        private void UnsubscribeFromTriggers()
        {
            if (_currentStep?.Triggers == null) return;
            
            foreach (var trigger in _currentStep.Triggers)
            {
                trigger.OnTriggered -= OnTriggerCompleted;
            }
            _isWaitingForTriggers = false;
        }

        private void OnTriggerCompleted(TutorialTrigger triggeredTrigger)
        {
            if (!_isWaitingForTriggers || _currentStep == null) return;

            // Update the status of the triggered trigger
            int triggerIndex = _currentStep.Triggers.IndexOf(triggeredTrigger);
            if (triggerIndex >= 0)
            {
                _triggerStatuses[triggerIndex] = true;
            }

            // Check if all triggers are now completed
            if (_triggerStatuses.All(status => status))
            {
                UnsubscribeFromTriggers();
                ExecuteActions();
            }
        }

        private void ExecuteActions()
        {
            if (_currentStep.Actions != null)
            {
                foreach (var action in _currentStep.Actions)
                {
                    action.Execute();
                }
            }

            MoveToNextStep();
        }

        private void MoveToNextStep()
        {
            _currentStepIndex++;
            
            if (_currentStepIndex < _tutorialConfig.TutorialSteps.Count)
            {
                ExecuteStep(_currentStepIndex);
            }
            else
            {
                TutorialFinished();
            }
        }

        private void TutorialFinished()
        {
            // Tutorial completion logic
            _currentStep = null;
            _currentStepIndex = -1;
            Debug.Log("Tutorial completed!");
        }

        public void CompleteCurrentStep()
        {
            // Force complete current step
            UnsubscribeFromTriggers();
            ExecuteActions();
        }
    }
    
    [Serializable]
    public class TutorialStep
    {
        public string stepId;
        //public List<DialogueLine> explanationLines;
        [SerializeReference] public List<TutorialTrigger> Triggers;
        [SerializeReference] public List<TutorialAction> Actions;

        public TutorialStep()
        {
            
        }
    }
}