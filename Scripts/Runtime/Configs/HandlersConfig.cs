using System.Collections.Generic;
using GameCore.Handlers;
using GameCore.Utils;
using TriInspector;
using UnityEngine;

namespace GameCore.Configs
{
    [CreateAssetMenu(fileName = "HandlersConfig", menuName = "Configs/HandlersConfig")]
    public class HandlersConfig : Config
    {
        [TableList(Draggable = true, HideAddButton = false, HideRemoveButton = false, AlwaysExpanded = false)]
        [SerializeField] private List<AddressablePrefabByType<IHandlerable>> _handlers;

        public IReadOnlyList<AddressablePrefabByType<IHandlerable>> Handlers => _handlers;
    }
}