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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */
#endregion
using System;
using UnityEngine;

namespace CZToolKit.Unity
{
    public static class Util_Unity
    {
#if UNITY_EDITOR
        /// <summary>
        /// 绝对路径转项目内相对路径
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <returns></returns>
        public static string ConvertToAssetsPath(string absolutePath)
        {
            return absolutePath.Substring(Application.dataPath.Length - 6);
        }
#endif

        public static string TextureToBase64(Texture2D texture)
        {
            var bytes = texture.EncodeToJPG();
            var baser64 = Convert.ToBase64String(bytes);
            return baser64;
        }

        public static Texture2D Base64ToTexture(string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            var texture = new Texture2D(100, 100);
            texture.LoadImage(bytes);
            return texture;
        }
    }
}
