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
    public interface IViewModel { }

    public interface IViewModel<TKey, TValue> : IViewModel, IReadOnlyViewModel<TKey, TValue>
    {
        T GetPropertyValue<T>(string propertyName);

        void SetPropertyValue<T>(string propertyName, T value);

        void BindingProperty<T>(string propertyName, Action<T> onValueChangedCallback);

        void UnBindingProperty<T>(string propertyName, Action<T> onValueChangedCallback);
    }
}
