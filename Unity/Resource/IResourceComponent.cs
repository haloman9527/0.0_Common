using UnityObject = UnityEngine.Object;

namespace Atom
{
    public interface IResourceComponent
    {
        AssetHandle LoadAsset<TObject>(string location) where TObject : UnityEngine.Object;

        public AssetHandle LoadAssetAsync<TObject>(string location) where TObject : UnityObject;

        public SceneHandle LoadScene(string location);

        public SceneHandle LoadSceneAsync(string location);

        public void UnloadUnusedAssets();
    }
}