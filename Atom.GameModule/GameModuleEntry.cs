using System;
using System.Collections.Generic;

namespace Atom
{
    public static class GameModuleEntry
    {
        private static readonly List<GameModule> s_GameModules = new List<GameModule>();
        private static readonly Dictionary<string, GameModule>  s_GameModulesByName = new Dictionary<string, GameModule>();

        public static GameModule GetGameModule(string name)
        {
            s_GameModulesByName.TryGetValue(name, out var module);
            return module;
        }
        
        public static void RegisterGameModule<T>(string name, T module)  where T : GameModule
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
                s_GameModules[i].Shutdown();
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