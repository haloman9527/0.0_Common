
namespace CZToolKit.Singletons
{
    public static partial class Game
    {
        static Game()
        {
            InitRootModule();
        }
        
        public static void Update()
        {
            UpdateModules();
            UpdateSingletons();
        }

        public static void LateUpdate()
        {
            LateUpdateModules();
            LateUpdateSingletons();
        }

        public static void Close()
        {
            CloseSingletons();
            CloseSubModules();
        }
    }   
}