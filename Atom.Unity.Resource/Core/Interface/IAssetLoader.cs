using UnityObject = UnityEngine.Object;

namespace Atom
{
    public interface IAssetLoader
    {
        AssetHandleBase LoadAsset<T>(string location) where T : UnityObject;
        
        AssetHandleBase LoadAssetAsync<T>(string location) where T : UnityObject;
        
        AssetsHandleBase LoadAssets<T>(string location) where T : UnityObject;
        
        AssetsHandleBase LoadAssetsAsync<T>(string location) where T : UnityObject;
        
        SceneHandleBase LoadScene(string location);
        
        SceneHandleBase LoadSceneAsync(string location);
        
        RawFileHandleBase LoadRawFile(string location);
        
        RawFileHandleBase LoadRawFileAsync(string location);

        void UnloadUnusedAssets();
    }
}