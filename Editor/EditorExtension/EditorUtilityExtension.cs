#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace CZToolKit.Core.Editors
{
    public static class EditorUtilityExtension
    {
        public static MonoScript FindScriptFromType(Type _type, Func<MonoScript, bool> _pattern = null, bool _compareTypeName = true)
        {
            string findStr = "t:script " + (_compareTypeName ? _type.Name : "");
            var scriptGUIDs = AssetDatabase.FindAssets(findStr);
            foreach (var scriptGUID in scriptGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

                if (script != null)
                {
                    if (_pattern == null || _pattern(script))
                        return script;
                }
            }
            return null;
        }
        public static IEnumerable<MonoScript> FindAllScriptFromType(Type _type, Func<MonoScript, bool> _pattern = null, bool _compareTypeName = true)
        {
            string findStr = "t:script " + (_compareTypeName ? _type.Name : "");
            var scriptGUIDs = AssetDatabase.FindAssets(findStr);
            foreach (var scriptGUID in scriptGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
                if (script != null)
                {
                    if (_pattern == null || _pattern(script))
                        yield return script;
                }
            }
        }
    }
}
