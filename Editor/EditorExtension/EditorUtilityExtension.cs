#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 
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

        public static MonoScript FindScriptFromType(Type _type)
        {
            if (MonoScriptCache.TryGetValue(_type, out MonoScript monoScript))
                return monoScript;


            var scriptGUIDs = AssetDatabase.FindAssets($"t:script {_type.Name}");
            foreach (var scriptGUID in scriptGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

                if (script != null && String.Equals(_type.Name, Path.GetFileNameWithoutExtension(assetPath), StringComparison.OrdinalIgnoreCase) && script.GetClass() == _type)
                    MonoScriptCache[_type] = monoScript = script;
            }

            return monoScript;
        }
    }
}
