using System.Collections.Generic;
using Content.Scripts.Configs.Base;
using Content.Scripts.UI.Base;
using TriInspector;
using UnityEngine;

namespace Content.Scripts.Configs
{
    [CreateAssetMenu(fileName = "ScreensConfig", menuName = "Configs/ScreensConfig")]
    public class ScreensConfig : Config
    {
        [field: SerializeField] public Transform Root { get; private set; }

        [TableList(Draggable = true, HideAddButton = false, HideRemoveButton = false, AlwaysExpanded = false)]
        [SerializeField] private List<AddressablePrefabByType<View>> _screens;

        public IReadOnlyList<AddressablePrefabByType<View>> Screens => _screens;
    }
}