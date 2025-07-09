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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Atom
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public struct ValueChangedArg<T>
        {
            public T oldValue;
            public T newValue;
        }

        private EventStation<string> m_ValueChangedEvents = new EventStation<string>();
        
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 只在属性中调用
        /// </summary>
        /// <param name="field"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetFieldValue<T>(ref T field)
        {
            return ref field;
        }

        /// <summary>
        /// 只在属性中调用
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool SetFieldValue<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            var oldValue = field;
            field = value;
            m_ValueChangedEvents.Publish(propertyName, new ValueChangedArg<T>() { oldValue = oldValue, newValue = value });
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            OnPropertyChanged(propertyName);
            return true;
        }

        public void RegisterValueChanged<T>(string name, Action<ValueChangedArg<T>> valueChangedCallback)
        {
            m_ValueChangedEvents.Subscribe(name, valueChangedCallback);
        }

        public void UnregisterValueChanged<T>(string name, Action<ValueChangedArg<T>> valueChangedCallback)
        {
            m_ValueChangedEvents.Unsubscribe(name, valueChangedCallback);
        }

        public void UnregisterAllValueChanged(string name)
        {
            m_ValueChangedEvents.UnRegisterEvent(name);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
        }
    }
}