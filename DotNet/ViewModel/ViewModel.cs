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

namespace Moyo
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Events<string> Events { get; } = new Events<string>();

        /// <summary>
        /// 只在属性中调用
        /// </summary>
        /// <param name="field"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected ref T GetFieldValue<T>(ref T field)
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
        protected bool SetFieldValue<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            var oldValue = field;
            field = value;
            Events.Publish(propertyName, oldValue, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
        }

        public void RegisterValueChanged<T>(string name, Action<T, T> valueChangedCallback)
        {
            Events.Subscribe(name, valueChangedCallback);
        }

        public void UnregisterValueChanged<T>(string name, Action<T, T> valueChangedCallback)
        {
            Events.Unsubscribe(name, valueChangedCallback);
        }

        public void UnregisterAllValueChanged(string name)
        {
            Events.Remove(name);
        }
    }
}