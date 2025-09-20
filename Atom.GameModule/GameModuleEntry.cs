using System;
using System.Collections.Generic;

namespace Atom
{
    public static class GameModuleEntry
    {
        private static readonly List<IGameModule> s_GameModules = new List<IGameModule>();
        private static readonly Dictionary<string, IGameModule> s_GameModulesByName = new Dictionary<string, IGameModule>();
        private static readonly Dictionary<IGameModule, string> s_GameModuleNames = new Dictionary<IGameModule, string>();

        public static IGameModule GetGameModule(string name)
        {
            s_GameModulesByName.TryGetValue(name, out var module);
            return module;
        }

        public static void GetAllGameModules(List<IGameModule> modules)
        {
            modules.Clear();
            modules.AddRange(s_GameModules);
        }

        public static void RegisterGameModule<T>(string name, T module) where T : class, IGameModule
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (module == null)
                throw new ArgumentNullException(nameof(module));

            if (s_GameModulesByName.ContainsKey(name))
                throw new Exception($"GameModule {name} is already registered");

            if (s_GameModuleNames.ContainsKey(module))
                throw new Exception($"GameModule {module} is already registered");

            s_GameModules.Add(module);
            s_GameModulesByName.Add(name, module);
            s_GameModuleNames.Add(module, name);
            module.Init();
        }

        public static void UnRegisterGameModule(IGameModule module)
        {
            if (!s_GameModuleNames.TryGetValue(module, out var name))
                return;

            try
            {
                module.UnInit();
            }
            finally
            {
                s_GameModules.Remove(module);
                s_GameModuleNames.Remove(module);
                s_GameModulesByName.Remove(name);
            }
        }

        public static void Shutdown()
        {
            var count = s_GameModules.Count;
            for (var i = 0; i < count; i++)
            {
                try
                {
                    s_GameModules[i].UnInit();
                }
                catch (Exception e)
                {
#if UNITY_EDITOR
                    UnityEngine.Debug.LogException(e);
#endif
                }
            }

            s_GameModules.Clear();
            s_GameModulesByName.Clear();
        }

        public static void FixedUpdate()
        {
            var count = s_GameModules.Count;
            for (var i = 0; i < count; i++)
            {
                try
                {
                    s_GameModules[i].FixedUpdate();
                }
                catch (Exception e)
                {
#if UNITY_EDITOR
                    UnityEngine.Debug.LogException(e);
#endif
                }
            }
        }

        public static void Update()
        {
            var count = s_GameModules.Count;
            for (var i = 0; i < count; i++)
            {
                try
                {
                    s_GameModules[i].Update();
                }
                catch (Exception e)
                {
#if UNITY_EDITOR
                    UnityEngine.Debug.LogException(e);
#endif
                }
            }
        }

        public static void LateUpdate()
        {
            var count = s_GameModules.Count;
            for (var i = 0; i < count; i++)
            {
                try
                {
                    s_GameModules[i].LateUpdate();
                }
                catch (Exception e)
                {
#if UNITY_EDITOR
                    UnityEngine.Debug.LogException(e);
#endif
                }
            }
        }
    }
}