using System.Collections.Generic;
using GameCore.Creatures;
using GameCore.Utils;
using TriInspector;
using UnityEngine;

namespace GameCore.Configs
{
    [CreateAssetMenu(fileName = "CreaturesConfig", menuName = "Configs/CreaturesConfig")]
    public class CreaturesConfig : Config
    {
        [TableList(Draggable = true, HideAddButton = false, HideRemoveButton = false, AlwaysExpanded = false)]
        [SerializeField] private List<AddressablePrefabByType<ICreature>> _creatures;

        public IReadOnlyList<AddressablePrefabByType<ICreature>> Creatures => _creatures;
    }
}