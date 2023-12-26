using CZToolKit.ET;

namespace CZToolKit.Singletons
{
    public static partial class Game
    {
        private static void UpdateEntites()
        {
            Systems.FixedUpdate(Root.Instance.EntitiesQueue);
        }

        private static void LateUpdateEntites()
        {
            Systems.LateUpdate(Root.Instance.EntitiesQueue);
        }

        private static void CloseEntities()
        {
            Root.Instance.Dispose();
        }
    }
}