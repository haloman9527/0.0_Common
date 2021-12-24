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

        public static Type GetEditorType(Type objectType)
        {
            if (ObjectEditorTypeCache.TryGetValue(objectType, out Type editorType))
                return editorType;
            if (objectType.BaseType != null)
                return GetEditorType(objectType.BaseType);
            else
                return typeof(ObjectDrawer);
        }

        static ObjectDrawer InternalCreateEditor(object target)
        {
            if (target == null) return null;

            return Activator.CreateInstance(GetEditorType(target.GetType()), true) as ObjectDrawer;
        }

        public static ObjectDrawer CreateEditor(object target)
        {
            ObjectDrawer objectEditor = InternalCreateEditor(target);
            if (objectEditor == null) return null;

            objectEditor.Initialize(target);
            return objectEditor;
        }

        protected ObjectDrawer() { }

        public FieldInfo FieldInfo { get; set; }
        public FieldAttribute Attribute { get; set; }
        public object Target { get; set; }
        protected IReadOnlyList<FieldInfo> Fields { get; private set; }

        void Initialize(object target)
        {
            Target = target;
            Fields = Util_Reflection.GetFieldInfos(Target.GetType()).Where(field => EditorGUILayoutExtension.CanDraw(field)).ToList();
        }

        void Initialize(object target, FieldInfo field)
        {
            Target = target;
            Fields = Util_Reflection.GetFieldInfos(Target.GetType()).Where(tmpField => EditorGUILayoutExtension.CanDraw(tmpField)).ToList();
        }

        public virtual void OnGUI(Rect position, GUIContent label)
        {
            GUI.Label(position, label);
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