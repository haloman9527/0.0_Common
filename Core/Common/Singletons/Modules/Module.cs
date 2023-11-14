using System;
using System.Collections.Generic;
using System.Linq;
using CZToolKit.ObjectPool;

namespace CZToolKit
{
    public class Module
    {
        protected readonly static SubModulesQueueCachePool QueueCachePool = new SubModulesQueueCachePool();
        protected readonly static SubModulesStackCachePool StackCachePool = new SubModulesStackCachePool();

        private EventMark mark;
        private bool isUsing;
        private bool isActive;
        private bool isActiveSelf;
        private Module parent;
        private List<Module> subModules = new List<Module>();

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

        public Module Parent
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

        private IEnumerable<Module> EnumerateSubModules_Internal(bool recursive)
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

        private IEnumerable<Module> EnumerateSubModulesWithoutUnused_Internal(bool recursive)
        {
            foreach (var module in EnumerateSubModules_Internal(recursive))
            {
                if (!module.isUsing)
                    continue;
                yield return module;
            }
        }

        private void SetParent_Internal(Module newParent)
        {
            if (this.parent == newParent)
                return;

            var subModulesQueue = QueueCachePool.Acquire();
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

            QueueCachePool.Release(subModulesQueue);

            this.parent?.OnChildChanged();
        }

        private void Open_Internal()
        {
            this.Mark(EventMark.Open);
            this.OnOpen();
        }

        private void Close_Internal()
        {
            var subModulesStack = StackCachePool.Acquire();
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

            StackCachePool.Release(subModulesStack);
        }

        private void ValidActive_Internal()
        {
            var subModulesQueue = QueueCachePool.Acquire();
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

            QueueCachePool.Release(subModulesQueue);
        }

        /// <summary>
        /// 遍历所有子模块.
        /// </summary>
        /// <param name="recursive"> 递归遍历所有子模块 </param>
        /// <returns></returns>
        public IEnumerable<Module> EnumerateSubModules(bool recursive = false)
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
        public Module[] GetSubModules(bool recursive = false)
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
        public void GetSubModules(List<Module> result, bool recursive = false)
        {
            if (!this.isUsing)
                throw new InvalidOperationException();

            result.Clear();
            result.AddRange(EnumerateSubModulesWithoutUnused_Internal(recursive));
        }

        public void OpenSubModule(Module module)
        {
            if (!this.isUsing)
                throw new InvalidOperationException();

            module.Open(this);
        }

        public T OpenSubModule<T>() where T : Module, new()
        {
            if (!this.isUsing)
                throw new InvalidOperationException();

            var module = new T();
            module.Open(this);
            return module;
        }

        public void CloseSubModules()
        {
            if (!this.isUsing)
                throw new InvalidOperationException();

            var subModulesStack = StackCachePool.Acquire();
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

            StackCachePool.Release(subModulesStack);
        }

        /// <summary>
        /// 打开模块
        /// </summary>
        public void Open(Module parent)
        {
            if (this.isUsing)
                throw new InvalidOperationException();

            this.isUsing = true;
            this.isActiveSelf = true;
            this.parent = parent;
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

            var subModulesQueue = QueueCachePool.Acquire();
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

            QueueCachePool.Release(subModulesQueue);
        }

        public void LateUpdate()
        {
            if (!this.isUsing)
                throw new InvalidOperationException();

            var subModulesQueue = QueueCachePool.Acquire();
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

            QueueCachePool.Release(subModulesQueue);
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

        public class SubModulesQueueCachePool : ObjectPool<Queue<Module>>
        {
            protected override Queue<Module> Create()
            {
                return new Queue<Module>();
            }

            protected override void OnAcquire(Queue<Module> unit)
            {
                base.OnAcquire(unit);
                unit.Clear();
            }

            protected override void OnRelease(Queue<Module> unit)
            {
                base.OnRelease(unit);
                unit.Clear();
            }
        }

        public class SubModulesStackCachePool : ObjectPool<Stack<Module>>
        {
            protected override Stack<Module> Create()
            {
                return new Stack<Module>();
            }

            protected override void OnAcquire(Stack<Module> unit)
            {
                base.OnAcquire(unit);
                unit.Clear();
            }

            protected override void OnRelease(Stack<Module> unit)
            {
                base.OnRelease(unit);
                unit.Clear();
            }
        }
    }
}