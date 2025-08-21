using Content.Scripts.Configs;
using Content.Scripts.UI.Loading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using VContainer;

namespace Content.Scripts.Services
{
    public class ScenesService : Service
    {
        [Inject] private ScreensService _screensService;
        
        public async UniTask LoadSceneAsync(string name, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            _screensService.OpenLoading<LoadingScreen>();
            await Addressables.LoadSceneAsync(name, loadSceneMode);
            _screensService.CloseLoading();
        }
    }
}