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

namespace CZToolKit
{
    public interface IBindableProperty : IDisposable
    {
        event ValueChangedEvent<object> onBoxedValueChanged;

        object BoxedValue { get; set; }
        Type ValueType { get; }
        
        bool SetValue(object value);
        void SetValueWithoutNotify(object value);
        void NotifyValueChanged();
        void ClearValueChangedEvent();
    }

    public interface IBindableProperty<T> : IDisposable
    {
        event ValueChangedEvent<T> onValueChanged;

        T Value { get; set; }

        bool SetValue(T value);
        void SetValueWithoutNotify(T value);
        void RegisterValueChangedEvent(ValueChangedEvent<T> onValueChanged);
        void UnregisterValueChangedEvent(ValueChangedEvent<T> onValueChanged);
    }
}
