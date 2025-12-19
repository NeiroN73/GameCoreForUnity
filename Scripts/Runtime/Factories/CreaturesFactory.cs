using GameCore.Creatures;
using GameCore.Configs;
using VContainer;
using System;
using System.Collections.Generic;
using GameCore.Services;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameCore.Factories
{
    public class CreaturesFactory : Factory, IInitializable
    {
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private BehavioursConfig behavioursConfig;
        [Inject] private AssetsLoaderService _assetsLoaderService;

        private Dictionary<string, ICreature> _creaturesById;
        private Dictionary<Type, ICreature> _creaturesByType;

        public void Initialize()
        {
            _creaturesById = new();
            _creaturesByType = new();
        
            foreach (var handler in behavioursConfig.Behaviours)
            {
                if (handler == null) continue;

                var asset = _assetsLoaderService.LoadAssetSync<ICreature>(handler.Asset); //TODO: сделать прелоадом
                
                if (!string.IsNullOrEmpty(asset.Id))
                    _creaturesById[asset.Id] = asset;
                
                _creaturesByType[asset.GetType()] = asset;
            }
        }
        
        public TCreature Create<TCreature>(Vector3 position = default, Quaternion rotation = default, Transform parent = null) 
            where TCreature : MonoBehaviour, ICreature
        {
            var prefab = GetCreature<TCreature>();
            if (prefab == null)
                throw new InvalidOperationException($"Creature of type {typeof(TCreature)} not found in config");

            return Create(prefab, position, rotation, parent);
        }

        public TCreature CreateById<TCreature>(string id, Vector3 position = default, Quaternion rotation = default, Transform parent = null) 
            where TCreature : MonoBehaviour, ICreature
        {
            var prefab = GetCreatureById<TCreature>(id);
            if (prefab == null)
                throw new InvalidOperationException($"Creature with ID '{id}' and type {typeof(TCreature)} not found in config");

            return Create(prefab, position, rotation, parent);
        }

        public TCreature Create<TCreature>(TCreature prefab, Vector3 position = default, Quaternion rotation = default, Transform parent = null) 
            where TCreature : MonoBehaviour, ICreature
        {
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            var creature = Object.Instantiate(prefab, position, rotation, parent);
            InitializeCreature(creature);
            return creature;
        }
        
        private TCreature GetCreature<TCreature>() where TCreature : MonoBehaviour
        {
            if (_creaturesByType.TryGetValue(typeof(TCreature), out var creature) && 
                creature is TCreature typedCreature)
            {
                return typedCreature;
            }
            return null;
        }
        
        private TCreature GetCreatureById<TCreature>(string id) where TCreature : MonoBehaviour
        {
            if (string.IsNullOrEmpty(id)) return null;
        
            if (_creaturesById.TryGetValue(id, out var creature) && 
                creature is TCreature typedCreature)
            {
                return typedCreature;
            }
            return null;
        }
        
        public void InitializeCreature<TCreature>(TCreature creature)
            where TCreature : MonoBehaviour, ICreature
        {
            _objectResolver.Inject(creature);
        }
    }
}