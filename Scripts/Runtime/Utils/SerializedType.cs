using System;
using System.Collections.Generic;
using Content.Scripts.Utils;
using JetBrains.Annotations;
using TriInspector;
using UnityEngine;

namespace Game.Scripts.Utils
{
    [Serializable]
    public class SerializedType<TFilter> where TFilter : class
    {
        [Group("Type")][HideLabel][SerializeField][Dropdown(nameof(GetTypes))] private string type;

        public Type Type => Type.GetType(type);

#if UNITY_EDITOR
        [UsedImplicitly]
        private IEnumerable<string> GetTypes() => TypeExtensions.FilterTypes<TFilter>();
#endif
    }
}