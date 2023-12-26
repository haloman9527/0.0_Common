namespace CZToolKit.Singletons
{
    public static partial class Game
    {
        static Game()
        {
        }

        public static void Update()
        {
            UpdateSingletons();
            UpdateEntites();
        }

        public static void LateUpdate()
        {
            LateUpdateSingletons();
            LateUpdateEntites();
        }

        public static void Close()
        {
            CloseEntities();
            CloseSingletons();
        }
    }
}