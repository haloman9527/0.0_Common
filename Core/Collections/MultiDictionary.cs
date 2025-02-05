using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Moyo.Collections
{
    /// <summary>游戏框架多值字典类。</summary>
    /// <typeparam name="TKey">指定多值字典的主键类型。</typeparam>
    /// <typeparam name="TValue">指定多值字典的值类型。</typeparam>
    public sealed class MultiDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, LinkedListRange<TValue>>>, IEnumerable
    {
        private readonly LinkedList<TValue> m_LinkedList;
        private readonly Dictionary<TKey, LinkedListRange<TValue>> m_Dictionary;

        /// <summary>初始化游戏框架多值字典类的新实例。</summary>
        public MultiDictionary()
        {
            this.m_LinkedList = new LinkedList<TValue>();
            this.m_Dictionary = new Dictionary<TKey, LinkedListRange<TValue>>();
        }

        /// <summary>获取多值字典中实际包含的主键数量。</summary>
        public int Count => this.m_Dictionary.Count;

        /// <summary>获取多值字典中指定主键的范围。</summary>
        /// <param name="key">指定的主键。</param>
        /// <returns>指定主键的范围。</returns>
        public LinkedListRange<TValue> this[TKey key]
        {
            get
            {
                this.m_Dictionary.TryGetValue(key, out var frameworkLinkedListRange);
                return frameworkLinkedListRange;
            }
        }

        /// <summary>清理多值字典。</summary>
        public void Clear()
        {
            this.m_Dictionary.Clear();
            this.m_LinkedList.Clear();
        }

        /// <summary>检查多值字典中是否包含指定主键。</summary>
        /// <param name="key">要检查的主键。</param>
        /// <returns>多值字典中是否包含指定主键。</returns>
        public bool Contains(TKey key) => this.m_Dictionary.ContainsKey(key);

        /// <summary>检查多值字典中是否包含指定值。</summary>
        /// <param name="key">要检查的主键。</param>
        /// <param name="value">要检查的值。</param>
        /// <returns>多值字典中是否包含指定值。</returns>
        public bool Contains(TKey key, TValue value)
        {
            return this.m_Dictionary.TryGetValue(key, out var frameworkLinkedListRange) && frameworkLinkedListRange.Contains(value);
        }

        /// <summary>尝试获取多值字典中指定主键的范围。</summary>
        /// <param name="key">指定的主键。</param>
        /// <param name="range">指定主键的范围。</param>
        /// <returns>是否获取成功。</returns>
        public bool TryGetValue(TKey key, out LinkedListRange<TValue> range) => this.m_Dictionary.TryGetValue(key, out range);

        /// <summary>向指定的主键增加指定的值。</summary>
        /// <param name="key">指定的主键。</param>
        /// <param name="value">指定的值。</param>
        public void Add(TKey key, TValue value)
        {
            if (this.m_Dictionary.TryGetValue(key, out var frameworkLinkedListRange))
            {
                this.m_LinkedList.AddBefore(frameworkLinkedListRange.Terminal, value);
            }
            else
            {
                LinkedListNode<TValue> first = this.m_LinkedList.AddLast(value);
                LinkedListNode<TValue> terminal = this.m_LinkedList.AddLast(default(TValue));
                this.m_Dictionary.Add(key, new LinkedListRange<TValue>(first, terminal));
            }
        }

        /// <summary>从指定的主键中移除指定的值。</summary>
        /// <param name="key">指定的主键。</param>
        /// <param name="value">指定的值。</param>
        /// <returns>是否移除成功。</returns>
        public bool Remove(TKey key, TValue value)
        {
            if (this.m_Dictionary.TryGetValue(key, out var frameworkLinkedListRange))
            {
                for (LinkedListNode<TValue> node = frameworkLinkedListRange.First; node != null && node != frameworkLinkedListRange.Terminal; node = node.Next)
                {
                    if (node.Value.Equals((object)value))
                    {
                        if (node == frameworkLinkedListRange.First)
                        {
                            LinkedListNode<TValue> next = node.Next;
                            if (next == frameworkLinkedListRange.Terminal)
                            {
                                this.m_LinkedList.Remove(next);
                                this.m_Dictionary.Remove(key);
                            }
                            else
                                this.m_Dictionary[key] = new LinkedListRange<TValue>(next, frameworkLinkedListRange.Terminal);
                        }

                        this.m_LinkedList.Remove(node);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>从指定的主键中移除所有的值。</summary>
        /// <param name="key">指定的主键。</param>
        /// <returns>是否移除成功。</returns>
        public bool RemoveAll(TKey key)
        {
            if (!this.m_Dictionary.TryGetValue(key, out var frameworkLinkedListRange))
                return false;
            this.m_Dictionary.Remove(key);
            LinkedListNode<TValue> next;
            for (LinkedListNode<TValue> node = frameworkLinkedListRange.First; node != null; node = next)
            {
                next = node != frameworkLinkedListRange.Terminal ? node.Next : (LinkedListNode<TValue>)null;
                this.m_LinkedList.Remove(node);
            }

            return true;
        }

        /// <summary>返回循环访问集合的枚举数。</summary>
        /// <returns>循环访问集合的枚举数。</returns>
        public MultiDictionary<TKey, TValue>.Enumerator GetEnumerator() => new MultiDictionary<TKey, TValue>.Enumerator(this.m_Dictionary);

        IEnumerator<KeyValuePair<TKey, LinkedListRange<TValue>>> IEnumerable<KeyValuePair<TKey, LinkedListRange<TValue>>>.GetEnumerator() => (IEnumerator<KeyValuePair<TKey, LinkedListRange<TValue>>>)this.GetEnumerator();

        /// <summary>返回循环访问集合的枚举数。</summary>
        /// <returns>循环访问集合的枚举数。</returns>
        IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)this.GetEnumerator();

        /// <summary>循环访问集合的枚举数。</summary>
        [StructLayout(LayoutKind.Auto)]
        public struct Enumerator :
            IEnumerator<KeyValuePair<TKey, LinkedListRange<TValue>>>,
            IDisposable,
            IEnumerator
        {
            private Dictionary<TKey, LinkedListRange<TValue>>.Enumerator m_Enumerator;

            internal Enumerator(
                Dictionary<TKey, LinkedListRange<TValue>> dictionary)
            {
                this.m_Enumerator = dictionary != null ? dictionary.GetEnumerator() : throw new Exception("Dictionary is invalid.");
            }

            /// <summary>获取当前结点。</summary>
            public KeyValuePair<TKey, LinkedListRange<TValue>> Current => this.m_Enumerator.Current;

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