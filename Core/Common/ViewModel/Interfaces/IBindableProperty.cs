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

namespace CZToolKit.VM
{
    public interface IBindableProperty
    {
        event ValueChangedEvent<object> onBoxedValueChanged;

        object ValueBoxed { get; set; }
        Type ValueType { get; }

        void SetValueWithNotify(object value);
        void SetValueWithoutNotify(object value);
        void NotifyValueChanged();
        void ClearValueChangedEvent();
    }

    public interface IBindableProperty<T> : IBindableProperty
    {
        event ValueChangedEvent<T> onValueChanged;

        T Value { get; set; }

        void SetValueWithNotify(T value);
        void SetValueWithoutNotify(T value);
        void RegisterValueChangedEvent(ValueChangedEvent<T> onValueChanged);
        void UnregisterValueChangedEvent(ValueChangedEvent<T> onValueChanged);
    }
}
