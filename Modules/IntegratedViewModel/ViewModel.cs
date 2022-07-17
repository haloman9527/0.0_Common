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

namespace CZToolKit.Core.ViewModel
{
    public class ViewModel : IViewModel<string, IBindableProperty>, IReadOnlyViewModel<string, IBindableProperty>
    {
        [NonSerialized]
        Dictionary<string, IBindableProperty> internalBindableProperties;
        Dictionary<string, IBindableProperty> InternalBindableProperties
        {
            get
            {
                if (internalBindableProperties == null)
                    internalBindableProperties = new Dictionary<string, IBindableProperty>();
                return internalBindableProperties;
            }
        }

        public IEnumerable<string> Keys
        {
            get { return InternalBindableProperties.Keys; }
        }

        public IEnumerable<IBindableProperty> Values
        {
            get { return InternalBindableProperties.Values; }
        }

        public int Count
        {
            get { return InternalBindableProperties.Count; }
        }

        public IBindableProperty this[string propertyName]
        {
            get { return InternalBindableProperties[propertyName]; }
            set { InternalBindableProperties[propertyName] = value; }
        }

        public IEnumerator<KeyValuePair<string, IBindableProperty>> GetEnumerator()
        {
            return InternalBindableProperties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InternalBindableProperties.GetEnumerator();
        }

        public bool ContainsKey(string key)
        {
            return InternalBindableProperties.ContainsKey(key);
        }

        public bool TryGetValue(string key, out IBindableProperty value)
        {
            return InternalBindableProperties.TryGetValue(key, out value);
        }

        public T GetPropertyValue<T>(string propertyName)
        {
            return this[propertyName].AsBindableProperty<T>().Value;
        }

        public void SetPropertyValue<T>(string propertyName, T value)
        {
            this[propertyName].AsBindableProperty<T>().Value = value;
        }

        public void BindingProperty<T>(string propertyName, Action<T> onValueChangedCallback)
        {
            this[propertyName].AsBindableProperty<T>().RegisterValueChangedEvent(onValueChangedCallback);
        }

        public void UnBindingProperty<T>(string propertyName, Action<T> onValueChangedCallback)
        {
            this[propertyName].AsBindableProperty<T>().UnregisterValueChangedEvent(onValueChangedCallback);
        }
    }
}