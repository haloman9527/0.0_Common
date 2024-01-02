
namespace CZToolKit
{
    public interface IResourceManager
    {
        T Load<T>(string path) where T : UnityEngine.Object;
        
        void Unload(UnityEngine.Object asset);
    }
}