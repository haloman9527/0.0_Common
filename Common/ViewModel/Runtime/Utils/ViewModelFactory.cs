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
using CZToolKit;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CZToolKit.VM
{
    public static class ViewModelFactory
    {
        static Dictionary<Type, Type> ViewModelTypeCache;

        static ViewModelFactory()
        {
            ViewModelTypeCache = new Dictionary<Type, Type>();
            foreach (var type in Util_TypeCache.GetTypesWithAttribute<ViewModelAttribute>())
            {
                if (type.IsAbstract) continue;
                var attribute = type.GetCustomAttribute<ViewModelAttribute>(true);
                ViewModelTypeCache[attribute.targetType] = type;
            }
        }

        public static Type GetViewModelType(Type modelType)
        {
            var viewModelType = (Type)null;
            while (viewModelType == null)
            {
                ViewModelTypeCache.TryGetValue(modelType, out viewModelType);
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
