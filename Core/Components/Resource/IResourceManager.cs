
namespace CZToolKit
{
    public interface IResourceManager
    {
        T LoadSync<T>(string path) where T : UnityEngine.Object;
        
        void Unload(UnityEngine.Object asset);
    }
}