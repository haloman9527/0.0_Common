
using System;
using System.Threading.Tasks;

namespace CZToolKit
{
    public class GameModule : Module
    {
        private ILocalAssetLoader assetLoader;

        public ILocalAssetLoader AssetLoader
        {
            get { return assetLoader; }
        }
        
        protected override void OnOpen()
        {
            base.OnOpen();
            assetLoader = ObjectPool.Instance.Acquire<LocalAssetLoader>();
        }

        private async void B()
        {
            var task = new Task(() => { });
            await task;
        }

        protected override void OnClose()
        {
            ObjectPool.Instance.Release(assetLoader);
            base.OnClose();
        }
    }
}
