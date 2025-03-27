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
        private readonly Dictionary<string, IBindableProperty> bindableProperties = new Dictionary<string, IBindableProperty>();

        public event Action<object, string> PropertyChanged;

        public int Count => bindableProperties.Count;

        public IReadOnlyDictionary<string, IBindableProperty> Properties => bindableProperties;

        public bool Contains(string propertyName)
        {
            return bindableProperties.ContainsKey(propertyName);
        }

        public bool TryGetProperty(string propertyName, out IBindableProperty property)
        {
            if (bindableProperties == null)
            {
                property = null;
                return false;
            }

            return bindableProperties.TryGetValue(propertyName, out property);
        }

        public bool TryGetProperty<T>(string propertyName, out IBindableProperty<T> property)
        {
            if (!bindableProperties.TryGetValue(propertyName, out var tempProperty))
            {
                property = null;
                return false;
            }

            property = tempProperty as IBindableProperty<T>;
            return property != null;
        }

        public IBindableProperty GetProperty(string propertyName)
        {
            if (!bindableProperties.TryGetValue(propertyName, out var property))
            {
                return null;
            }

            return property;
        }

        public IBindableProperty<T> GetProperty<T>(string propertyName)
        {
            if (!bindableProperties.TryGetValue(propertyName, out var property))
            {
                return null;
            }

            return property as IBindableProperty<T>;
        }

        public void RegisterProperty(string propertyName, IBindableProperty property)
        {
            bindableProperties.Add(propertyName, property);
        }

        public void RegisterProperty<T>(string propertyName, Func<T> getter = null, Action<T> setter = null)
        {
            bindableProperties.Add(propertyName, new BindableProperty<T>(getter, setter));
        }

        public void RegisterProperty<T>(string propertyName, RefFunc<T> getter)
        {
            bindableProperties.Add(propertyName, new BindableProperty<T>(() => getter(), v => getter() = v));
        }

        public void UnregisterProperty(string propertyName)
        {
            if (!bindableProperties.TryGetValue(propertyName, out var property))
            {
                return;
            }

            property.Dispose();
            bindableProperties.Remove(propertyName);
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
            bindableProperties.Clear();
        }
    }
}