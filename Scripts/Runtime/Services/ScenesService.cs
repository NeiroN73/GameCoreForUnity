using Cysharp.Threading.Tasks;
using GameCore.Utils;
using UnityEngine.SceneManagement;

namespace GameCore.Services
{
    public class ScenesService : Service
    {
        private readonly Subject _sceneChanged = new();
        public IObservable SceneChanged => _sceneChanged;
        
        public async UniTask LoadSceneAsync(string name, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            await SceneManager.LoadSceneAsync(name, loadSceneMode);
            _sceneChanged.OnNext();
        }
    }
}