using System;
using UnityObject = UnityEngine.Object;

namespace CZToolKit
{
    public interface ILocalAssetLoader : IReference
    {
        T LoadAsset<T>(string path);

        UnityObject LoadAsset(string path, Action<object> callback);

        byte[] LoadAsset(string path);

        void ReleaseAsset(string path);

        void ReleaseAllAsset();
    }
}