using System;
using System.Collections.Generic;
using System.Reflection;

namespace CZToolKit.Core.Editors
{
    public static class ObjectDrawerUtility
    {
        private static void BuildObjectDrawers()
        {
            if (ObjectDrawerUtility.mapBuilt)
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
                            CustomObjectDrawerAttribute[] array;
                            if (typeof(ObjectDrawer).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract && (array = (type.GetCustomAttributes(typeof(CustomObjectDrawerAttribute), false) as CustomObjectDrawerAttribute[])).Length > 0)
                            {
                                ObjectDrawerUtility.objectDrawerTypeMap.Add(array[0].Type, type);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            ObjectDrawerUtility.mapBuilt = true;
        }

        private static bool ObjectDrawerForType(Type type, ref ObjectDrawer objectDrawer, ref Type objectDrawerType, int hash)
        {
            ObjectDrawerUtility.BuildObjectDrawers();
            if (!ObjectDrawerUtility.objectDrawerTypeMap.ContainsKey(type))
            {
                return false;
            }
            objectDrawerType = ObjectDrawerUtility.objectDrawerTypeMap[type];
            if (ObjectDrawerUtility.objectDrawerMap.ContainsKey(hash))
            {
                objectDrawer = ObjectDrawerUtility.objectDrawerMap[hash];
            }
            return true;
        }

        public static ObjectDrawer GetObjectDrawer(FieldInfo _fieldInfo)
        {
            ObjectDrawer objectDrawer = null;
            Type type = null;
            if (!ObjectDrawerUtility.ObjectDrawerForType(_fieldInfo.FieldType, ref objectDrawer, ref type, _fieldInfo.GetHashCode()))
                return null;
            if (objectDrawer == null)
            {
                objectDrawer = (Activator.CreateInstance(type) as ObjectDrawer);
                ObjectDrawerUtility.objectDrawerMap.Add(_fieldInfo.GetHashCode(), objectDrawer);
            }
            objectDrawer.FieldInfo = _fieldInfo;
            return objectDrawer;
        }

        public static ObjectDrawer GetObjectDrawer(ObjectDrawerAttribute attribute)
        {
            ObjectDrawer objectDrawer = null;
            Type type = null;
            if (!ObjectDrawerUtility.ObjectDrawerForType(attribute.GetType(), ref objectDrawer, ref type, attribute.GetHashCode()))
                return null;
            if (objectDrawer != null)
                return objectDrawer;
            objectDrawer = (Activator.CreateInstance(type) as ObjectDrawer);
            objectDrawer.Attribute = attribute;
            ObjectDrawerUtility.objectDrawerMap.Add(attribute.GetHashCode(), objectDrawer);
            return objectDrawer;
        }

        private static Dictionary<Type, Type> objectDrawerTypeMap = new Dictionary<Type, Type>();

        private static Dictionary<int, ObjectDrawer> objectDrawerMap = new Dictionary<int, ObjectDrawer>();

        private static bool mapBuilt = false;
    }
}
