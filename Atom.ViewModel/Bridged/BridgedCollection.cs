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
using System.Threading;

namespace Atom
{
    public class BridgedCollection<T, TE> :
        IList<TE>,
        ICollection<TE>,
        IEnumerable<TE>,
        IEnumerable,
        IList,
        ICollection,
        IReadOnlyList<TE>,
        IReadOnlyCollection<TE>
        where T : IList<TE>, new()
    {
        [NonSerialized] private object _syncRoot;

        private IBridgedValue<T> m_BridgedItems;

        protected IBridgedValue<T> BridgedItems => this.m_BridgedItems;

        private T items => m_BridgedItems.Value;

        public int Count => this.items.Count;

        protected T Items => this.items;

        public BridgedCollection() : this(new T())
        {
        }

        public BridgedCollection(T list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            this.m_BridgedItems = new BridgedValue<T>(list);
        }

        public BridgedCollection(Func<T> listGetter, Action<T> listSetter)
        {
            if (listGetter == null)
                throw new ArgumentNullException(nameof(listGetter));
            this.m_BridgedItems = new BridgedValueGetterSetter<T>(listGetter, listSetter);
        }

        public TE this[int index]
        {
            get => this.items[index];
            set
            {
                if (this.items.IsReadOnly)
                    throw new NotSupportedException("Collection is read-only.");
                if (index < 0 || index >= this.items.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                this.SetItem(index, value);
            }
        }

        protected virtual void ClearItems()
        {
            this.items.Clear();
        }
        
        protected virtual void RemoveItem(int index)
        {
            this.items.RemoveAt(index);
        }

        protected virtual void InsertItem(int index, TE item)
        {
            this.items.Insert(index, item);
        }

        protected virtual void SetItem(int index, TE item)
        {
            this.items[index] = item;
        }

        public void Add(TE item)
        {
            this.InsertItem(this.items.Count, item);
        }

        public void Clear()
        {
            this.ClearItems();
        }

        public void CopyTo(TE[] array, int index) => this.items.CopyTo(array, index);

        public bool Contains(TE item) => this.items.Contains(item);


        public IEnumerator<TE> GetEnumerator() => this.items.GetEnumerator();


        public int IndexOf(TE item) => this.items.IndexOf(item);


        public void Insert(int index, TE item)
        {
            if (this.items.IsReadOnly)
                throw new NotSupportedException("Collection is read-only.");
            if (index < 0 || index > this.items.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            this.InsertItem(index, item);
        }


        public bool Remove(TE item)
        {
            if (this.items.IsReadOnly)
                throw new NotSupportedException("Collection is read-only.");
            int index = this.items.IndexOf(item);
            if (index < 0)
                return false;
            this.RemoveItem(index);
            return true;
        }


        public void RemoveAt(int index)
        {
            if (this.items.IsReadOnly)
                throw new NotSupportedException("Collection is read-only.");
            if (index < 0 || index >= this.items.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            this.RemoveItem(index);
        }


        bool ICollection<TE>.IsReadOnly => this.items.IsReadOnly;


        IEnumerator IEnumerable.GetEnumerator() => this.items.GetEnumerator();


        bool ICollection.IsSynchronized => false;


        object ICollection.SyncRoot
        {
            get
            {
                if (this._syncRoot == null)
                {
                    if (this.items is ICollection items)
                        this._syncRoot = items.SyncRoot;
                    else
                        Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), (object)null);
                }

                return this._syncRoot;
            }
        }


        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (array.Rank != 1)
                throw new ArgumentException(nameof(array));
            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException(nameof(array));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (array.Length - index < this.Count)
                throw new ArgumentException(nameof(array));
            if (array is TE[] array1)
            {
                this.items.CopyTo(array1, index);
            }
            else
            {
                Type elementType = array.GetType().GetElementType();
                Type c = typeof(T);
                if (!elementType.IsAssignableFrom(c) && !c.IsAssignableFrom(elementType))
                    throw new ArgumentException(nameof(array));
                if (!(array is object[] objArray))
                    throw new ArgumentException(nameof(array));
                int count = this.items.Count;
                try
                {
                    for (int index1 = 0; index1 < count; ++index1)
                    {
                        int index2 = index++;
                        // ISSUE: variable of a boxed type
                        var local = (object)this.items[index1];
                        objArray[index2] = (object)local;
                    }
                }
                catch (ArrayTypeMismatchException ex)
                {
                    throw new ArgumentException(nameof(array), ex);
                }
            }
        }


        object IList.this[int index]
        {
            get => (object)this.items[index];
            set
            {
                if (this.items == null)
                    throw new ArgumentNullException(nameof(value));
                try
                {
                    this[index] = (TE)value;
                }
                catch (InvalidCastException ex)
                {
                    throw new ArgumentException(nameof(index), ex);
                }
            }
        }


        bool IList.IsReadOnly => this.items.IsReadOnly;


        bool IList.IsFixedSize => this.items is IList items ? items.IsFixedSize : this.items.IsReadOnly;


        int IList.Add(object value)
        {
            if (this.items.IsReadOnly)
                throw new NotSupportedException("Collection is read-only.");
            if (this.items == null)
                throw new ArgumentNullException(nameof(value));
            try
            {
                this.Add((TE)value);
            }
            catch (InvalidCastException ex)
            {
                throw new ArgumentException(nameof(value), ex);
            }

            return this.Count - 1;
        }


        bool IList.Contains(object value) => IsCompatibleObject(value) && this.Contains((TE)value);


        int IList.IndexOf(object value) => IsCompatibleObject(value) ? this.IndexOf((TE)value) : -1;


        void IList.Insert(int index, object value)
        {
            if (this.items.IsReadOnly)
                throw new NotSupportedException("Collection is read-only.");
            if (this.items == null)
                throw new ArgumentNullException(nameof(value));
            try
            {
                this.Insert(index, (TE)value);
            }
            catch (InvalidCastException ex)
            {
                throw new ArgumentException(nameof(value), ex);
            }
        }


        void IList.Remove(object value)
        {
            if (this.items.IsReadOnly)
                throw new NotSupportedException("Collection is read-only.");
            if (!IsCompatibleObject(value))
                return;
            this.Remove((TE)value);
        }

        private static bool IsCompatibleObject(object value)
        {
            if (value is T)
                return true;
            return value == null && (object)default(T) == null;
        }
    }
}