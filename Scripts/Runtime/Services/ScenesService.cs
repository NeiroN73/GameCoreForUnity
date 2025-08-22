using Cysharp.Threading.Tasks;
using GameCore.UI.Loading;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using VContainer;

namespace GameCore.Services
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