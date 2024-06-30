using UnityObject = UnityEngine.Object;

namespace CZToolKit
{
    public class ResourceManager : Singleton<ResourceManager>, IResourceComponent
    {
        private IResourceComponent o;

        public void Install(IResourceComponent o)
        {
            this.o = o;
        }

        public AssetHandle LoadAsset<T>(string location) where T : UnityObject
        {
            return o.LoadAsset<T>(location);
        }

        public AssetHandle LoadAssetAsync<T>(string location) where T : UnityObject
        {
            return o.LoadAssetAsync<T>(location);
        }

        public SceneHandle LoadScene(string location)
        {
            return o.LoadScene(location);
        }

        public SceneHandle LoadSceneAsync(string location)
        {
            return o.LoadSceneAsync(location);
        }

        public void UnloadUnusedAssets()
        {
            o.UnloadUnusedAssets();
        }

        public T As<T>() where T : class, IResourceComponent
        {
            return o as T;
        }
    }
}