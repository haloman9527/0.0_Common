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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    public interface IListField
    {
        void RemoveElementAt(int _index);
    }

    public class ListField<L, E> : BaseField<L>, INotifyValueChanged<L>, IListField where L : IList<E>
    {
        public Foldout foldout;
        IntegerField lengthField;

        Type fieldType;
        Type elementType;

        public ListField(string _label, L _list, VisualElement _inputElement) : base(_label, _inputElement)
        {
            foldout = new Foldout();
            //foldout.SetValueWithoutNotify(EditorGUIExtension.GetFoldoutBool(_list.GetHashCode().ToString(), false));
            //foldout.RegisterValueChangedCallback(e =>
            //{
            //    EditorGUIExtension.SetFoldoutBool(_list.GetHashCode().ToString(), e.newValue);
            //});
            foldout.text = _label;
            Add(foldout);

            value = _list;
            fieldType = value.GetType();
            elementType = typeof(E);

            Reload();
        }

        public override void SetValueWithoutNotify(L newValue)
        {
            value = newValue;
            fieldType = value.GetType();
            Reload();
        }

        void Reload()
        {
            foldout.Clear();
            lengthField = new IntegerField("Count");
            lengthField.SetValueWithoutNotify(value.Count);
            lengthField.RegisterCallback<KeyDownEvent>(ChangeCount);
            foldout.Add(lengthField);
            for (int i = 0; i < value.Count; i++)
            {
                int j = i;
                foldout.Add(UIElementsFactory.CreateField("element " + j, typeof(E), value[j], _newValue =>
                {
                    value[j] = (E)_newValue;
                }));
            }
        }

        void ChangeCount(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return)
                SetCount(lengthField.value);
        }

        public void RemoveElement(E _element)
        {
            value.Remove(_element);
            Reload();
        }

        public void RemoveElementAt(int _index)
        {
            value.RemoveAt(_index);
            Reload();
        }

        void SetCount(int _newCount)
        {
            _newCount = Mathf.Max(0, _newCount);
            if (_newCount != value.Count)
            {
                int currentCount = value.Count;
                if (_newCount > currentCount)
                {
                    if (fieldType.IsArray)
                    {
                        int num3 = -1;
                        for (int i = 0; i < _newCount; i++)
                        {
                            if (i < value.Count)
                                num3 = i;
                            if (num3 == -1)
                                break;
                        }
                    }
                    else
                    {

                        if (!typeof(UnityObject).IsAssignableFrom(elementType))
                        {
                            Type type = value.Count > 0 ? value[value.Count - 1].GetType() : elementType;
                            for (int i = currentCount; i < _newCount; i++)
                                value.Add((E)Activator.CreateInstance(type, true));
                        }
                        else
                        {
                            for (int i = currentCount; i < _newCount; i++)
                                value.Add(default);
                        }
                    }
                }
                else
                {
                    if (!fieldType.IsArray)
                    {
                        while (value.Count > _newCount)
                        {
                            value.RemoveAt(value.Count - 1);
                        }
                    }
                }
            }
            Reload();
        }
    }
}
#endif