using System;
using System.Collections.Generic;
using System.Linq;

namespace CZToolKit
{
    public interface IGameModule
    {
        bool Active { get; }

        bool ActiveSelf { get; set; }

        GameModule Parent { get; set; }

        GameModule[] GetSubModules(bool recursive = false);
        
        void GetSubModules(List<GameModule> result, bool recursive = false);
        
        GameModule GetSubModule(Type moduleType, bool recursive = false);
        
        T GetSubModule<T>(bool recursive = false) where T : GameModule;
        
        GameModule OpenSubModule(Type moduleType, object args = null);
        
        T OpenSubModule<T>(object args = null) where T : GameModule;
        
        void CloseSubmodules();

        void Close();
    }

    public abstract class GameModule : IGameModule
    {
        private static GameModule NewModule(GameModule parent, Type moduleType, object args = null)
        {
            var module = (GameModule)null;
            if (args == null)
                module = (GameModule)Activator.CreateInstance(moduleType);
            else
                module = (GameModule)Activator.CreateInstance(moduleType, args);
            module.SetParent_Internal(parent);
            module.Enter();
            module.SetActiveSelf_Internal(true);
            return module;
        }

        public static T NewModule<T>(object args = null) where T : GameModule
        {
            return (T)NewModule(null, typeof(T), args);
        }

        private bool active;
        private bool activeSelf;
        private GameModule parent;
        private List<GameModule> subModules = new List<GameModule>();
        private HashSet<GameModule> subModuleUpdateCache = new HashSet<GameModule>();

        public bool Active
        {
            get { return active; }
        }

        public bool ActiveSelf
        {
            get { return active; }
            set { SetActiveSelf_Internal(value); }
        }

        public GameModule Parent
        {
            get { return parent; }
            set { SetParent_Internal(value); }
        }

        protected List<GameModule> SubModules
        {
            get { return subModules; }
        }

        private void SetActiveSelf_Internal(bool active)
        {
            if (this.activeSelf == active)
                return;
            this.activeSelf = active;
            RefreshActive_Internal();
        }

        private void RefreshActive_Internal()
        {
            var _active = parent == null ? activeSelf : parent.active && activeSelf;
            if (active == _active)
                return;
            if (_active)
                Enable();
            else
                Disable();
        }

        private void SetParent_Internal(GameModule parent)
        {
            if (this.Parent == parent)
                return;

            this.parent = parent;

            if (this.parent != null)
                this.parent.subModules.Add(this);

            this.ParentChanged();
            this.RefreshActive_Internal();
        }

        private IEnumerable<GameModule> GetSubModules_Internal(bool recursive = false)
        {
            foreach (var module in subModules)
            {
                if (module.parent != this)
                    continue;
                yield return module;
                if (!recursive)
                    continue;
                foreach (var _m in module.GetSubModules_Internal(true))
                {
                    yield return _m;
                }
            }
        }

        private GameModule GetSubModule_Internal(Type moduleType, bool recursive = false)
        {
            foreach (var module in GetSubModules_Internal(recursive))
            {
                if (moduleType.IsAssignableFrom(module.GetType()))
                    return module;
            }

            return null;
        }

        private GameModule OpenSubModule_Internal(Type moduleType, object args = null)
        {
            return NewModule(this, moduleType, args);
        }

        /// <summary>
        /// 获取所有子模块.
        /// </summary>
        /// <param name="recursive"> 递归获取所有子模块 </param>
        /// <returns></returns>
        public GameModule[] GetSubModules(bool recursive = false)
        {
            return GetSubModules_Internal(recursive).ToArray();
        }

        /// <summary>
        /// 获取所有子模块.
        /// </summary>
        /// <param name="result"> 结果 </param>
        /// <param name="recursive"> 递归获取所有子模块 </param>
        /// <returns></returns>
        public void GetSubModules(List<GameModule> result, bool recursive = false)
        {
            result.Clear();
            result.AddRange(GetSubModules_Internal(recursive));
        }

        /// <summary>
        /// 获取指定类型的子模块.
        /// </summary>
        /// <param name="moduleType"> 指定的子模块类型 </param>
        /// <param name="recursive"> 递归查找类型符合的子模块 </param>
        /// <returns></returns>
        public GameModule GetSubModule(Type moduleType, bool recursive = false)
        {
            return GetSubModule_Internal(moduleType);
        }

