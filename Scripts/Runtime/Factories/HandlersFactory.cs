using GameCore.Handlers;
using GameCore.Configs;
using VContainer;
using System;
using System.Collections.Generic;
using GameCore.Services;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameCore.Factories
{
    public class HandlersFactory : Factory, IInitializable
    {
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private HandlersConfig _handlersConfig;
        [Inject] private AssetsLoaderService _assetsLoaderService;

        private Dictionary<string, IHandlerable> _handlersById;
        private Dictionary<Type, IHandlerable> _handlersByType;

        public void Initialize()
        {
            _handlersById = new();
            _handlersByType = new();
        
            foreach (var handler in _handlersConfig.Handlers)
            {
                if (handler == null) continue;

                var asset = _assetsLoaderService.LoadAssetSync<IHandlerable>(handler.Asset); //TODO: сделать прелоадом
                
                if (!string.IsNullOrEmpty(asset.Id))
                    _handlersById[asset.Id] = asset;
                
                _handlersByType[asset.GetType()] = asset;
            }
        }
        
        public THandler Create<THandler>(Vector3 position = default, Quaternion rotation = default, Transform parent = null) 
            where THandler : MonoBehaviour, IHandlerable
        {
            var prefab = GetHandler<THandler>();
            if (prefab == null)
                throw new InvalidOperationException($"Handler of type {typeof(THandler)} not found in config");

            return Create(prefab, position, rotation, parent);
        }

        public THandler CreateById<THandler>(string id, Vector3 position = default, Quaternion rotation = default, Transform parent = null) 
            where THandler : MonoBehaviour, IHandlerable
        {
            var prefab = GetHandlerById<THandler>(id);
            if (prefab == null)
                throw new InvalidOperationException($"Handler with ID '{id}' and type {typeof(THandler)} not found in config");

            return Create(prefab, position, rotation, parent);
        }

        public THandler Create<THandler>(THandler prefab, Vector3 position = default, Quaternion rotation = default, Transform parent = null) 
            where THandler : MonoBehaviour, IHandlerable
        {
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            var handler = Object.Instantiate(prefab, position, rotation, parent);
            InitializeHandler(handler);
            return handler;
        }
        
        private THandler GetHandler<THandler>() where THandler : MonoBehaviour
        {
            if (_handlersByType.TryGetValue(typeof(THandler), out var handler) && 
                handler is THandler typedHandler)
            {
                return typedHandler;
            }
            return null;
        }
        
        private  THandler GetHandlerById<THandler>(string id) where THandler : MonoBehaviour
        {
            if (string.IsNullOrEmpty(id)) return null;
        
            if (_handlersById.TryGetValue(id, out var handler) && 
                handler is THandler typedHandler)
            {
                return typedHandler;
            }
            return null;
        }
        
        public void InitializeHandler<THandler>(THandler handler)
            where THandler : MonoBehaviour, IHandlerable
        {
            _objectResolver.Inject(handler);
        }
    }
}