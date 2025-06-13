using System;
using UnityEngine.SceneManagement;

namespace Atom
{
    public abstract class SceneHandleBase : HandleBase
    {
        public abstract string SceneName { get; }

        public abstract Scene SceneObject { get; }

        public abstract event Action<SceneHandleBase> OnCompleted;

        public abstract void UnloadAsync();
    }
}