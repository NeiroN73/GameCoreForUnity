using System.Collections.Generic;
using System.Linq;
using GameCore.Services;
using UnityEngine;
using VContainer;

namespace Content.Scripts.Installers
{
    public class CoreController : MonoBehaviour
    {
        [Inject] private ScenesService _scenesService;
        
        private IInitializable[] _initializables;
        private ISceneChangable[] _sceneChangables;
        private ITickable[] _tickables;

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            _initializables = resolver.Resolve<IEnumerable<IInitializable>>().ToArray();
            _sceneChangables = resolver.Resolve<IEnumerable<ISceneChangable>>().ToArray();
            _tickables = resolver.Resolve<IEnumerable<ITickable>>().ToArray();
        }

        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);

            foreach (var initializable in _initializables)
            {
                initializable.Initialize();
            }

            _scenesService.SceneChanged.Subscribe(SceneChanged);
        }

        private void SceneChanged()
        {
            foreach (var sceneChangable in _sceneChangables)
            {
                sceneChangable.SceneChanged();
            }
        }

        private void Update()
        {
            foreach (var tickable in _tickables)
            {
                tickable.Tick(Time.deltaTime);
            }
        }
    }
}