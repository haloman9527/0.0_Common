//#region 注 释
///***
// *
// *  Title:
// *  
// *  Description:
// *  
// *  Date:
// *  Version:
// *  Writer: 半只龙虾人
// *  Github: https://github.com/HalfLobsterMan
// *  Blog: https://www.crosshair.top/
// *
// */
//#endregion
//using UnityEngine;
//using System;
//using System.Reflection;
//using System.Collections.Generic;

//using UnityObject = UnityEngine.Object;
//using UnityEditor;
//using System.Collections;

//namespace CZToolKit.Core.Editors
//{
//    public class SerializedObjectS
//    {
//        readonly object target;
//        List<SerializedPropertyS> childrens;

//        public SerializedObjectS(object target)
//        {
//            this.target = target;
//        }

//        void InternalCreate()
//        {
//            Type type = target.GetType();
//            childrens = new List<SerializedPropertyS>();
//            if (!typeof(IList).IsAssignableFrom(type))
//            {
//                foreach (var fieldInfo in Util_Reflection.GetFieldInfos(type))
//                {
//                    if (fieldInfo.Name.StartsWith("<"))
//                        continue;
//                    // public 修饰符
//                    if (!fieldInfo.IsPublic)
//                    {
//                        // 如果不带有SerializeField特性
//                        if (!Util_Attribute.TryGetFieldAttribute<SerializeField>(fieldInfo, out var serializeField))
//                            continue;
//                        // 若带有NonSerialized特性
//                        if (Util_Attribute.TryGetFieldAttribute<NonSerializedAttribute>(fieldInfo, out var nonSerialized))
//                            continue;
//                        // 若带有HideInInspector特性
//                        if (Util_Attribute.TryGetFieldAttribute<HideInInspector>(fieldInfo, out var hideInInspector))
//                            continue;
//                    }
//                    childrens.Add(new SerializedPropertyS(fieldInfo, target));
//                }
//            }
//        }

//        public IEnumerable<SerializedPropertyS> GetIterator()
//        {
//            if (childrens == null)
//            {
//                InternalCreate();
//            }
//            foreach (var property in childrens)
//            {
//                yield return property;
//            }
//        }
//    }

//    public class PropertyTree
//    {
//        public static SerializedPropertyS Create(object target)
//        {
//            if (target == null)
//            {
//                return null;
//            }
//            Type type = target.GetType();
//            if (typeof(IList).IsAssignableFrom(type))
//            {
//                return null;
//            }
//            return new SerializedPropertyS(target);
//        }
//    }

//    public class SerializedPropertyS
//    {
//        FieldInfo fieldInfo;
//        object context;
//        object target;
//        List<SerializedPropertyS> childrens;

//        public bool isExpanded;
//        public readonly GUIContent niceName;

//        internal FieldInfo FieldInfo { get { return fieldInfo; } }
//        internal object Context { get { return context; } }
//        public bool HasChildren { get { return HasChildrenInternal(); } }
//        public string Name { get { return fieldInfo.Name; } }
//        public bool IsArray { get; }

//        public SerializedPropertyS(object target)
//        {
//            this.target = target;
//        }

//        internal SerializedPropertyS(FieldInfo fieldInfo, object context)
//        {
//            this.fieldInfo = fieldInfo;
//            this.context = context;
//            this.target = fieldInfo.GetValue(context);
//            this.niceName = GUIHelper.TextContent(ObjectNames.NicifyVariableName(fieldInfo.Name));
//            this.IsArray = typeof(IList).IsAssignableFrom(target.GetType());

//            if (!HasChildren)
//            {
//                childrens = new List<SerializedPropertyS>();
//                if (!typeof(UnityObject).IsAssignableFrom(fieldInfo.FieldType))
//                    fieldInfo.SetValue(context, EditorGUIExtension.CreateInstance(fieldInfo.FieldType));
//            }
//        }

//        void InternalCreate()
//        {
//            Type type = fieldInfo == null ? fieldInfo.FieldType : target.GetType();
//            childrens = new List<SerializedPropertyS>();
//            if (target == null)
//            {
//                target = EditorGUIExtension.CreateInstance(type);
//                fieldInfo.SetValue(context, target);
//            }

//            foreach (var fieldInfo in Util_Reflection.GetFieldInfos(type))
//            {
//                if (fieldInfo.Name.StartsWith("<"))
//                    continue;
//                // public 修饰符
//                if (!fieldInfo.IsPublic)
//                {
//                    // 如果不带有SerializeField特性
//                    if (!Util_Attribute.TryGetFieldAttribute<SerializeField>(fieldInfo, out var serializeField))
//                    {
//                        continue;
//                    }
//                    // 若带有NonSerialized特性
//                    if (Util_Attribute.TryGetFieldAttribute<NonSerializedAttribute>(fieldInfo, out var nonSerialized))
//                    {
//                        continue;
//                    }
//                    // 若带有HideInInspector特性
//                    if (Util_Attribute.TryGetFieldAttribute<HideInInspector>(fieldInfo, out var hideInInspector))
//                    {
//                        continue;
//                    }
//                }
//                childrens.Add(new SerializedPropertyS(fieldInfo, target));
//            }
//        }

//        public IEnumerable<SerializedPropertyS> GetIterator()
//        {
//            if (childrens == null)
//                InternalCreate();
//            foreach (var children in childrens)
//            {
//                yield return children;
//            }
//        }

//        bool HasChildrenInternal()
//        {
//            if (EditorGUIExtension.IsBasicType(fieldInfo.FieldType))
//            {
//                return false;
//            }
//            var type = fieldInfo.FieldType;
//            if (type.IsClass || (type.IsValueType && !type.IsPrimitive))
//            {
//                if (!typeof(Delegate).IsAssignableFrom(type) && typeof(object).IsAssignableFrom(type))
//                {
//                    return true;
//                }
//            }
//            return false;
//        }
//    }
//}
