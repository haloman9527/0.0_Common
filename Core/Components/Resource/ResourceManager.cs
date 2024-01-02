using CZToolKit.Singletons;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace CZToolKit
{
    public class ResourceManager : Singleton<ResourceManager>, IResourceManager
    {
        public T Load<T>(string path) where T : UnityObject
        {
            return Resources.Load<T>(path);
        }

        public void Unload(UnityObject asset)
        {
            Resources.UnloadAsset(asset);
        }
    }
}