using System.Collections.Generic;
using UnityEngine;

namespace GameCore.GoogleSheetsImporter
{
    [CreateAssetMenu(fileName = "GoogleSheetsConfig", menuName = "Configs/GoogleSheetsConfig")]
    public class GoogleSheetsConfig : ScriptableObject
    {
        [field: SerializeField] public List<string> _sheets { get; private set; } = new();
    }
}