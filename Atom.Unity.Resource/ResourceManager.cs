using System.Buffers.Text;
using UnityObject = UnityEngine.Object;

namespace Atom
{
    public class ResourceManager : SingletonBase<ResourceManager>, ISingleton, IResourceLoader
    {
        private IResourceLoader m_Loader;

        public void Init(IResourceLoader loader)
        {
            this.m_Loader = loader;
        }

        public AssetHandleBase LoadAsset(string location)
        {
            return LoadAsset<UnityObject>(location);
        }

        public AssetHandleBase LoadAssetAsync(string location)
        {
            return LoadAssetAsync<UnityObject>(location);
        }

        public AssetHandleBase LoadAsset<T>(string location) where T : UnityObject
        {
            return m_Loader.LoadAsset<T>(location);
        }

        public AssetHandleBase LoadAssetAsync<T>(string location) where T : UnityObject
        {
            return m_Loader.LoadAssetAsync<T>(location);
        }

        public AssetsHandleBase LoadAssets(string location)
        {
            return LoadAssets<UnityObject>(location);
        }

        public AssetsHandleBase LoadAssetsAsync(string location)
        {
            return LoadAssetsAsync<UnityObject>(location);
        }

        public AssetsHandleBase LoadAssets<T>(string location) where T : UnityObject
        {
            return m_Loader.LoadAssets<T>(location);
        }

        public AssetsHandleBase LoadAssetsAsync<T>(string location) where T : UnityObject
        {
            return m_Loader.LoadAssetsAsync<T>(location);
        }

        public SceneHandleBase LoadScene(string location)
        {
            return m_Loader.LoadScene(location);
        }

        public SceneHandleBase LoadSceneAsync(string location)
        {
            return m_Loader.LoadSceneAsync(location);
        }

        public void UnloadUnusedAssets()
        {
            m_Loader.UnloadUnusedAssets();
        }
    }
}