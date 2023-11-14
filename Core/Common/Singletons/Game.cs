
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
            UpdateSingletons();
            UpdateModules();
        }

        public static void LateUpdate()
        {
            LateUpdateSingletons();
            LateUpdateModules();
        }

        public static void Close()
        {
            CloseSubModules();
            CloseSingletons();
        }
    }   
}