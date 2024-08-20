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

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CZToolKit
{
    public partial class ViewModel
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

        public bool TryGetProperty<T>(string propertyName, out IBindableProperty<T> property)
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

    public partial class ViewModel
    {
        public abstract class RefProperty
        {
        }

        public class RefProperty<V> : RefProperty
        {
            public delegate ref V RefFunc();

            private RefFunc getter;

            public ref V Value
            {
                get { return ref getter(); }
            }

            public RefProperty(RefFunc getter)
            {
                this.getter = getter;
            }
        }
    }

    public partial class ViewModel : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Dictionary<string, RefProperty> properties = new Dictionary<string, RefProperty>();

        public Dictionary<string, RefProperty> Properties => properties;
        
        protected virtual void OnPropertyChanging(string propertyName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool SetField<T>(string propertyName, T value)
        {
            if (!properties.TryGetValue(propertyName, out var p) || !(p is RefProperty<T> property))
            {
                return false;
            }

            if (EqualityComparer<T>.Default.Equals(property.Value, value)) return false;
            
            OnPropertyChanging(propertyName);
            property.Value = value;
            OnPropertyChanged(propertyName);
            
            return true;
        }

        protected T GetField<T>(string propertyName)
        {
            if (properties.TryGetValue(propertyName, out var p) && p is RefProperty<T> property)
            {
                return property.Value;
            }

            return default;
        }

        public void NotifyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
    }
}