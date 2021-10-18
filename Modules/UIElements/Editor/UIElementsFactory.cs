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
#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

using UnityObject = UnityEngine.Object;
using System.Collections;

namespace CZToolKit.Core.Editors
{
    public static class UIElementsFactory
    {
        static readonly MethodInfo CreateFieldMethod = typeof(UIElementsFactory).GetMethod("CreateFieldS", BindingFlags.Static | BindingFlags.NonPublic);
        static readonly Dictionary<Type, Type> fieldDrawersCache = new Dictionary<Type, Type>();
        static readonly Dictionary<Type, Func<Type, BindableElement>> fieldDrawerCreatorMap = new Dictionary<Type, Func<Type, BindableElement>>();
        public static IReadOnlyDictionary<Type, Func<Type, BindableElement>> FieldDrawerCreatorMap { get { return fieldDrawerCreatorMap; } }

        static UIElementsFactory()
        {
            AddDrawer(typeof(Enum), typeof(EnumField), realType => { return new EnumField(); });

            AddDrawer<bool, Toggle>(_ => { return new Toggle(); });
            AddDrawer<int, IntegerField>(_ => { return new IntegerField(); });
            AddDrawer<long, LongField>(_ => { return new LongField(); });
            AddDrawer<float, FloatField>(_ => { return new FloatField(); });
            AddDrawer<double, DoubleField>(_ => { return new DoubleField(); });
            AddDrawer<string, TextField>(_ => { return new TextField() { multiline = true }; });

            AddDrawer<LayerMask, LayerMaskField>(_ => { return new LayerMaskField(); });
            AddDrawer<Rect, RectField>(_ => { return new RectField(); });
            AddDrawer<Bounds, BoundsField>(_ => { return new BoundsField(); });
            AddDrawer<BoundsInt, BoundsIntField>(_ => { return new BoundsIntField(); });
            AddDrawer<Color, ColorField>(_ => { return new ColorField(); });
            AddDrawer<Vector2, Vector2Field>(_ => { return new Vector2Field(); });
            AddDrawer<Vector2Int, Vector2IntField>(_ => { return new Vector2IntField(); });
            AddDrawer<Vector3, Vector3Field>(_ => { return new Vector3Field(); });
            AddDrawer<Vector3Int, Vector3IntField>(_ => { return new Vector3IntField(); });
            AddDrawer<Vector4, Vector4Field>(_ => { return new Vector4Field(); });

            AddDrawer<AnimationCurve, CurveField>(_ => { return new CurveField(); });
            AddDrawer<Gradient, GradientField>(_ => { return new GradientField(); });
            AddDrawer<UnityObject, ObjectField>(realType => { return new ObjectField() { objectType = realType }; });
        }

        static void AddDrawer(Type _fieldType, Type _drawerType, Func<Type, BindableElement> _fieldDrawerCreator)
        {
            fieldDrawersCache[_fieldType] = _drawerType;
            fieldDrawerCreatorMap[_fieldType] = _fieldDrawerCreator;
        }

        static void AddDrawer<F, D>(Func<Type, BindableElement> _fieldDrawerCreator) where D : VisualElement, new()
        {
            Type fieldType = typeof(F);
            Type drawerType = typeof(D);

            fieldDrawersCache[fieldType] = drawerType;
            fieldDrawerCreatorMap[fieldType] = _fieldDrawerCreator;
        }

        static INotifyValueChanged<F> CreateFieldS<F>(string _label, F _value, Type _realFieldType, Action<object> _onValueChanged)
        {
            INotifyValueChanged<F> fieldDrawer = null;
            // 如果没有Drawer创建方法，返回空
            if (!fieldDrawerCreatorMap.TryGetValue(typeof(F), out Func<Type, BindableElement> drawerCreator))
                return null;

            // 创建Drawer，设置初始值，注册onValueChanged方法
            fieldDrawer = drawerCreator(_realFieldType) as INotifyValueChanged<F>;
            fieldDrawer.SetValueWithoutNotify(_value);
            fieldDrawer.RegisterValueChangedCallback((e) =>
            {
                _onValueChanged(e.newValue);
            });

            BaseField<F> tDrawer = fieldDrawer as BaseField<F>;
            tDrawer.label = _label;
            return fieldDrawer;
        }

        public static BindableElement CreateField<T>(string _label, Type _valueType, T _value, Action<object> _onValueChanged)
        {
            Type realValueType = _valueType;

            // 对字段类型进行修饰，UnityObject的子类型修饰为UnityObject
            // 枚举类型修饰为Enum
            if (!fieldDrawerCreatorMap.ContainsKey(_valueType))
            {
                if (typeof(UnityObject).IsAssignableFrom(_valueType))
                    _valueType = typeof(UnityObject);
                else if (typeof(Enum).IsAssignableFrom(_valueType) && !fieldDrawerCreatorMap.ContainsKey(_valueType))
                    _valueType = typeof(Enum);
            }

            // LayerMask需单独创建
            if (_value is LayerMask layerMask)
            {
                var layerField = new LayerMaskField(_label, layerMask.value);
                layerField.RegisterValueChangedCallback(e =>
                {
                    _onValueChanged(new LayerMask { value = e.newValue });
                });
                return layerField;
            }

            if (_value is IList list)
            {
                Type elementType = null;
                if (_valueType.IsArray)
                {
                    elementType = _valueType.GetElementType();
                }
                else
                {
                    Type type2 = _valueType;
                    while (!type2.IsGenericType)
                    {
                        type2 = type2.BaseType;
                    }
                    elementType = type2.GetGenericArguments()[0];
                }
                BindableElement bind = Activator.CreateInstance(typeof(ListField<,>).MakeGenericType(_valueType, elementType), _label, _value) as BindableElement;

                return bind;
            }

            BindableElement fieldDrawer = null;
            var createFieldSpecificMethod = CreateFieldMethod.MakeGenericMethod(_valueType);
            fieldDrawer = createFieldSpecificMethod.Invoke(null, new object[] { _label, _value, realValueType, _onValueChanged }) as BindableElement;

            return fieldDrawer;
        }
    }
}
#endif