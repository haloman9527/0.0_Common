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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    public class SimpleObjectEditor
    {
        static Dictionary<Type, Type> ObjectEditorTypeCache;

        static SimpleObjectEditor()
        {
            BuildCache();
        }

        static void BuildCache()
        {
            ObjectEditorTypeCache = new Dictionary<Type, Type>();

            foreach (var type in TypeCache.GetTypesDerivedFrom<SimpleObjectEditor>())
            {
                foreach (var att in Utility_Attribute.GetTypeAttributes(type, true))
                {
                    if (att is SimpleObjectEditorAttribute sAtt)
                        ObjectEditorTypeCache[sAtt.targetType] = type;
                }
            }
        }

        static Type GetEditorType(Type _objectType)
        {
            if (ObjectEditorTypeCache.TryGetValue(_objectType, out Type editorType))
            {
                Debug.Log(_objectType);
                Debug.Log(editorType);
                return editorType;
            }
            if (_objectType.BaseType != null)
                return GetEditorType(_objectType.BaseType);
            else
                return typeof(SimpleObjectEditor);
        }

        static SimpleObjectEditor InternalCreateEditor(object _targetObject)
        {
            if (_targetObject == null) return null;

            return Activator.CreateInstance(GetEditorType(_targetObject.GetType()), true) as SimpleObjectEditor;
        }

        public static SimpleObjectEditor CreateEditor(object _targetObject)
        {
            SimpleObjectEditor objectEditor = InternalCreateEditor(_targetObject);
            if (objectEditor == null) return null;

            objectEditor.Initialize(_targetObject);
            return objectEditor;
        }

        protected SimpleObjectEditor() { }

        public object Target { get; private set; }
        protected IReadOnlyList<FieldInfo> Fields { get; private set; }

        void Initialize(object _target)
        {
            Target = _target;
            Fields = Utility_Reflection.GetFieldInfos(Target.GetType()).Where(field => EditorGUILayoutExtension.CanDraw(field)).ToList();
        }

        public virtual void OnGUI(Rect _position)
        {
            GUI.Label(_position, Target.GetType().Name);
        }

        public virtual float GetHeight()
        {
            return 20;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public class SimpleObjectEditorAttribute : Attribute
    {
        public Type targetType;

        public SimpleObjectEditorAttribute(Type _targetType) { targetType = _targetType; }
    }
}
