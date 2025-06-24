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
using System.Collections.Generic;

namespace Atom
{
    public delegate ref V RefFunc<V>();

    public class ExternalViewModel
    {
        private readonly Dictionary<string, IBindableProperty> m_BindableProperties = new Dictionary<string, IBindableProperty>();
        public event Action<object, string> PropertyChanged;

        public IReadOnlyDictionary<string, IBindableProperty> Properties
        {
            get { return m_BindableProperties; }
        }
        
        public int Count
        {
            get { return m_BindableProperties.Count; }
        }

        public bool Contains(string propertyName)
        {
            return m_BindableProperties.ContainsKey(propertyName);
        }

        public bool TryGetProperty(string propertyName, out IBindableProperty property)
        {
            if (m_BindableProperties == null)
            {
                property = null;
                return false;
            }

            return m_BindableProperties.TryGetValue(propertyName, out property);
        }

        public bool TryGetProperty<T>(string propertyName, out IBindableProperty<T> property)
        {
            if (!m_BindableProperties.TryGetValue(propertyName, out var tempProperty))
            {
                property = null;
                return false;
            }

            property = tempProperty as IBindableProperty<T>;
            return property != null;
        }

        public IBindableProperty GetProperty(string propertyName)
        {
            if (!m_BindableProperties.TryGetValue(propertyName, out var property))
            {
                return null;
            }

            return property;
        }

        public IBindableProperty<T> GetProperty<T>(string propertyName)
        {
            if (!m_BindableProperties.TryGetValue(propertyName, out var property))
            {
                return null;
            }

            return property as IBindableProperty<T>;
        }

        public void RegisterProperty(string propertyName, IBindableProperty property)
        {
            m_BindableProperties.Add(propertyName, property);
        }

        public void RegisterProperty<T>(string propertyName, Func<T> getter = null, Action<T> setter = null)
        {
            m_BindableProperties.Add(propertyName, new BindableProperty<T>(getter, setter));
        }

        public void RegisterProperty<T>(string propertyName, RefFunc<T> getter)
        {
            m_BindableProperties.Add(propertyName, new BindableProperty<T>(() => getter(), v => getter() = v));
        }

        public void UnregisterProperty(string propertyName)
        {
            if (!m_BindableProperties.TryGetValue(propertyName, out var property))
            {
                return;
            }

            m_BindableProperties.Remove(propertyName);
        }

        public T GetPropertyValue<T>(string propertyName)
        {
            return GetProperty<T>(propertyName).Value;
        }

        public void SetPropertyValue<T>(string propertyName, T value)
        {
            var property = GetProperty<T>(propertyName);
            if (!property.SetValue(value))
            {
                return;
            }

            PropertyChanged?.Invoke(this, propertyName);
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            GetProperty(propertyName)?.NotifyValueChanged();
            PropertyChanged?.Invoke(this, propertyName);
        }

        public void Reset()
        {
            m_BindableProperties.Clear();
        }
    }
}