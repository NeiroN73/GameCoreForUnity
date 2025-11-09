using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameCore.Factories;
using GameCore.UI;
using GameCore.UI.Loading;
using VContainer;

namespace GameCore.Services
{
    public class ScreensService : Service, IInitializable
    {
        [Inject] private readonly ScreensFactory _screensFactory;
        [Inject] private readonly ScenesService _scenesService;
        
        private readonly Dictionary<Type, View> _screensByType = new();
        private readonly Stack<View> _screensStack = new();
        
        private View _loadingScreen;

        public void Initialize()
        {
            _scenesService.SceneChanged.Subscribe(DestroyScreens);
        }
        
        public async UniTask<TScreen> OpenAsync<TScreen>() where TScreen : View
        {
            if (_screensByType.TryGetValue(typeof(TScreen), out var screen))
            {
                if (screen)
                {
                    screen.Open();
                    _screensStack.Push(screen);
                    return (TScreen)screen;
                }
            }
            
            OpenLoading<LoadingScreen>();
            var newScreen = await _screensFactory.CreateAsync<TScreen>();
            CloseLoading();
            
            newScreen.Open();
            _screensByType[typeof(TScreen)] = newScreen;
            _screensStack.Push(newScreen);
    
            return newScreen;
        }

        public void OpenLoading<TScreen>() where TScreen : View
        {
            if (_screensByType.TryGetValue(typeof(TScreen), out var screen))
            {
                _loadingScreen = screen;
            }
            else
            {
                _loadingScreen = _screensFactory.CreateSync<TScreen>();
                _screensByType[typeof(TScreen)] = _loadingScreen;
            }

            if (_loadingScreen)
            {
                _loadingScreen.Open();
            }
        }

        public void CloseLoading()
        {
            if (_loadingScreen)
            {
                _loadingScreen.Close();
            }
        }

        public void Close()
        {
            if (_screensStack.TryPop(out var screen))
            {
                screen.Close();
                screen.gameObject.SetActive(false);
            }
        }
        
        public void DestroyScreens()
        {
            foreach (var screenPair in _screensByType)
            {
                var screen = screenPair.Value;
                if (screen == null) continue;

                screen.Close();
        
                if (screen is IDisposable disposable)
                {
                    disposable.Dispose();
                }
        
                if (screen.gameObject != null)
                {
                    UnityEngine.Object.Destroy(screen.gameObject);
                }
            }
    
            _screensByType.Clear();
            _screensStack.Clear();
        }
    }
}
