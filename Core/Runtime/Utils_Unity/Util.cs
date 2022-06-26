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
using UnityEngine;

namespace CZToolKit.Core
{
    public static partial class Util
    {
#if UNITY_EDITOR
        public static string ConvertToRelativePath(string absolutePath)
        {
            var path = absolutePath.Replace('\\', '/');
            return path.Substring(path.LastIndexOf("/Assets/") + 1); ;
        }
#endif

        public static string TextureToBase64(Texture2D texture)
        {
            byte[] bytes = texture.EncodeToJPG();
            string baser64 = Convert.ToBase64String(bytes);
            return baser64;
        }

        public static Texture2D Base64ToTexture(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            Texture2D texture = new Texture2D(100, 100);
            texture.LoadImage(bytes);
            return texture;
        }
    }
}
