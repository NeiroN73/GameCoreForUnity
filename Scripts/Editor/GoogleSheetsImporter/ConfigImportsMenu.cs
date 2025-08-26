using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Editor.GoogleSheetsImporter
{
    public class ConfigImportsMenu
    {
        private const string SPREADSHEET_ID = "1DzvCF5Dc0Ms7EF5b0sKsmE_evB1pkJjyR-8RmKpQXJk";
        private const string ENEMY_SHEET_NAME = "Enemy";
        private const string CREDENTIALS_PATH = "Assets/Game/GoogleSheetsCredentials/thismineismine-4ebff6dec14d.json";
        
        private const string FileName = "configs.json";
        private static string _path = "Assets/Game/GoogleSheetsCredentials/configs.json";
        
        // [MenuItem("ThisMineIsMine/ImportGoogleSheets")]
        // private static async void ImportGoogleSheets()
        // {
        //     var sheetsImporter = new GoogleSheetsImporter(CREDENTIALS_PATH, SPREADSHEET_ID);
        //     var configs = LoadConfigs();
        //     var parser = new EnemyParser(configs);
        //     await sheetsImporter.DownloadAndParseSheet(ENEMY_SHEET_NAME, parser);
        //     SaveConfigs(configs);
        // }
        //
        // private static GoogleSheetsConfigs LoadConfigs()
        // {
        //     if (!File.Exists(_path))
        //         return new GoogleSheetsConfigs();
        //
        //     string json = File.ReadAllText(_path);
        //     return JsonUtility.FromJson<GoogleSheetsConfigs>(json) ?? new GoogleSheetsConfigs();
        // }
        //
        // private static void SaveConfigs(GoogleSheetsConfigs configs)
        // {
        //     string json = JsonUtility.ToJson(configs, true);
        //     File.WriteAllText(_path, json);
        // }
    }
}