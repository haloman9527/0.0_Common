using System;

namespace Jiange
{
    public abstract class SceneHandle : HandleBase
    {
        public abstract string SceneName { get; }

        public abstract UnityEngine.SceneManagement.Scene SceneObject { get; }

        public abstract event Action<SceneHandle> OnCompleted;

        public abstract void UnloadAsync();
    }
}