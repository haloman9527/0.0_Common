namespace CZToolKit.ET
{
    public class TimeComponent : Entity
    {
        public const int logicDeltaTime = 2;
        
        public float logicTimeScale = 1;
        public int frameIndex;
        
        public float renderTimeScale;
    }

    public static class TimeComponentSystems
    {
        public class AwakeSystem : AwakeSystem<TimeComponent>
        {
            protected override void Awake(TimeComponent o)
            {
            }
        }
    }
}