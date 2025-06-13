using System;

namespace Atom
{
    [Serializable]
    public abstract class SingletonBase<T> :
        ISingleton,
        ISingletonAwake,
        ISingletonDestroy,
        ISingletonFixedUpdate,
        ISingletonUpdate,
        ISingletonLateUpdate
        where T : SingletonBase<T>
    {
        private static T s_Instance;

        public static T Instance
        {
            get { return s_Instance; }
        }

        public void Awake()
        {
            if (s_Instance != null)
                throw new Exception($"singleton register twice! {TypeCache<T>.TYPE.Name}");

            s_Instance = (T)this;
            OnAwake();
        }

        public void Destroy()
        {
            s_Instance = null;
            OnDestroy();
        }

        public void FixedUpdate()
        {
            OnFixedUpdate();
        }

        public void Update()
        {
            OnUpdate();
        }

        public void LateUpdate()
        {
            OnLateUpdate();
        }

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnFixedUpdate()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnLateUpdate()
        {
        }
    }
}