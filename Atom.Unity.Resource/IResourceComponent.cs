using UnityObject = UnityEngine.Object;

namespace Atom
{
    public interface IResourceLoader
    {
        AssetHandleBase LoadAsset<T>(string location) where T : UnityObject;

        AssetHandleBase LoadAssetAsync<T>(string location) where T : UnityObject;
        
        AssetsHandleBase LoadAssets<T>(string location) where T : UnityObject;
        
        AssetsHandleBase LoadAssetsAsync<T>(string location) where T : UnityObject;
        
        SceneHandleBase LoadScene(string location);

        SceneHandleBase LoadSceneAsync(string location);

        void UnloadUnusedAssets();
    }
}