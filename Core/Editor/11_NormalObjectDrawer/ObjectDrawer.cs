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
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    public class ObjectDrawer
    {
        static Dictionary<Type, Type> ObjectEditorTypeCache;

        static ObjectDrawer()
        {
            BuildCache();
        }

        static void BuildCache()
        {
            ObjectEditorTypeCache = new Dictionary<Type, Type>();

            foreach (var type in TypeCache.GetTypesDerivedFrom<ObjectDrawer>())
            {
                foreach (var att in Util_Attribute.GetTypeAttributes(type, false))
                {
                    if (att is CustomObjectDrawerAttribute sAtt)
                        ObjectEditorTypeCache[sAtt.targetType] = type;
                }
            }
        }

        public static bool HasCustomEditor()
        {
            return false;
        }

        public static Type GetEditorType(Type _objectType)
        {
            if (ObjectEditorTypeCache.TryGetValue(_objectType, out Type editorType))
                return editorType;
            if (_objectType.BaseType != null)
                return GetEditorType(_objectType.BaseType);
            else
                return typeof(ObjectDrawer);
        }

        static ObjectDrawer InternalCreateEditor(object _targetObject)
        {
            if (_targetObject == null) return null;

            return Activator.CreateInstance(GetEditorType(_targetObject.GetType()), true) as ObjectDrawer;
        }

        public static ObjectDrawer CreateEditor(object _targetObject)
        {
            ObjectDrawer objectEditor = InternalCreateEditor(_targetObject);
            if (objectEditor == null) return null;

            objectEditor.Initialize(_targetObject);
            return objectEditor;
        }

        protected ObjectDrawer() { }

        public FieldInfo FieldInfo { get; set; }
        public FieldAttribute Attribute { get; set; }
        public object Target { get; set; }
        protected IReadOnlyList<FieldInfo> Fields { get; private set; }

        void Initialize(object _target)
        {
            Target = _target;
            Fields = Util_Reflection.GetFieldInfos(Target.GetType()).Where(field => EditorGUILayoutExtension.CanDraw(field)).ToList();
        }

        void Initialize(object _target, FieldInfo _field)
        {
            Target = _target;
            Fields = Util_Reflection.GetFieldInfos(Target.GetType()).Where(field => EditorGUILayoutExtension.CanDraw(field)).ToList();
        }

        public virtual void OnGUI(Rect _position, GUIContent _label)
        {
            GUI.Label(_position, _label);
        }

        public virtual float GetHeight()
        {
            return 20;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public class CustomObjectDrawerAttribute : Attribute
    {
        public Type targetType;

        public CustomObjectDrawerAttribute(Type _targetType) { targetType = _targetType; }
    }
}
#endif