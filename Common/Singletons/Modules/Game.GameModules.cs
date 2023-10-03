namespace CZToolKit.Singletons
{
    public sealed class RootModule : GameModule
    {
        
    }

    public static partial class Game
    {
        public static RootModule RootModule { get; private set; }

        private static void InitRootModule()
        {
            RootModule = new RootModule();
            RootModule.Open(null);
        }

        private static void UpdateModules()
        {
            RootModule.Update();
        }

        private static void LateUpdateModules()
        {
            RootModule.LateUpdate();
        }

        private static void CloseSubModules()
        {
            RootModule.CloseSubModules();
        }
    }
}