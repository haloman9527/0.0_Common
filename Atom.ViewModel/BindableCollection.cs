#region 注 释

/***
 *
 *  Title:
 *
 *  Description:
 *
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace Atom
{
    public partial class BindableCollection<T, TE> : BridgedCollection<T, TE> where T : IList<TE>, new()
    {
        private SimpleMonitor _monitor = new SimpleMonitor();

        [field: NonSerialized] public event NotifyCollectionChangedEventHandler CollectionChanged;
        [field: NonSerialized] public event Action CountChanged;
        [field: NonSerialized] public event Action ItemsChanged;

        public BindableCollection() : base(new T())
        {
            this.bindableProperty = new BindableProperty<T>(this.BridgedItems);
        }

        public BindableCollection(T list) : base(list)
        {
            this.bindableProperty = new BindableProperty<T>(this.BridgedItems);
        }

        public BindableCollection(Func<T> getter, Action<T> setter) : base(getter, setter)
        {
            this.bindableProperty = new BindableProperty<T>(this.BridgedItems);
        }

        protected override void ClearItems()
        {
            this.CheckReentrancy();
            base.ClearItems();
            this.CountChanged?.Invoke();
            this.ItemsChanged?.Invoke();
            this.OnCollectionReset();
        }

        protected override void RemoveItem(int index)
        {
            this.CheckReentrancy();
            TE obj = this[index];
            base.RemoveItem(index);
            this.CountChanged?.Invoke();
            this.ItemsChanged?.Invoke();
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, (object)obj, index);
        }

        protected override void InsertItem(int index, TE item)
        {
            this.CheckReentrancy();
            base.InsertItem(index, item);
            this.CountChanged?.Invoke();
            this.ItemsChanged?.Invoke();
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, (object)item, index);
        }

        protected override void SetItem(int index, TE item)
        {
            this.CheckReentrancy();
            TE oldItem = this[index];
            base.SetItem(index, item);
            this.ItemsChanged?.Invoke();
            this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, (object)oldItem, (object)item, index);
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            this.CheckReentrancy();
            TE obj = this[oldIndex];
            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, obj);
            this.ItemsChanged?.Invoke();
            this.OnCollectionChanged(NotifyCollectionChangedAction.Move, (object)obj, newIndex, oldIndex);
        }

        protected IDisposable BlockReentrancy()
        {
            this._monitor.Enter();
            return (IDisposable)this._monitor;
        }

        protected void CheckReentrancy()
        {
            if (this._monitor.Busy && this.CollectionChanged != null && this.CollectionChanged.GetInvocationList().Length > 1)
                throw new InvalidOperationException("Observable Collection Reentrancy Not Allowed");
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged == null)
                return;
            using (this.BlockReentrancy())
                this.CollectionChanged((object)this, e);
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        private void OnCollectionReset()
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        [Serializable]
        private class SimpleMonitor : IDisposable
        {
            private int _busyCount;

            public void Enter() => ++this._busyCount;

            public void Dispose() => --this._busyCount;

            public bool Busy => this._busyCount > 0;
        }
    }

    public partial class BindableCollection<T, TE> : IBindableProperty, IBindableProperty<T>
    {
        private BindableProperty<T> bindableProperty;

        public event Action<object, object> BoxedValueChanged
        {
            add { this.bindableProperty.BoxedValueChanged += value; }
            remove { this.bindableProperty.BoxedValueChanged -= value; }
        }

        public event Action<T, T> ValueChanged
        {
            add { this.bindableProperty.ValueChanged += value; }
            remove { this.bindableProperty.ValueChanged -= value; }
        }

        public object BoxedValue
        {
            get => bindableProperty.BoxedValue;
            set => bindableProperty.BoxedValue = value;
        }

        public T Value
        {
            get => bindableProperty.Value;
            set => bindableProperty.Value = value;
        }

        public Type ValueType => bindableProperty.ValueType;

        public bool SetValue(T value)
        {
            this.CheckReentrancy();
            this.Value = value;
            this.CountChanged?.Invoke();
            this.ItemsChanged?.Invoke();
            this.OnCollectionReset();
            return true;
        }

        public void SetValueWithoutNotify(T value)
        {
            this.CheckReentrancy();
            this.bindableProperty.SetValueWithoutNotify(value);
        }

        public void ClearValueChangedEvent() => this.bindableProperty.ClearValueChangedEvent();

        public void NotifyValueChanged() => this.bindableProperty.NotifyValueChanged();

        public bool SetValue(object value) => this.bindableProperty.SetValue(value);

        public void SetValueWithoutNotify(object value) => this.bindableProperty.SetValue(value);

        public void RegisterValueChangedEvent(Action<T, T> onValueChanged) => this.bindableProperty.RegisterValueChangedEvent(onValueChanged);

        public void UnregisterValueChangedEvent(Action<T, T> onValueChanged) => this.bindableProperty.UnregisterValueChangedEvent(onValueChanged);
    }
}