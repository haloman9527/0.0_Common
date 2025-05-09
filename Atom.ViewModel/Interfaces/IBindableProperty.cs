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

namespace Atom
{
    public interface IBindableProperty
    {
        event Action<object, object> BoxedValueChanged;

        object BoxedValue { get; set; }
        Type ValueType { get; }
        
        bool SetValue(object value);
        void SetValueWithoutNotify(object value);
        void NotifyValueChanged();
        void ClearValueChangedEvent();
    }

    public interface IBindableProperty<T>
    {
        event Action<T, T> ValueChanged;

        T Value { get; set; }

        bool SetValue(T value);
        void SetValueWithoutNotify(T value);
        void RegisterValueChangedEvent(Action<T, T> onValueChanged);
        void UnregisterValueChangedEvent(Action<T, T> onValueChanged);
    }
}
