namespace CZToolKit.Singletons
{
    public sealed class RootModule : GameModule
    {
        
    }
    
    public static partial class Game
    {
        public static RootModule RootModule { get; private set; } = GameModule.NewModule<RootModule>();

        private static void UpdateModules()
        {
            RootModule.Update();
        }

        private static void LateUpdateModules()
        {
            RootModule.LateUpdate();
        }

        private static void CloseModules()
        {
            var modules = RootModule.GetSubModules();
            foreach (var module in modules)
            {
                module.Close();
            }
        }
    }
}