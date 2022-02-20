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
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
using System;
using System.Collections;
using System.Collections.Generic;

namespace CZToolKit.Core.BindableProperty
{
    public class BindableDictionary<TKey, TValue> : BindableProperty<Dictionary<TKey, TValue>>, IDictionary<TKey, TValue>
    {
        public event Action<TKey> onAdded;
        public event Action<TKey, TValue> onItemValueChanged;
        public event Action<TKey> onRemoved;
        public event Action onClear;

        public ICollection<TKey> Keys
        {
            get { return Value.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return Value.Values; }
        }

        public int Count
        {
            get { return Value.Count; }
        }

        public BindableDictionary() : base()
        {
            SetValueWithoutNotify(new Dictionary<TKey, TValue>());
        }

        public TValue this[TKey key]
        {
            get { return Value[key]; }
            set
            {
                if (Value.ContainsKey(key))
                {
                    Value[key] = value;
                    onItemValueChanged?.Invoke(key, value);
                }
                else
                {
                    Value[key] = value;
                    onAdded?.Invoke(key);
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        public void Add(TKey key, TValue value)
        {
            Value.Add(key, value);
            onAdded?.Invoke(key);
        }

        public bool ContainsKey(TKey key)
        {
            return Value.ContainsKey(key);
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            var result = Value.Remove(key);
            if (result)
                onRemoved?.Invoke(key);
            return result;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Value.TryGetValue(key, out value);
        }

        public void Clear()
        {
            Value.Clear();
            onClear?.Invoke();
        }

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }
    }
}
