using Cysharp.Threading.Tasks;
using GameCore.Factories;
using GameCore.UI.Loading;
using GameCore.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace GameCore.Services
{
    public class ScenesService : Service
    {
        [Inject] private ScreensService _screensService;
        [Inject] private ScreensFactory _screensFactory;

        private readonly Subject _sceneChanged = new();
        public IObservable SceneChanged => _sceneChanged;
        
        public async UniTask LoadSceneAsync(string name, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            _screensService.OpenLoading<LoadingScreen>();
            await SceneManager.LoadSceneAsync(name, loadSceneMode);
            _sceneChanged.OnNext();
            _screensService.CloseLoading();
        }
    }
}