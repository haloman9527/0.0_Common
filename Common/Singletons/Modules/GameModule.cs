using System;
using System.Collections.Generic;
using System.Linq;

namespace CZToolKit
{
    public class GameModule
    {
        [Flags]
        public enum EventMark
        {
            None = 0,
            Open = 1 << 0,
            Close = 1 << 1,
            Active = 1 << 2,
            Inactive = 1 << 3,
            Update = 1 << 4,
            LateUpdate = 1 << 5,
        }

        private EventMark mark;
        private bool isUsing;
        private bool isActive;
        private bool isActiveSelf;
        private GameModule parent;
        private List<GameModule> subModules = new List<GameModule>();
        private Queue<GameModule> subModulesQueue = new Queue<GameModule>();
        private Stack<GameModule> subModulesStack = new Stack<GameModule>();

        public bool IsUsing
        {
            get { return isUsing; }
        }
        
        public bool Active
        {
            get { return isActive; }
            set
            {
                this.isActiveSelf = value;
                ValidActive_Internal();
            }
        }

        public bool ActiveSelf
        {
            get { return isActiveSelf; }
        }

        public GameModule Parent
        {
            get { return parent; }
            set { SetParent_Internal(value); }
        }

        private void Mark(EventMark mark)
        {
            this.mark |= mark;
        }

        private void Erase(EventMark mark)
        {
            this.mark &= ~mark;
        }

        private bool HasMark(EventMark mark)
        {
            return (this.mark & mark) == mark;
        }

        private IEnumerable<GameModule> EnumerateSubModules_Internal(bool recursive)
        {
            if (!recursive)
            {
                foreach (var module in subModules)
                {
                    yield return module;
                }
            }
            else
            {
                foreach (var module in subModules)
                {
                    yield return module;
                    foreach (var subModule in module.EnumerateSubModules_Internal(true))
                    {
                        yield return subModule;
                    }
                }
            }
        }

        private IEnumerable<GameModule> EnumerateSubModulesWithoutUnused_Internal(bool recursive)
        {
            foreach (var module in EnumerateSubModules_Internal(recursive))
            {
                if (!module.isUsing)
                    continue;
                yield return module;
            }
        }

        private void SetParent_Internal(GameModule newParent)
        {
            if (this.parent == newParent)
                return;

            subModulesQueue.Clear();
            subModulesQueue.Enqueue(this);
            foreach (var module in EnumerateSubModulesWithoutUnused_Internal(false))
            {
                subModulesQueue.Enqueue(module);
            }

            while (subModulesQueue.Count > 0)
            {
                var module = subModulesQueue.Dequeue();
                if (!module.isUsing)
                    continue;

                module.OnBeforeParentChanged();
            }

            this.parent?.subModules.Remove(this);
            this.parent?.OnChildChanged();

            this.parent = newParent;
            this.parent?.subModules.Add(this);

            subModulesQueue.Clear();
            subModulesQueue.Enqueue(this);
            foreach (var module in EnumerateSubModulesWithoutUnused_Internal(true))
            {
                subModulesQueue.Enqueue(module);
            }

            while (subModulesQueue.Count > 0)
            {
                var module = subModulesQueue.Dequeue();
                if (!module.isUsing)
                    continue;

                module.ValidActive_Internal();
                module.OnParentChanged();
            }

            this.parent?.OnChildChanged();
        }

        private void Open_Internal()
        {
            this.Mark(EventMark.Open);
            this.OnOpen();
        }

        private void Close_Internal()
        {
            subModulesStack.Clear();
            subModulesStack.Push(this);
            foreach (var module in EnumerateSubModulesWithoutUnused_Internal(true))
            {
                subModulesStack.Push(module);
            }

            while (subModulesStack.Count > 0)
            {
                var module = subModulesStack.Pop();
                if (module.HasMark(EventMark.Close))
                    continue;
                module.Mark(EventMark.Close);
                module.OnClose();
            }
        }

        private void ValidActive_Internal()
        {
            subModulesQueue.Clear();
            subModulesQueue.Enqueue(this);
            foreach (var module in EnumerateSubModulesWithoutUnused_Internal(true))
            {
                subModulesQueue.Enqueue(module);
            }

            while (subModulesQueue.Count > 0)
            {
                var module = subModulesQueue.Dequeue();
                if (!module.isUsing)
                    continue;
                var targetActive = parent == null ? isActiveSelf : parent.isActive && isActiveSelf;
                if (module.isActive == targetActive)
                    return;
                module.isActive = targetActive;
                if (module.isActive)
                    OnActive();
                else
                    OnInactive();
            }
        }

