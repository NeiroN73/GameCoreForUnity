using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TriInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using TypeExtensions = Content.Scripts.Utils.TypeExtensions;

namespace Content.Scripts.Configs
{
    [Serializable]
    public class AddressablePrefabByType<TFilter> where TFilter : class
    {
        [Group("Type")][HideLabel][SerializeField][Dropdown(nameof(GetTypes))] private string type;

        [Group("Asset")][HideLabel][SerializeField] private AssetReferenceGameObject asset;
        
        public Type Type => TypeExtensions.GetType(type);
        public AssetReferenceGameObject Asset => asset;

#if UNITY_EDITOR

        [UsedImplicitly]
        private IEnumerable<string> GetTypes() => TypeExtensions.FilterTypes<TFilter>();

#endif
    }
}