        /// <summary>
        /// 获取指定类型的子模块.
        /// </summary>
        /// <param name="recursive">递归查找类型符合的子模块 </param>
        /// <typeparam name="T"> 指定的子模块类型 </typeparam>
        /// <returns></returns>
        public T GetSubModule<T>(bool recursive = false) where T : GameModule
        {
            return (T)GetSubModule(typeof(T), recursive);
        }

        /// <summary>
        /// 打开一个子模块.
        /// </summary>
        /// <param name="moduleType"> 子模块类型 </param>
        /// <param name="args"> 参数 </param>
        /// <returns></returns>
        public GameModule OpenSubModule(Type moduleType, object args = null)
        {
            return OpenSubModule_Internal(moduleType, args);
        }

        /// <summary>
        /// 打开一个子模块.
        /// </summary>
        /// <typeparam name="T"> 子模块类型 </typeparam>
        /// <param name="args"> 参数 </param>
        /// <returns></returns>
        public T OpenSubModule<T>(object args = null) where T : GameModule
        {
            return (T)OpenSubModule_Internal(typeof(T), args);
        }

        /// <summary>
        /// 关闭所有子模块.
        /// </summary>
        public void CloseSubmodules()
        {
            foreach (var module in GetSubModules_Internal())
            {
                module.Close();
            }
        }

        public void Close()
        {
            Exit();
        }

        private void Enter()
        {
            try
            {
                OnEnter();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void Exit()
        {
            this.SetActiveSelf_Internal(false);
            foreach (var module in GetSubModules_Internal())
            {
                module.Exit();
            }

            try
            {
                OnExit();
            }
            catch (Exception e)
            {
                throw e;
            }

            this.SetParent_Internal(null);
        }

        private void Enable()
        {
            active = true;

            try
            {
                OnEnable();
            }
            catch (Exception e)
            {
                throw e;
            }

            foreach (var module in GetSubModules_Internal())
            {
                module.RefreshActive_Internal();
            }
        }

        private void Disable()
        {
            active = false;
            try
            {
                OnDisable();
            }
            catch (Exception e)
            {
                throw e;
            }

            foreach (var module in GetSubModules_Internal())
            {
                module.RefreshActive_Internal();
            }
        }

        public void Update()
        {
            if (!active)
                return;

            try
            {
                OnUpdate();
            }
            catch (Exception e)
            {
                throw e;
            }

            subModuleUpdateCache.Clear();
            foreach (var module in subModules)
            {
                if (!module.active)
                    continue;

                subModuleUpdateCache.Add(module);
            }

            for (int i = 0; i < subModules.Count; i++)
            {
                var module = subModules[i];
                if (module.parent != this)
                {
                    subModules.RemoveAt(i--);
                    continue;
                }

                if (!subModuleUpdateCache.Contains(module))
                    continue;

                module.Update();
            }
        }

        public void LateUpdate()
        {
            if (!active)
                return;

            try
            {
                OnLateUpdate();
            }
            catch (Exception e)
            {
                throw e;
            }

            subModuleUpdateCache.Clear();
            foreach (var module in subModules)
            {
                if (!module.active)
                    continue;

                subModuleUpdateCache.Add(module);
            }

            for (int i = 0; i < subModules.Count; i++)
            {
                var module = subModules[i];
                if (module.parent != this)
                {
                    subModules.RemoveAt(i--);
                    continue;
                }

                if (!subModuleUpdateCache.Contains(module))
                    continue;

                module.LateUpdate();
            }
        }

        private void ParentChanged()
        {
            try
            {
                OnParentChanged();
            }
            catch (Exception e)
            {
                throw e;
            }

            foreach (var module in GetSubModules_Internal())
            {
                module.ParentChanged();
            }
        }

        /// <summary>
        /// 当模块打开/进入时调用.
        /// </summary>
        protected virtual void OnEnter()
        {
        }

        /// <summary>
        /// 当模块推出时调用.
        /// </summary>
        protected virtual void OnExit()
        {
        }

        /// <summary>
        /// 当模块启用时调用.
        /// </summary>
        protected virtual void OnEnable()
        {
        }

        /// <summary>
        /// 当模块禁用时调用.
        /// </summary>
        protected virtual void OnDisable()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnLateUpdate()
        {
        }

        /// <summary>
        /// 当模块的直接或间接父模块改变时调用.
        /// </summary>
        protected virtual void OnParentChanged()
        {
        }
    }
}