
namespace CZToolKit
{
    public interface IResourceManager
    {
        UnityEngine.Object Load<T>(string path) where T : UnityEngine.Object;
        
        void Unload(UnityEngine.Object asset);
    }
}