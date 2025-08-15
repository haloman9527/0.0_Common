using System;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace Atom
{
    public sealed class AssetStation : IAssetLoader
    {
        private IAssetLoader m_Loader;
        private List<HandleBase> m_AssetHandles;

        public AssetStation(IAssetLoader loader)
        {
            m_Loader = loader;
            m_AssetHandles =  new List<HandleBase>();
        }

        public void Release(HandleBase handle)
        {
            if (m_AssetHandles.Remove(handle))
            {
                if (handle is IDisposable disposable)
                    disposable.Dispose();
            }
        }

        public void ReleaseAll()
        {
            for (int i = 0; i < m_AssetHandles.Count; i++)
            {
                var handle = m_AssetHandles[i];
                if (handle.IsValid && handle is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            m_AssetHandles.Clear();
        }

        public AssetHandleBase LoadAsset<T>(string location) where T : UnityObject
        {
            var handle = m_Loader.LoadAsset<T>(location);
            m_AssetHandles.Add(handle);
            return handle;
        }

        public AssetHandleBase LoadAssetAsync<T>(string location) where T : UnityObject
        {
            var handle = m_Loader.LoadAssetAsync<T>(location);
            m_AssetHandles.Add(handle);
            return handle;
        }

        public AssetsHandleBase LoadAssets<T>(string location) where T : UnityObject
        {
            var handle = m_Loader.LoadAssets<T>(location);
            m_AssetHandles.Add(handle);
            return handle;
        }

        public AssetsHandleBase LoadAssetsAsync<T>(string location) where T : UnityObject
        {
            var handle = m_Loader.LoadAssetsAsync<T>(location);
            m_AssetHandles.Add(handle);
            return handle;
        }

        public SceneHandleBase LoadScene(string location)
        {
            var handle = m_Loader.LoadScene(location);
            m_AssetHandles.Add(handle);
            return handle;
        }

        public SceneHandleBase LoadSceneAsync(string location)
        {
            var handle = m_Loader.LoadSceneAsync(location);
            m_AssetHandles.Add(handle);
            return handle;
        }

        public RawFileHandleBase LoadRawFile(string location)
        {
            var handle = m_Loader.LoadRawFile(location);
            m_AssetHandles.Add(handle);
            return handle;
        }

        public RawFileHandleBase LoadRawFileAsync(string location)
        {
            var handle = m_Loader.LoadRawFileAsync(location);
            m_AssetHandles.Add(handle);
            return handle;
        }
    }
}