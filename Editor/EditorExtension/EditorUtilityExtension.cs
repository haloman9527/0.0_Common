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
using System.IO;
using UnityEditor;

namespace CZToolKit.Core.Editors
{
    public static class EditorUtilityExtension
    {
        public static MonoScript FindScriptFromType(Type _type)
        {
            var scriptGUIDs = AssetDatabase.FindAssets($"t:script {_type.Name}");

            if (scriptGUIDs.Length == 0)
                return null;

            foreach (var scriptGUID in scriptGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

                if (script != null && String.Equals(_type.Name, Path.GetFileNameWithoutExtension(assetPath), StringComparison.OrdinalIgnoreCase) && script.GetClass() == _type)
                    return script;
            }

            return null;
        }
    }
}
