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

namespace CZToolKit
{
    public interface IViewModel
    {
        int Count { get; }

        IBindableProperty this[string propertyName] { get; set; }

        IEnumerable<string> Keys { get; }

        IEnumerable<IBindableProperty> Values { get; }

        IEnumerator<KeyValuePair<string, IBindableProperty>> GetEnumerator();

        bool Contains(string key);

        void Clear();

        IBindableProperty GetProperty(string propertyName);

        IBindableProperty<T> GetProperty<T>(string propertyName);

        bool TryGetProperty(string propertyName, out IBindableProperty property);

        bool TrygetProperty<T>(string propertyName, out IBindableProperty<T> property);

        void RegisterProperty(string propertyName, IBindableProperty property);

        void UnregisterProperty(string propertyName);

        T GetPropertyValue<T>(string propertyName);

        void SetPropertyValue<T>(string propertyName, T value);

        void ClearValueChangedEvent(string propertyName);
    }

    public class ViewModel : IViewModel
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

        public IEnumerable<string> Keys => InternalBindableProperties.Keys;

        public IEnumerable<IBindableProperty> Values => InternalBindableProperties.Values;

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

        public bool Contains(string propertyName)
        {
            return InternalBindableProperties.ContainsKey(propertyName);
        }

        public IBindableProperty GetProperty(string propertyName)
        {
            if (internalBindableProperties == null)
            {
                return null;
            }

            if (!internalBindableProperties.TryGetValue(propertyName, out var property))
            {
                return null;
            }

            return property;
        }

        public IBindableProperty<T> GetProperty<T>(string propertyName)
        {
            if (internalBindableProperties == null)
            {
                return null;
            }

            if (!internalBindableProperties.TryGetValue(propertyName, out var property))
            {
                return null;
            }

            return property as IBindableProperty<T>;
        }

        public bool TryGetProperty(string propertyName, out IBindableProperty property)
        {
            if (internalBindableProperties == null)
            {
                property = null;
                return false;
            }

            return internalBindableProperties.TryGetValue(propertyName, out property);
        }

        public bool TrygetProperty<T>(string propertyName, out IBindableProperty<T> property)
        {
            if (internalBindableProperties == null)
            {
                property = null;
                return false;
            }

            if (!internalBindableProperties.TryGetValue(propertyName, out var tempProperty))
            {
                property = null;
                return false;
            }

            property = tempProperty as IBindableProperty<T>;
            return property != null;
        }

        public bool Remove(string key)
        {
            return InternalBindableProperties.Remove(key);
        }

        public void Clear()
        {
            InternalBindableProperties.Clear();
        }

        public void RegisterProperty(string propertyName, IBindableProperty property)
        {
            InternalBindableProperties.Add(propertyName, property);
        }

        public void UnregisterProperty(string propertyName)
        {
            InternalBindableProperties.Remove(propertyName);
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