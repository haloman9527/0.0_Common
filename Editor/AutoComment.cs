using System.IO;
using UnityEditor;

namespace CZToolKit.Core.Editors
{
    public class AutoComment : UnityEditor.AssetModificationProcessor
    {
        static string comment = @"#region 注 释
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
";

        static void OnWillCreateAsset(string _newFile)
        {
            EditorApplication.delayCall += () => { AddComment(_newFile); };
        }

        static void AddComment(string _fileName)
        {
            _fileName = _fileName.Replace(".meta", "");
            if (!_fileName.EndsWith(".cs")) return;
            string raw = File.ReadAllText(_fileName);
            if (raw.StartsWith("#region 注 释")) return;
            File.WriteAllText(_fileName, comment + raw);
            AssetDatabase.Refresh();
        }
    }
}
