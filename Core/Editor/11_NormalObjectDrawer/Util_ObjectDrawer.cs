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
using System.Reflection;

namespace CZToolKit.Core.Editors
{
    public static class Util_ObjectDrawer
    {
        private static void BuildObjectDrawers()
        {
            if (Util_ObjectDrawer.mapBuilt)
            {
                return;
            }
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly != null)
                {
                    try
                    {
                        foreach (Type type in assembly.GetExportedTypes())
                        {
                            CustomFieldDrawerAttribute[] array;
                            if (typeof(ObjectDrawer).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract && (array = (type.GetCustomAttributes(typeof(CustomFieldDrawerAttribute), false) as CustomFieldDrawerAttribute[])).Length > 0)
                            {
                                Util_ObjectDrawer.objectDrawerTypeMap.Add(array[0].type, type);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            Util_ObjectDrawer.mapBuilt = true;
        }

        private static bool ObjectDrawerForType(Type fieldType, ref ObjectDrawer fieldDrawer, ref Type fieldDrawerType, int hash)
        {
            Util_ObjectDrawer.BuildObjectDrawers();
            if (!Util_ObjectDrawer.objectDrawerTypeMap.ContainsKey(fieldType))
            {
                return false;
            }
            fieldDrawerType = Util_ObjectDrawer.objectDrawerTypeMap[fieldType];
            if (Util_ObjectDrawer.objectDrawerMap.ContainsKey(hash))
            {
                fieldDrawer = Util_ObjectDrawer.objectDrawerMap[hash];
            }
            return true;
        }

        public static ObjectDrawer GetObjectDrawer(FieldInfo fieldInfo)
        {
            ObjectDrawer objectDrawer = null;
            Type type = null;
            if (!Util_ObjectDrawer.ObjectDrawerForType(fieldInfo.FieldType, ref objectDrawer, ref type, fieldInfo.GetHashCode()))
                return null;
            if (objectDrawer == null)
            {
                objectDrawer = (Activator.CreateInstance(type) as ObjectDrawer);
                Util_ObjectDrawer.objectDrawerMap.Add(fieldInfo.GetHashCode(), objectDrawer);
            }
            objectDrawer.FieldInfo = fieldInfo;
            return objectDrawer;
        }

        public static ObjectDrawer GetObjectDrawer(FieldAttribute attribute)
        {
            ObjectDrawer objectDrawer = null;
            Type type = null;
            if (!Util_ObjectDrawer.ObjectDrawerForType(attribute.GetType(), ref objectDrawer, ref type, attribute.GetHashCode()))
                return null;
            if (objectDrawer != null)
                return objectDrawer;
            objectDrawer = (Activator.CreateInstance(type) as ObjectDrawer);
            objectDrawer.Attribute = attribute;
            Util_ObjectDrawer.objectDrawerMap.Add(attribute.GetHashCode(), objectDrawer);
            return objectDrawer;
        }

        private static Dictionary<Type, Type> objectDrawerTypeMap = new Dictionary<Type, Type>();

        private static Dictionary<int, ObjectDrawer> objectDrawerMap = new Dictionary<int, ObjectDrawer>();

        private static bool mapBuilt = false;
    }
}
#endif