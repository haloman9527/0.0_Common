using System;
using System.Collections.Generic;

namespace Atom
{
    public static class SingletonEntry
    {
        private static readonly Dictionary<Type, ISingleton> s_SingletonTypes = new Dictionary<Type, ISingleton>();
        private static readonly Queue<ISingleton> s_Singletons = new Queue<ISingleton>();

        public static IReadOnlyDictionary<Type, ISingleton> SingleTypes
        {
            get { return s_SingletonTypes; }
        }

        private static ISingleton GetSingleton_Internal(Type singletonType)
        {
            if (s_SingletonTypes.TryGetValue(singletonType, out var singleton))
                return singleton;
            
            foreach (var pair in s_SingletonTypes)
            {
                if (singletonType.IsAssignableFrom(pair.Value.GetType()))
                    return pair.Value;
            }

            return null;
        }

        private static void RegisterSingleton_Internal(ISingleton singleton, Type singletonType)
        {
            s_SingletonTypes.Add(singletonType, singleton);
            s_Singletons.Enqueue(singleton);
            if (singleton is ISingletonAwake awake)
                awake.Awake();
        }

        public static ISingleton GetSingleton(Type singletonType)
        {
            return GetSingleton_Internal(singletonType);
        }

        public static T GetSingleton<T>()
        {
            return (T)GetSingleton_Internal(TypeCache<T>.TYPE);
        }

        public static T RegisterSingleton<T>() where T : SingletonBase<T>, new()
        {
            var singleton = new T();
            RegisterSingleton_Internal(singleton, TypeCache<T>.TYPE);
            return singleton;
        }

        public static void RegisterSingleton(ISingleton singleton)
        {
            var singletonType = singleton.GetType();
            RegisterSingleton_Internal(singleton, singletonType);
        }

        public static void FixedUpdate()
        {
            int count = s_Singletons.Count;
            while (count-- > 0)
            {
                var singleton = s_Singletons.Dequeue();
                if (!(singleton is ISingletonFixedUpdate fixedUpdate))
                    continue;

                s_Singletons.Enqueue(singleton);
                fixedUpdate.FixedUpdate();
            }
        }

        public static void Update()
        {
            int count = s_Singletons.Count;
            while (count-- > 0)
            {
                var singleton = s_Singletons.Dequeue();
                if (!(singleton is ISingletonUpdate update))
                    continue;

                s_Singletons.Enqueue(singleton);
                update.Update();
            }
        }

        public static void LateUpdate()
        {
            int count = s_Singletons.Count;
            while (count-- > 0)
            {
                var singleton = s_Singletons.Dequeue();
                if (!(singleton is ISingletonLateUpdate lateUpdate))
                    continue;

                s_Singletons.Enqueue(singleton);
                lateUpdate.LateUpdate();
            }
        }

        public static void Shutdown()
        {
            // 顺序反过来清理
            var singletonStack = new Stack<ISingleton>();
            while (s_Singletons.Count > 0)
            {
                singletonStack.Push(s_Singletons.Dequeue());
            }
            
            while (singletonStack.Count > 0)
            {
                var singleton = singletonStack.Pop();
                if (singleton is ISingletonDestroy destory)
                    destory.Destroy();
            }

            s_SingletonTypes.Clear();
        }
    }
}