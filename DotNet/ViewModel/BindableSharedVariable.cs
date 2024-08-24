// #region 注 释
//
// /***
//  *
//  *  Title:
//  *  
//  *  Description:
//  *  
//  *  Date:
//  *  Version:
//  *  Writer: 半只龙虾人
//  *  Github: https://github.com/haloman9527
//  *  Blog: https://www.haloman.net/
//  *
//  */
//
// #endregion
//
// using System;
//
// namespace CZToolKit
// {
//     public interface IValueProvider
//     {
//         object GetValue(long id);
//
//         void SetValue(long id, object value);
//     }
//
//     public interface ISharedVariable
//     {
//         IValueProvider ValueProvider { get; set; }
//
//         object BoxedValue { get; set; }
//     }
//
//     public interface ISharedVariable<T>
//     {
//         IValueProvider ValueProvider { get; set; }
//
//         long ID { get; set; }
//
//         T Value { get; set; }
//     }
//
//     [Serializable]
//     public class BindableSharedVariable<T> : IBindableProperty<T>, IBindableProperty, ISharedVariable, ISharedVariable<T>
//     {
//         private IValueProvider valueProvider;
//
//         private event Func<long> IDGetter;
//         private event Action<long> IDSetter;
//
//         public event ValueChangedEvent<long> onIDChanged;
//         public event ValueChangedEvent<T> onValueChanged;
//         public event ValueChangedEvent<object> onBoxedValueChanged;
//
//         public long ID
//         {
//             get
//             {
//                 if (IDGetter == null)
//                     throw new NotImplementedException("haven't get method");
//                 return IDGetter();
//             }
//             set
//             {
//                 if (IDSetter == null)
//                     throw new NotImplementedException("haven't set method");
//                 if (ID == value)
//                     return;
//                 var oldID = ID;
//                 IDSetter(value);
//                 onIDChanged?.Invoke(oldID, value);
//             }
//         }
//
//         public IValueProvider ValueProvider
//         {
//             get { return valueProvider; }
//             set { valueProvider = value; }
//         }
//         
//         public T Value
//         {
//             get => (T)BoxedValue;
//             set => BoxedValue = value;
//         }
//
//         public object BoxedValue
//         {
//             get => valueProvider.GetValue(IDGetter());
//             set
//             {
//                 var oldValue = valueProvider.GetValue(IDGetter());
//                 if (Equals(oldValue, value))
//                     return;
//                 valueProvider.SetValue(IDGetter(), value);
//                 NotifyValueChanged_Internal((T)oldValue, (T)value);
//             }
//         }
//
//         public Type ValueType => typeof(T);
//
//         public BindableSharedVariable(Func<long> IDGetter)
//         {
//             this.IDGetter = IDGetter;
//         }
//
//         private void NotifyValueChanged_Internal(T oldValue, T newValue)
//         {
//             onValueChanged?.Invoke(oldValue, newValue);
//             onBoxedValueChanged?.Invoke(oldValue, newValue);
//         }
//
//         public IBindableProperty<TOut> AsBindableSharedVariable<TOut>()
//         {
//             return this as BindableSharedVariable<TOut>;
//         }
//
//         public void RegisterValueChangedEvent(ValueChangedEvent<T> onValueChanged)
//         {
//             this.onValueChanged += onValueChanged;
//         }
//
//         public void UnregisterValueChangedEvent(ValueChangedEvent<T> onValueChanged)
//         {
//             this.onValueChanged -= onValueChanged;
//         }
//
//         public void SetValueWithNotify(T value)
//         {
//             valueProvider.SetValue(ID, value);
//             NotifyValueChanged();
//         }
//
//         public void SetValueWithoutNotify(T value)
//         {
//             valueProvider.SetValue(ID, value);
//         }
//
//         public void SetValueWithNotify(object value)
//         {
//             SetValueWithoutNotify((T)value);
//             NotifyValueChanged();
//         }
//
//         public void SetValueWithoutNotify(object value)
//         {
//             SetValueWithoutNotify((T)value);
//         }
//
//         public void ClearValueChangedEvent()
//         {
//             while (this.onValueChanged != null)
//                 this.onValueChanged -= this.onValueChanged;
//         }
//
//         public void NotifyValueChanged()
//         {
//             NotifyValueChanged_Internal(Value, Value);
//         }
//
//         public override string ToString()
//         {
//             return (Value != null ? Value.ToString() : "null");
//         }
//
//         public void Dispose()
//         {
//             
//         }
//     }
// }