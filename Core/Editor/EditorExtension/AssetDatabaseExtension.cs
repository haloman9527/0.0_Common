#region ◊¢  Õ
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: ∞Î÷ª¡˙œ∫»À
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    public static class AssetDatabaseExtension
    {
        public static T FirstAsset<T>(string filter) where T : UnityObject
        {
            string[] guids = AssetDatabase.FindAssets(filter);
            foreach (var guid in guids)
            {
                T t = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                if (t != null)
                    return t;
            }
            return null;
        }

        public static T FirstAsset<T>(string filter, string[] searchFolders) where T : UnityObject
        {
            string[] guids = AssetDatabase.FindAssets(filter, searchFolders);
            foreach (var guid in guids)
            {
                T t = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                if (t != null)
                    return t;
            }
            return null;
        }

        public static UnityObject FirstAsset(Type type, string filter)
        {
            string[] guids = AssetDatabase.FindAssets(filter);
            foreach (var guid in guids)
            {
                UnityObject t = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), type);
                if (t != null)
                    return t;
            }
            return null;
        }

        public static UnityObject FirstAsset(Type type, string filter, string[] searchFolders)
        {
            string[] guids = AssetDatabase.FindAssets(filter, searchFolders);
            foreach (var guid in guids)
            {
                UnityObject t = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), type);
                if (t != null)
                    return t;
            }
            return null;
        }
    }
}
#endif