using System.Collections.Generic;
using GameCore.UI;
using GameCore.Utils;
using TriInspector;
using UnityEngine;

namespace GameCore.Configs
{
    [CreateAssetMenu(fileName = "ScreensConfig", menuName = "Configs/ScreensConfig")]
    public class ScreensConfig : Config
    {
        [TableList(Draggable = true, HideAddButton = false, HideRemoveButton = false, AlwaysExpanded = false)]
        [SerializeField] private List<AddressablePrefabByType<View>> _screens;

        public IReadOnlyList<AddressablePrefabByType<View>> Screens => _screens;
    }
}