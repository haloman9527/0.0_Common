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

namespace CZToolKit.Core.ViewModel
{
    public interface IBindableProperty
    {
        event Action<object> onBoxedValueChanged;

        object ValueBoxed { get; set; }
        Type ValueType { get; }

        void SetValueWithNotify(object value);
        void SetValueWithoutNotify(object value);
        IBindableProperty<T> AsBindableProperty<T>();
        void RegisterValueChangedEvent<T>(Action<T> onValueChanged);
        void UnregisterValueChangedEvent<T>(Action<T> onValueChanged);
        void NotifyValueChanged();
        void ClearValueChagnedEvent();
    }

    public interface IBindableProperty<T>
    {
        event Action<T> onValueChanged;

        T Value { get; set; }

        void SetGetterSetter(Func<T> getter, Action<T> setter);
        void RegisterValueChangedEvent(Action<T> onValueChanged);
        void UnregisterValueChangedEvent(Action<T> onValueChanged);
        void SetValueWithNotify(object value);
        void SetValueWithoutNotify(T value);
        void NotifyValueChanged();
        void ClearValueChagnedEvent();
    }
}
