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
 *  Blog: https://www.mindgear.net/
 *
 */

#endregion

using System;
using System.Collections;
using System.Collections.Generic;

namespace CZToolKit
{
    public class ViewModel : IDictionary<string, IBindableProperty>, IReadOnlyDictionary<string, IBindableProperty>
    {
        private Dictionary<string, IBindableProperty> internalBindableProperties;

        Dictionary<string, IBindableProperty> InternalBindableProperties
        {
            get
            {
                if (internalBindableProperties == null)
                    internalBindableProperties = new Dictionary<string, IBindableProperty>();
                return internalBindableProperties;
            }
        }

        public int Count => InternalBindableProperties.Count;

        public bool IsReadOnly => (InternalBindableProperties as IDictionary).IsReadOnly;

        public IEnumerable<string> Keys => InternalBindableProperties.Keys;

        public IEnumerable<IBindableProperty> Values => InternalBindableProperties.Values;

        ICollection<IBindableProperty> IDictionary<string, IBindableProperty>.Values => InternalBindableProperties.Values;

        ICollection<string> IDictionary<string, IBindableProperty>.Keys => InternalBindableProperties.Keys;

        public IBindableProperty this[string propertyName]
        {
            get { return InternalBindableProperties[propertyName]; }
            set { InternalBindableProperties[propertyName] = value; }
        }

        public IEnumerator<KeyValuePair<string, IBindableProperty>> GetEnumerator()
        {
            return InternalBindableProperties.GetEnumerator();
        }

        public bool Contains(KeyValuePair<string, IBindableProperty> item)
        {
            return (InternalBindableProperties as ICollection<KeyValuePair<string, IBindableProperty>>).Contains(item);
        }

        public bool Remove(KeyValuePair<string, IBindableProperty> item)
        {
            return (InternalBindableProperties as IDictionary<string, IBindableProperty>).Remove(item);
        }

        public void Add(string key, IBindableProperty value)
        {
            InternalBindableProperties.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return InternalBindableProperties.ContainsKey(key);
        }

        public bool TryGetValue(string key, out IBindableProperty value)
        {
            return InternalBindableProperties.TryGetValue(key, out value);
        }

        public bool Remove(string key)
        {
            return InternalBindableProperties.Remove(key);
        }

        public void Clear()
        {
            InternalBindableProperties.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InternalBindableProperties.GetEnumerator();
        }

        void ICollection<KeyValuePair<string, IBindableProperty>>.Add(KeyValuePair<string, IBindableProperty> item)
        {
            (InternalBindableProperties as ICollection<KeyValuePair<string, IBindableProperty>>).Add(item);
        }

        void ICollection<KeyValuePair<string, IBindableProperty>>.CopyTo(KeyValuePair<string, IBindableProperty>[] array, int arrayIndex)
        {
            (InternalBindableProperties as ICollection<KeyValuePair<string, IBindableProperty>>).CopyTo(array, arrayIndex);
        }

        public void RegisterProperty(string propertyName, IBindableProperty property)
        {
            this.Add(propertyName, property);
        }

        public void UnregisterProperty(string propertyName)
        {
            this.Remove(propertyName);
        }

        public T GetPropertyValue<T>(string propertyName)
        {
            return this[propertyName].AsBindableProperty<T>().Value;
        }

        public void SetPropertyValue<T>(string propertyName, T value)
        {
            this[propertyName].AsBindableProperty<T>().Value = value;
        }

        public void BindProperty<T>(string propertyName, ValueChangedEvent<T> onValueChangedCallback)
        {
            this[propertyName].AsBindableProperty<T>().RegisterValueChangedEvent(onValueChangedCallback);
        }

        public void UnBindProperty<T>(string propertyName, ValueChangedEvent<T> onValueChangedCallback)
        {
            this[propertyName].AsBindableProperty<T>().UnregisterValueChangedEvent(onValueChangedCallback);
        }

        public void ClearValueChangedEvent(string propertyName)
        {
            this[propertyName].ClearValueChangedEvent();
        }
    }
}