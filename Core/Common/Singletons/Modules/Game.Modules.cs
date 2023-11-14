namespace CZToolKit.Singletons
{
    public static partial class Game
    {
        public static Module RootModule { get; private set; }

        private static void InitRootModule()
        {
            RootModule = new Module();
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