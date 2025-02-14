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
using Sirenix.Utilities;

namespace Moyo
{
    public static class ViewModelFactory
    {
        private static bool s_Initialized;
        private static Dictionary<Type, IViewModelProducer> s_ViewModelProducers;

        static ViewModelFactory()
        {
            Init(true);
        }

        public static void Init(bool force)
        {
            if (!force && s_Initialized)
                return;

            if (s_ViewModelProducers == null)
            {
                s_ViewModelProducers = new Dictionary<Type, IViewModelProducer>();
            }
            else
            {
                s_ViewModelProducers.Clear();
            }

            foreach (var type in TypesCache.GetTypesWithAttribute<ViewModelAttribute>())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (type.IsGenericType)
                {
                    continue;
                }

                var attribute = CustomAttributeExtensions.GetCustomAttribute<ViewModelAttribute>(type, true);
                if (HasDefaultConstructor(type))
                {
                    var producerType = typeof(ViewModelProducerS<>).MakeGenericType(type);
                    s_ViewModelProducers.Add(attribute.modelType, Activator.CreateInstance(producerType) as IViewModelProducer);
                }
                else
                {
                    var producerType = typeof(ViewModelProducerR<>).MakeGenericType(type);
                    s_ViewModelProducers.Add(attribute.modelType, Activator.CreateInstance(producerType) as IViewModelProducer);
                }
            }

            s_Initialized = true;
        }

        private static bool HasDefaultConstructor(Type type)
        {
            return type == typeof(string) || type.IsArray || type.IsValueType || type.GetConstructor(System.Type.EmptyTypes) != (ConstructorInfo)null;
        }

        private static IViewModelProducer GetProducer(Type modelType)
        {
            if (!s_ViewModelProducers.TryGetValue(modelType, out var producer))
            {
                var type = modelType;
                do
                {
                    type = type.BaseType;
                } while (type != null && !s_ViewModelProducers.TryGetValue(type, out producer));

                s_ViewModelProducers[modelType] = producer;
            }

            return producer;
        }

        public static Type GetViewModelType(Type modelType)
        {
            return GetProducer(modelType)?.ViewModelType;
        }

        public static ViewModel ProduceViewModel(object model)
        {
            var modelType = model.GetType();
            var producer = GetProducer(modelType);
            if (producer != null)
            {
                return Activator.CreateInstance(producer.ViewModelType, model) as ViewModel;
            }

            return null;
        }

        public static object ProduceViewModel(Type modelType)
        {
            var producer = GetProducer(modelType);
            if (producer != null)
            {
                return producer.Produce();
            }

            return null;
        }

        public static object ProduceViewModel<TModel>()
        {
            var producer = GetProducer(TypeCache<TModel>.TYPE);
            if (producer != null)
            {
                return producer.Produce();
            }

            return null;
        }
    }

    public interface IViewModelProducer
    {
        Type ViewModelType { get; }

        object Produce();
    }

    public sealed class ViewModelProducerR<T> : IViewModelProducer where T : class
    {
        public Type ViewModelType => typeof(T);

        public T Produce() => Activator.CreateInstance<T>();

        object IViewModelProducer.Produce() => Produce();
    }

    public sealed class ViewModelProducerS<T> : IViewModelProducer where T : class, new()
    {
        public Type ViewModelType => typeof(T);

        public T Produce() => new T();

        object IViewModelProducer.Produce() => Produce();
    }
}