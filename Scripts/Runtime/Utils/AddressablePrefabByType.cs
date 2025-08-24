using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TriInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameCore.Utils
{
    [Serializable]
    public class AddressablePrefabByType<TFilter> where TFilter : class
    {
        [Group("Type")][HideLabel][SerializeField][Dropdown(nameof(GetTypes))] private string type;

        [Group("Asset")][HideLabel][SerializeField] private AssetReferenceGameObject asset;
        
        public Type Type => TypeExtensions.GetType(type);
        public AssetReferenceGameObject Asset => asset;

        [UsedImplicitly]
        private IEnumerable<string> GetTypes() => TypeExtensions.FilterTypes<TFilter>();
    }
}