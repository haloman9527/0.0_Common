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
 *  Blog: https://www.mindgear.net/
 *
 */

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;

namespace CZToolKit.VM
{
    public static class ViewModelFactory
    {
        private static bool s_Initialized;
        private static Dictionary<Type, Type> s_ViewModelTypeCache;

        static ViewModelFactory()
        {
        }

        public static void Init(bool force)
        {
            if (!force && s_Initialized)
                return;

            if (s_ViewModelTypeCache == null)
                s_ViewModelTypeCache = new Dictionary<Type, Type>();
            else
                s_ViewModelTypeCache.Clear();

            foreach (var type in Util_TypeCache.GetTypesWithAttribute<ViewModelAttribute>())
            {
                if (type.IsAbstract)
                    continue;
                var attribute = type.GetCustomAttribute<ViewModelAttribute>(true);
                s_ViewModelTypeCache[attribute.targetType] = type;
            }

            s_Initialized = true;
        }

        public static Type GetViewModelType(Type modelType)
        {
            var viewModelType = (Type)null;
            while (viewModelType == null)
            {
                s_ViewModelTypeCache.TryGetValue(modelType, out viewModelType);
                if (modelType.BaseType == null)
                    break;
                modelType = modelType.BaseType;
            }

            return viewModelType;
        }

        public static object CreateViewModel(object model)
        {
            return Activator.CreateInstance(GetViewModelType(model.GetType()), model);
        }
    }
}