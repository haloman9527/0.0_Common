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
using System.Collections.Generic;
using UnityEngine;

namespace Moyo.Unity
{
    public static class Util
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

        public static string TextureToBase64(this Texture2D texture)
        {
            var bytes = texture.EncodeToJPG();
            var baser64 = Convert.ToBase64String(bytes);
            return baser64;
        }

        public static Texture2D Base64ToTexture(string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            // 长宽任意，LoadImage会覆盖
            var texture = new Texture2D(100, 100);
            texture.LoadImage(bytes);
            return texture;
        }

        public static string GetTransformPath(this Transform transform, Transform root = null)
        {
            var path = transform.name;
            while (true)
            {
                transform = transform.parent;
                if (transform == root)
                {
                    break;
                }

                path = $"{transform.name}/{path}";
            }

            return path;
        }
        
        public static void FindComponents<T>(this UnityEngine.SceneManagement.Scene scene, List<T> components, bool includeInactive = false) where T : Component
        {
            var buffer = new List<T>();
            var rootGameObjects = scene.GetRootGameObjects();
            foreach (var rootGameObject in rootGameObjects)
            {
                rootGameObject.transform.GetComponentsInChildren(includeInactive, buffer);
                components.AddRange(buffer);
            }
        }

        public static List<T> FindComponents<T>(this UnityEngine.SceneManagement.Scene scene, bool includeInactive = false) where T : Component
        {
            var components = new List<T>();
            var buffer = new List<T>();
            var rootGameObjects = scene.GetRootGameObjects();
            foreach (var rootGameObject in rootGameObjects)
            {
                rootGameObject.transform.GetComponentsInChildren(includeInactive, buffer);
                components.AddRange(buffer);
            }

            return components;
        }

        public static T FindComponent<T>(this UnityEngine.SceneManagement.Scene scene, bool includeInactive = false) where T : Component
        {
            var rootGameObjects = scene.GetRootGameObjects();
            foreach (var rootGameObject in rootGameObjects)
            {
                var component = rootGameObject.transform.GetComponentInChildren<T>();
                if (component != null)
                    return component;
            }

            return null;
        }
    }
}
