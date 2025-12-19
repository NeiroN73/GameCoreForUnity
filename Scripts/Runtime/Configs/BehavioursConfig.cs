using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameCore.Configs
{
    [CreateAssetMenu(fileName = "BehavioursConfig", menuName = "Configs/BehavioursConfig")]
    public class BehavioursConfig : Config
    {
        [TableList(Draggable = true, HideAddButton = false, HideRemoveButton = false, AlwaysExpanded = false)]
        [SerializeField] private List<AddressablePrefabById> _behaviours;

        public IReadOnlyList<AddressablePrefabById> Behaviours => _behaviours;
    }

    [Serializable]
    public class AddressablePrefabById
    {
        [Group("Id")][HideLabel][SerializeField]/*[Dropdown(nameof(GetTypes))]*/ public string Id; //todo сделай систему айдишников

        [Group("Asset")][HideLabel][SerializeField] public AssetReferenceGameObject Asset;
    }
}