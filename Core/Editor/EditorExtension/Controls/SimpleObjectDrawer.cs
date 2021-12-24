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
    public class SimpleObjectDrawer
    {
        static Dictionary<Type, Type> ObjectEditorTypeCache;

        static SimpleObjectDrawer()
        {
            BuildCache();
        }

        static void BuildCache()
        {
            ObjectEditorTypeCache = new Dictionary<Type, Type>();

            foreach (var type in TypeCache.GetTypesDerivedFrom<SimpleObjectDrawer>())
            {
                foreach (var att in Util_Attribute.GetTypeAttributes(type, false))
                {
                    if (att is CustomSimpleObjectDrawerAttribute sAtt)
                        ObjectEditorTypeCache[sAtt.targetType] = type;
                }
            }
        }

        public static bool HasCustomDrawer(Type objectType)
        {
            return GetEditorType(objectType) != typeof(SimpleObjectDrawer);
        }

        public static Type GetEditorType(Type objectType)
        {
            if (ObjectEditorTypeCache.TryGetValue(objectType, out Type editorType))
                return editorType;
            if (objectType.BaseType != null)
                return GetEditorType(objectType.BaseType);
            else
                return typeof(SimpleObjectDrawer);
        }

        static SimpleObjectDrawer InternalCreateEditor(object target)
        {
            if (target == null) return null;

            return Activator.CreateInstance(GetEditorType(target.GetType()), target) as SimpleObjectDrawer;
        }

        public static SimpleObjectDrawer CreateEditor(object target)
        {
            SimpleObjectDrawer objectEditor = InternalCreateEditor(target);
            if (objectEditor == null) return null;
            return objectEditor;
        }

        public SimpleObjectDrawer(object target)
        {
            Initialize(target);
        }

        public object Target { get; set; }
        protected IReadOnlyList<FieldInfo> Fields { get; private set; }

        void Initialize(object target)
        {
            Target = target;
            Fields = Util_Reflection.GetFieldInfos(Target.GetType()).Where(field => EditorGUILayoutExtension.CanDraw(field)).ToList();
        }

        public virtual void OnGUI(Rect position, GUIContent label)
        {
            GUI.Label(position, label);
        }

        public virtual float GetHeight(GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public class CustomSimpleObjectDrawerAttribute : Attribute
    {
        public readonly Type targetType;

        public CustomSimpleObjectDrawerAttribute(Type targetType) { this.targetType = targetType; }
    }
}
