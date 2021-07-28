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
        static Dictionary<Type, MonoScript> MonoScriptCache = new Dictionary<Type, MonoScript>();

        public static MonoScript FindScriptFromType(Type _type, Func<MonoScript, bool> _pattern = null, bool _compareTypeName = true)
        {
            if (MonoScriptCache.TryGetValue(_type, out MonoScript monoScript))
                return monoScript;

            string findStr = "t:script " + (_compareTypeName ? _type.Name : "");
            var scriptGUIDs = AssetDatabase.FindAssets(findStr);
            foreach (var scriptGUID in scriptGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

                if (script != null && String.Equals(_type.Name, Path.GetFileNameWithoutExtension(assetPath), StringComparison.OrdinalIgnoreCase) && script.GetClass() == _type)
                {
                    if (_pattern == null || _pattern(script))
                        MonoScriptCache[_type] = monoScript = script;
                }
            }

            return monoScript;
        }
    }
}
