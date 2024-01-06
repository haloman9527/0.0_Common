namespace CZToolKit.Singletons
{
    public static partial class Game
    {
        static Game()
        {
        }

        public static void FixedUpdate()
        {
            FixedUpdateSingletons();
        }

        public static void Update()
        {
            UpdateSingletons();
        }

        public static void LateUpdate()
        {
            LateUpdateSingletons();
        }

        public static void Close()
        {
            CloseSingletons();
        }
    }
}