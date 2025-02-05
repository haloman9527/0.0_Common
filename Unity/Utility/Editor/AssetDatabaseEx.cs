using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Moyo.UnityEditors
{
    public static class AssetDatabaseEx
    {
        public static string[] FindAssetsFromSelection<T>(string filter) where T : UnityEngine.Object
        {
            var guids = new HashSet<string>();
            var searchInFolders = new string[1];
            foreach (var s in Selection.objects)
            {
                if (s is T)
                {
                    guids.Add(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(s)));
                }
                else if (s is DefaultAsset)
                {
                    var path = AssetDatabase.GetAssetPath(s);
                    if (AssetDatabase.IsValidFolder(path))
                    {
                        searchInFolders[0] = path;
                        foreach (var guid in AssetDatabase.FindAssets(filter, searchInFolders))
                        {
                            guids.Add(AssetDatabase.GUIDToAssetPath(guid));
                        }
                    }
                }
            }

            return guids.ToArray();
        }
    }
}