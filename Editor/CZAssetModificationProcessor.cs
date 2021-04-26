using System;
using UnityEditor;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    public class CZAssetModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        ///// <summary> 改名Bug补救方案 </summary>
        //static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        //{
        //    Object obj = AssetDatabase.LoadMainAssetAtPath(sourcePath);
        //    if (obj == null)
        //        return AssetMoveResult.DidNotMove;

        //    var srcDir = Path.GetDirectoryName(sourcePath);
        //    string dstDir = Path.GetDirectoryName(destinationPath);
        //    if (srcDir != dstDir)
        //        return AssetMoveResult.DidNotMove;

        //    string fileName = Path.GetFileNameWithoutExtension(destinationPath);
        //    obj.name = fileName;

        //    return AssetMoveResult.DidNotMove;
        //}

        public static Action<string> onWillCreateAsset;
        static void OnWillCreateAsset(string _newFile)
        {
            onWillCreateAsset?.Invoke(_newFile);
        }
    }
}
