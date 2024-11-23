using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Jiange.Collections
{
    /// <summary>游戏框架链表类。</summary>
    /// <typeparam name="T">指定链表的元素类型。</typeparam>
    public sealed class LinkedList<T> : ICollection<T>, IEnumerable<T>, IEnumerable, ICollection
    {
        private readonly System.Collections.Generic.LinkedList<T> m_LinkedList;
        private readonly Queue<LinkedListNode<T>> m_CachedNodes;

        /// <summary>初始化游戏框架链表类的新实例。</summary>
        public LinkedList()
        {
            this.m_LinkedList = new System.Collections.Generic.LinkedList<T>();
            this.m_CachedNodes = new Queue<LinkedListNode<T>>();
        }

        /// <summary>获取链表中实际包含的结点数量。</summary>
        public int Count => this.m_LinkedList.Count;

        /// <summary>获取链表结点缓存数量。</summary>
        public int CachedNodeCount => this.m_CachedNodes.Count;

        /// <summary>获取链表的第一个结点。</summary>
        public LinkedListNode<T> First => this.m_LinkedList.First;

        /// <summary>获取链表的最后一个结点。</summary>
        public LinkedListNode<T> Last => this.m_LinkedList.Last;

        /// <summary>获取一个值，该值指示 ICollection`1 是否为只读。</summary>
        public bool IsReadOnly => ((ICollection<T>)this.m_LinkedList).IsReadOnly;

        /// <summary>获取可用于同步对 ICollection 的访问的对象。</summary>
        public object SyncRoot => ((ICollection)this.m_LinkedList).SyncRoot;

        /// <summary>获取一个值，该值指示是否同步对 ICollection 的访问（线程安全）。</summary>
        public bool IsSynchronized => ((ICollection)this.m_LinkedList).IsSynchronized;

        /// <summary>在链表中指定的现有结点后添加包含指定值的新结点。</summary>
        /// <param name="node">指定的现有结点。</param>
        /// <param name="value">指定值。</param>
        /// <returns>包含指定值的新结点。</returns>
        public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)
        {
            LinkedListNode<T> newNode = this.AcquireNode(value);
            this.m_LinkedList.AddAfter(node, newNode);
            return newNode;
        }

        /// <summary>在链表中指定的现有结点后添加指定的新结点。</summary>
        /// <param name="node">指定的现有结点。</param>
        /// <param name="newNode">指定的新结点。</param>
        public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode) => this.m_LinkedList.AddAfter(node, newNode);

        /// <summary>在链表中指定的现有结点前添加包含指定值的新结点。</summary>
        /// <param name="node">指定的现有结点。</param>
        /// <param name="value">指定值。</param>
        /// <returns>包含指定值的新结点。</returns>
        public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
        {
            LinkedListNode<T> newNode = this.AcquireNode(value);
            this.m_LinkedList.AddBefore(node, newNode);
            return newNode;
        }

        /// <summary>在链表中指定的现有结点前添加指定的新结点。</summary>
        /// <param name="node">指定的现有结点。</param>
        /// <param name="newNode">指定的新结点。</param>
        public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode) => this.m_LinkedList.AddBefore(node, newNode);

        /// <summary>在链表的开头处添加包含指定值的新结点。</summary>
        /// <param name="value">指定值。</param>
        /// <returns>包含指定值的新结点。</returns>
        public LinkedListNode<T> AddFirst(T value)
        {
            LinkedListNode<T> node = this.AcquireNode(value);
            this.m_LinkedList.AddFirst(node);
            return node;
        }

        /// <summary>在链表的开头处添加指定的新结点。</summary>
        /// <param name="node">指定的新结点。</param>
        public void AddFirst(LinkedListNode<T> node) => this.m_LinkedList.AddFirst(node);

        /// <summary>在链表的结尾处添加包含指定值的新结点。</summary>
        /// <param name="value">指定值。</param>
        /// <returns>包含指定值的新结点。</returns>
        public LinkedListNode<T> AddLast(T value)
        {
            LinkedListNode<T> node = this.AcquireNode(value);
            this.m_LinkedList.AddLast(node);
            return node;
        }

        /// <summary>在链表的结尾处添加指定的新结点。</summary>
        /// <param name="node">指定的新结点。</param>
        public void AddLast(LinkedListNode<T> node) => this.m_LinkedList.AddLast(node);

        /// <summary>从链表中移除所有结点。</summary>
        public void Clear()
        {
            for (LinkedListNode<T> node = this.m_LinkedList.First; node != null; node = node.Next)
                this.ReleaseNode(node);
            this.m_LinkedList.Clear();
        }

        /// <summary>清除链表结点缓存。</summary>
        public void ClearCachedNodes() => this.m_CachedNodes.Clear();

        /// <summary>确定某值是否在链表中。</summary>
        /// <param name="value">指定值。</param>
        /// <returns>某值是否在链表中。</returns>
        public bool Contains(T value) => this.m_LinkedList.Contains(value);

        /// <summary>从目标数组的指定索引处开始将整个链表复制到兼容的一维数组。</summary>
        /// <param name="array">一维数组，它是从链表复制的元素的目标。数组必须具有从零开始的索引。</param>
        /// <param name="index">array 中从零开始的索引，从此处开始复制。</param>
        public void CopyTo(T[] array, int index) => this.m_LinkedList.CopyTo(array, index);

        /// <summary>从特定的 ICollection 索引开始，将数组的元素复制到一个数组中。</summary>
        /// <param name="array">一维数组，它是从 ICollection 复制的元素的目标。数组必须具有从零开始的索引。</param>
        /// <param name="index">array 中从零开始的索引，从此处开始复制。</param>
        public void CopyTo(Array array, int index) => ((ICollection)this.m_LinkedList).CopyTo(array, index);

        /// <summary>查找包含指定值的第一个结点。</summary>
        /// <param name="value">要查找的指定值。</param>
        /// <returns>包含指定值的第一个结点。</returns>
        public LinkedListNode<T> Find(T value) => this.m_LinkedList.Find(value);

        /// <summary>查找包含指定值的最后一个结点。</summary>
        /// <param name="value">要查找的指定值。</param>
        /// <returns>包含指定值的最后一个结点。</returns>
        public LinkedListNode<T> FindLast(T value) => this.m_LinkedList.FindLast(value);

        /// <summary>从链表中移除指定值的第一个匹配项。</summary>
        /// <param name="value">指定值。</param>
        /// <returns>是否移除成功。</returns>
        public bool Remove(T value)
        {
            LinkedListNode<T> node = this.m_LinkedList.Find(value);
            if (node == null)
                return false;
            this.m_LinkedList.Remove(node);
            this.ReleaseNode(node);
            return true;
        }

        /// <summary>从链表中移除指定的结点。</summary>
        /// <param name="node">指定的结点。</param>
        public void Remove(LinkedListNode<T> node)
        {
            this.m_LinkedList.Remove(node);
            this.ReleaseNode(node);
        }

        /// <summary>移除位于链表开头处的结点。</summary>
        public void RemoveFirst()
        {
            LinkedListNode<T> first = this.m_LinkedList.First;
            if (first == null)
                throw new Exception("First is invalid.");
            this.m_LinkedList.RemoveFirst();
            this.ReleaseNode(first);
        }

        /// <summary>移除位于链表结尾处的结点。</summary>
        public void RemoveLast()
        {
            LinkedListNode<T> last = this.m_LinkedList.Last;
            if (last == null)
                throw new Exception("Last is invalid.");
            this.m_LinkedList.RemoveLast();
            this.ReleaseNode(last);
        }

        /// <summary>返回循环访问集合的枚举数。</summary>
        /// <returns>循环访问集合的枚举数。</returns>
        public LinkedList<T>.Enumerator GetEnumerator() => new LinkedList<T>.Enumerator(this.m_LinkedList);

        private LinkedListNode<T> AcquireNode(T value)
        {
            LinkedListNode<T> linkedListNode;
            if (this.m_CachedNodes.Count > 0)
            {
                linkedListNode = this.m_CachedNodes.Dequeue();
                linkedListNode.Value = value;
            }
            else
                linkedListNode = new LinkedListNode<T>(value);

            return linkedListNode;
        }

        private void ReleaseNode(LinkedListNode<T> node)
        {
            node.Value = default(T);
            this.m_CachedNodes.Enqueue(node);
        }

        /// <summary>将值添加到 ICollection`1 的结尾处。</summary>
        /// <param name="value">要添加的值。</param>
        void ICollection<T>.Add(T value) => this.AddLast(value);

        /// <summary>返回循环访问集合的枚举数。</summary>
        /// <returns>循环访问集合的枚举数。</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => (IEnumerator<T>)this.GetEnumerator();

        /// <summary>返回循环访问集合的枚举数。</summary>
        /// <returns>循环访问集合的枚举数。</returns>
        IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)this.GetEnumerator();

        /// <summary>循环访问集合的枚举数。</summary>
        [StructLayout(LayoutKind.Auto)]
        public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            private System.Collections.Generic.LinkedList<T>.Enumerator m_Enumerator;

            internal Enumerator(System.Collections.Generic.LinkedList<T> linkedList) => this.m_Enumerator = linkedList != null ? linkedList.GetEnumerator() : throw new Exception("Linked list is invalid.");

            /// <summary>获取当前结点。</summary>
            public T Current => this.m_Enumerator.Current;

            /// <summary>获取当前的枚举数。</summary>
            object IEnumerator.Current => (object)this.m_Enumerator.Current;

            /// <summary>清理枚举数。</summary>
            public void Dispose() => this.m_Enumerator.Dispose();

            /// <summary>获取下一个结点。</summary>
            /// <returns>返回下一个结点。</returns>
            public bool MoveNext() => this.m_Enumerator.MoveNext();

            /// <summary>重置枚举数。</summary>
            void IEnumerator.Reset() => ((IEnumerator)this.m_Enumerator).Reset();
        }
    }
}