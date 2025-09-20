using System;
using System.Collections.Generic;

namespace Atom
{
    public static class GameModuleEntry
    {
        private static readonly List<IGameModule> s_GameModules = new List<IGameModule>();
        private static readonly Dictionary<string, IGameModule> s_GameModulesByName = new Dictionary<string, IGameModule>();

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
            if (s_GameModulesByName.ContainsKey(name))
            {
                throw new Exception($"GameModule {name} already exists");
            }

            s_GameModules.Add(module);
            s_GameModulesByName.Add(name, module);
            module.Init();
        }

        public static void Shutdown()
        {
            var count = s_GameModules.Count;
            for (var i = 0; i < count; i++)
            {
                s_GameModules[i].UnInit();
            }

            s_GameModules.Clear();
            s_GameModulesByName.Clear();
        }

        public static void FixedUpdate()
        {
            var count = s_GameModules.Count;
            for (var i = 0; i < count; i++)
            {
                s_GameModules[i].FixedUpdate();
            }
        }

        public static void Update()
        {
            var count = s_GameModules.Count;
            for (var i = 0; i < count; i++)
            {
                s_GameModules[i].Update();
            }
        }

        public static void LateUpdate()
        {
            var count = s_GameModules.Count;
            for (var i = 0; i < count; i++)
            {
                s_GameModules[i].LateUpdate();
            }
        }
    }
}