        /// <summary>
        /// 遍历所有子模块.
        /// </summary>
        /// <param name="recursive"> 递归遍历所有子模块 </param>
        /// <returns></returns>
        public IEnumerable<GameModule> EnumerateSubModules(bool recursive = false)
        {
            if (!this.isUsing)
                throw new InvalidOperationException();
                
            return EnumerateSubModulesWithoutUnused_Internal(recursive);
        }

        /// <summary>
        /// 获取所有子模块.
        /// </summary>
        /// <param name="recursive"> 递归获取所有子模块 </param>
        /// <returns></returns>
        public GameModule[] GetSubModules(bool recursive = false)
        {
            if (!this.isUsing)
                throw new InvalidOperationException();

            return EnumerateSubModulesWithoutUnused_Internal(recursive).ToArray();
        }

        /// <summary>
        /// 获取所有子模块.
        /// </summary>
        /// <param name="result"> 结果 </param>
        /// <param name="recursive"> 递归获取所有子模块 </param>
        /// <returns></returns>
        public void GetSubModules(List<GameModule> result, bool recursive = false)
        {
            if (!this.isUsing)
                throw new InvalidOperationException();
                
            result.Clear();
            result.AddRange(EnumerateSubModulesWithoutUnused_Internal(recursive));
        }

        public void OpenSubModule(GameModule module)
        {
            if (!this.isUsing)
                throw new InvalidOperationException();

            module.Open(this);
        }

        public void CloseSubModules()
        {
            if (!this.isUsing)
                throw new InvalidOperationException();

            subModulesStack.Clear();
            foreach (var module in EnumerateSubModulesWithoutUnused_Internal(false))
            {
                subModulesStack.Push(module);
            }

            while (subModulesStack.Count > 0)
            {
                var module = subModulesStack.Pop();
                if (!module.isUsing)
                    continue;
                module.Close();
            }
        }

        /// <summary>
        /// 打开模块
        /// </summary>
        public void Open(GameModule newParent)
        {
            if (this.isUsing)
                throw new InvalidOperationException();

            this.isUsing = true;
            this.isActiveSelf = true;
            this.parent = newParent;
            this.parent?.subModules.Add(this);
            this.parent?.OnChildChanged();
            this.Open_Internal();
            this.ValidActive_Internal();
        }

        /// <summary>
        /// 关闭模块
        /// </summary>
        public void Close()
        {
            if (!this.isUsing)
                throw new InvalidOperationException();
            
            this.isActiveSelf = false;
            this.ValidActive_Internal();
            this.Close_Internal();
            this.isUsing = false;
            this.mark = EventMark.None;
            this.parent?.subModules.Remove(this);
            this.parent?.OnChildChanged();
        }

        public void Update()
        {
            if (!this.isUsing)
                throw new InvalidOperationException();
                
            subModulesQueue.Clear();
            subModulesQueue.Enqueue(this);
            foreach (var module in EnumerateSubModulesWithoutUnused_Internal(true))
            {
                subModulesQueue.Enqueue(module);
            }

            while (subModulesQueue.Count > 0)
            {
                var module = subModulesQueue.Dequeue();
                if (!module.isUsing)
                    continue;
                if (!module.isActive)
                    continue;
                module.OnUpdate();
            }
        }

        public void LateUpdate()
        {
            if (!this.isUsing)
                throw new InvalidOperationException();
                
            subModulesQueue.Clear();
            subModulesQueue.Enqueue(this);
            foreach (var module in EnumerateSubModulesWithoutUnused_Internal(true))
            {
                subModulesQueue.Enqueue(module);
            }

            while (subModulesQueue.Count > 0)
            {
                var module = subModulesQueue.Dequeue();
                if (!module.isUsing)
                    continue;
                if (!module.isActive)
                    continue;
                module.OnLateUpdate();
            }
        }

        /// <summary>
        /// 当模块打开时调用.
        /// </summary>
        protected virtual void OnOpen()
        {
        }

        /// <summary>
        /// 当模块关闭时调用.
        /// </summary>
        protected virtual void OnClose()
        {
        }

        /// <summary>
        /// 当模块启用时调用.
        /// </summary>
        protected virtual void OnActive()
        {
        }

        /// <summary>
        /// 当模块禁用时调用.
        /// </summary>
        protected virtual void OnInactive()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnLateUpdate()
        {
        }

        /// <summary>
        /// 当模块的直接或间接父模块将要改变时调用.
        /// </summary>
        protected virtual void OnBeforeParentChanged()
        {
        }

        /// <summary>
        /// 当模块的直接或间接父模块改变时调用.
        /// </summary>
        protected virtual void OnParentChanged()
        {
        }

        /// <summary>
        /// 当模块的直接子模块改变时调用.
        /// </summary>
        protected virtual void OnChildChanged()
        {
        }
    }
}