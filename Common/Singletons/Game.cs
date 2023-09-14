
namespace CZToolKit.Singletons
{
    public static partial class Game
    {
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
            CloseSingletons();
            CloseModules();
        }
    }
}