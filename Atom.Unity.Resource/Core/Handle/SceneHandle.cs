using System;

namespace Atom
{
    public abstract class SceneHandleBase : HandleBase, IDisposable
    {
        public abstract string SceneName { get; }

        public abstract UnityEngine.SceneManagement.Scene SceneObject { get; }

        public abstract event Action<SceneHandleBase> OnCompleted;

        public abstract void UnloadAsync();

        public void Dispose() => this.UnloadAsync();
    }
}