using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CZToolKit
{
    public class Pipeline
    {
        public class Context
        {
            private readonly Dictionary<System.Type, object> contextObjects = new Dictionary<System.Type, object>();

            public void ClearAllContext()
            {
                contextObjects.Clear();
            }

            public T GetContextObject<T>() where T : class
            {
                var type = typeof(T);
                if (contextObjects.TryGetValue(type, out object obj))
                    return obj as T;
                return null;
            }

            public void SetContextObject<T>(T contextObject) where T : class
            {
                var type = typeof(T);
                contextObjects[type] = contextObject;
            }
        }

        public interface ITask
        {
            string Desc { get; }
        }

        public interface ISyncTask : ITask
        {
            void Execute(Pipeline pipeline);
        }

        public interface IAsyncTask : ITask
        {
            Task Execute(Pipeline pipeline);
        }

        public static void Run(List<ITask> tasks, Context context)
        {
            if (tasks == null)
                throw new ArgumentNullException("tasks");
            if (context == null)
                throw new ArgumentNullException("context");
            var pipeline = new Pipeline(tasks, context);
            pipeline.Run();
        }

        public readonly List<ITask> tasks;

        public readonly Pipeline.Context context;

        public Pipeline()
        {
            this.tasks = new List<ITask>();
            this.context = new Context();
        }

        public Pipeline(List<ITask> tasks, Context context)
        {
            this.tasks = tasks;
            this.context = context;
        }

        public async void Run()
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                if (task == null)
                    continue;

                try
                {
                    if (task is IAsyncTask asyncTask)
                    {
                        await asyncTask.Execute(this);
                    }
                    else if (task is ISyncTask syncTask)
                    {
                        syncTask.Execute(this);
                    }
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException($"Tasks[{i}]:{task.GetType()}:{task.Desc}", e);
                }
            }
        }
    }
}