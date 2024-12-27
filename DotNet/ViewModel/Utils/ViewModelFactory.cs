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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Moyo
{
    public static class ViewModelFactory
    {
        private static bool s_Initialized;
        private static Dictionary<Type, Type> s_ViewModelTypes;

        static ViewModelFactory()
        {
            Init(true);
        }

        public static void Init(bool force)
        {
            if (!force && s_Initialized)
                return;

            if (s_ViewModelTypes == null)
            {
                s_ViewModelTypes = new Dictionary<Type, Type>();
            }
            else
            {
                s_ViewModelTypes.Clear();
            }

            foreach (var type in Util_TypeCache.GetTypesWithAttribute<ViewModelAttribute>())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (type.IsGenericType)
                {
                    continue;
                }

                var attribute = type.GetCustomAttribute<ViewModelAttribute>(true);
                s_ViewModelTypes.Add(attribute.modelType, type);
            }

            s_Initialized = true;
        }

        public static Type GetViewModelType(Type modelType)
        {
            if (!s_ViewModelTypes.TryGetValue(modelType, out var viewModelType))
            {
                var type = modelType;
                do
                {
                    type = type.BaseType;
                } while (type != null && !s_ViewModelTypes.TryGetValue(type, out viewModelType));

                s_ViewModelTypes[modelType] = viewModelType;
            }

            return viewModelType;
        }

        public static object CreateViewModel(object model)
        {
            var modelType = model.GetType();
            
            var viewModelType = GetViewModelType(modelType);
            if (viewModelType != null)
            {
                var viewModel = Activator.CreateInstance(viewModelType, model);
                return viewModel;
            }
            
            return null;
        }

        public static object CreateViewModel(Type modelType)
        {
            var viewModelType = GetViewModelType(modelType);
            if (viewModelType != null)
            {
                var viewModel = Activator.CreateInstance(viewModelType);
                return viewModel;
            }
            
            return null;
        }
    }
}