using System;
using UnityObject = UnityEngine.Object;

namespace CZToolKit
{
    public class LocalAssetLoader : ILocalAssetLoader
    {
        public T LoadAsset<T>(string path)
        {
            throw new NotImplementedException();
        }

        public UnityObject LoadAsset(string path, Action<object> callback)
        {
            throw new NotImplementedException();
        }

        public byte[] LoadAsset(string path)
        {
            throw new NotImplementedException();
        }

        public void ReleaseAsset(string path)
        {
            throw new NotImplementedException();
        }

        public void ReleaseAllAsset()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}