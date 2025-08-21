using System;
using System.Collections.Generic;
using Content.Scripts.Factories;
using Content.Scripts.UI.Base;
using Content.Scripts.UI.Loading;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Content.Scripts.Services
{
    public class ScreensService : Service
    {
        [Inject] private readonly ScreensFactory _screensFactory;
        
        private readonly Dictionary<Type, View> _screensByType = new();
        private readonly Stack<View> _screensStack = new();
        
        public View CurrentScreen { get; private set; }
        private View _loadingScreen;

        public async UniTask<TScreen> OpenAsync<TScreen>() where TScreen : View
        {
            if (_screensByType.TryGetValue(typeof(TScreen), out var screen))
            {
                screen.Open();
                _screensStack.Push(screen);
                CurrentScreen = screen;
                return (TScreen)screen;
            }
            
            OpenLoading<LoadingScreen>();
            var newScreen = await _screensFactory.CreateAsync<TScreen>();
            CloseLoading();
            
            newScreen.Open();
            _screensByType.Add(typeof(TScreen), newScreen);
            _screensStack.Push(newScreen);
            CurrentScreen = newScreen;
    
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
                _screensByType.Add(typeof(TScreen), _loadingScreen);
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
                if (_screensStack.Count > 0)
                {
                    CurrentScreen = _screensStack.Peek();
                }
            }
        }
    }
}
