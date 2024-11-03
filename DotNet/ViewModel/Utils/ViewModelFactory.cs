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

namespace CZToolKit
{
    public static class ViewModelFactory
    {
        private static bool s_Initialized;
        private static Dictionary<Type, Type> s_ViewModelTypes;
        private static Dictionary<Type, IViewModelFactory> s_ViewModelFactories;

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

            if (s_ViewModelFactories == null)
            {
                s_ViewModelFactories = new Dictionary<Type, IViewModelFactory>();
            }
            else
            {
                s_ViewModelFactories.Clear();
            }
            
            foreach (var type in Util_TypeCache.GetTypesWithAttribute<ViewModelAttribute>())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                var attribute = type.GetCustomAttribute<ViewModelAttribute>(true);
                s_ViewModelFactories[attribute.modelType] = (IViewModelFactory)Activator.CreateInstance(type);
            }

            s_Initialized = true;
        }

        public static Type GetViewModelType(Type modelType)
        {
            var viewModelType = (Type)null;
            while (modelType != null && !s_ViewModelTypes.TryGetValue(modelType, out viewModelType))
            {
                modelType = modelType.BaseType;
            }

            return viewModelType;
        }

        public static IViewModelFactory GetViewModelFactory(Type modelType)
        {
            var factory = (IViewModelFactory)null;
            while (modelType != null && !s_ViewModelFactories.TryGetValue(modelType, out factory))
            {
                modelType = modelType.BaseType;
            }

            return factory;
        }

        public static object CreateViewModel(object model)
        {
            var modelType = model.GetType();

            var factory = GetViewModelFactory(modelType);
            if (factory != null)
            {
                var viewModel = factory.CreateViewModel(model);
                (viewModel as IViewModel)?.SetUp(model);
                return viewModel;
            }
            
            var viewModelType = GetViewModelType(modelType);
            if (viewModelType != null)
            {
                var viewModel = Activator.CreateInstance(viewModelType, model);
                (viewModel as IViewModel)?.SetUp(model);
                return viewModel;
            }
            
            return null;
        }

        public static object CreateViewModel(object model, params object[] parameters)
        {
            var modelType = model.GetType();
            
            var viewModelType = GetViewModelType(modelType);
            if (viewModelType != null)
            {
                var viewModel = Activator.CreateInstance(viewModelType, model, parameters);
                (viewModel as IViewModel)?.SetUp(model);
                return viewModel;
            }

            return null;
        }
    }
}