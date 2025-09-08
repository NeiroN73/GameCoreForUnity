// #if UNITY_EDITOR
// using System.Collections.Generic;
// using System.IO;
// using UnityEditor;
// using UnityEditor.AddressableAssets;
// using UnityEditor.AddressableAssets.Settings;
// using UnityEngine;
//
// namespace gta_mirror.Chunks
// {
//     public class AddressablesSizeCalculator
//     {
//         private static Dictionary<string, long> _sizeCache = new();
//
//         public static long GetAssetSizeOptimized(string addressableKey)
//         {
//             if (_sizeCache.TryGetValue(addressableKey, out long cachedSize))
//                 return cachedSize;
//
//             var settings = AddressableAssetSettingsDefaultObject.Settings;
//             if (settings == null) return 0;
//
//             var entry = FindAddressableEntry(settings, addressableKey);
//             if (entry == null || string.IsNullOrEmpty(entry.AssetPath)) return 0;
//
//             try
//             {
//                 string[] dependencies = AssetDatabase.GetDependencies(entry.AssetPath, true);
//                 long totalSize = 0;
//                 HashSet<string> processedPaths = new HashSet<string>();
//
//                 string mainPath = entry.AssetPath;
//                 long mainSize = GetFileSize(mainPath);
//                 totalSize += mainSize;
//                 processedPaths.Add(mainPath);
//
//                 foreach (var depPath in dependencies)
//                 {
//                     if (depPath == mainPath || processedPaths.Contains(depPath))
//                         continue;
//
//                     long depSize = GetFileSize(depPath);
//                     totalSize += depSize;
//                     processedPaths.Add(depPath);
//                 }
//
//                 _sizeCache[addressableKey] = totalSize;
//                 return totalSize;
//             }
//             catch
//             {
//                 return 0;
//             }
//         }
//
//         private static long GetFileSize(string assetPath)
//         {
//             string fullPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", assetPath));
//             if (File.Exists(fullPath))
//             {
//                 return new FileInfo(fullPath).Length;
//             }
//             return 0;
//         }
//
//         public static float BytesToMB(long bytes)
//         {
//             return bytes / (1024f * 1024f);
//         }
//
//         private static AddressableAssetEntry FindAddressableEntry(AddressableAssetSettings settings, string address)
//         {
//             try
//             {
//                 foreach (var group in settings.groups)
//                 {
//                     if (group == null) 
//                     {
//                         Debug.LogWarning("⚠Null group found");
//                         continue;
//                     }
//                 
//                     foreach (var entry in group.entries)
//                     {
//                         if (entry != null)
//                         {
//                             if (entry.address == address)
//                             {
//                                 return entry;
//                             }
//                         }
//                     }
//                 }
//             
//                 Debug.LogWarning($"Entry not found for address: {address}");
//                 return null;
//             }
//             catch (System.Exception e)
//             {
//                 Debug.LogError($"Error finding entry: {e.Message}");
//                 return null;
//             }
//         }
//
//         public static string FormatBytes(long bytes)
//         {
//             if (bytes == 0) return "0 B";
//         
//             string[] suffixes = { "B", "KB", "MB", "GB" };
//             int counter = 0;
//             float number = bytes;
//         
//             while (number >= 1024 && counter < suffixes.Length - 1)
//             {
//                 number /= 1024f;
//                 counter++;
//             }
//         
//             return $"{number:F2} {suffixes[counter]}";
//         }
//     }
// }
// #endif