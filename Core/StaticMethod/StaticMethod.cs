using System;
using System.Reflection;

namespace Atom
{
    public class StaticMethod : IStaticMethod
    {
        private MethodInfo method;
        private object[] param;

        public StaticMethod(Assembly assembly, string typeName, string methodName)
        {
            method = assembly.GetType(typeName).GetMethod(methodName, new Type[0]);
            param = new object[0];
        }

        void IStaticMethod.Run(object[] args)
        {
            method.Invoke(null, param);
        }

        public void Run()
        {
            (this as IStaticMethod).Run(param);
        }
    }

    public class StaticMethod<A> : IStaticMethod
    {
        private MethodInfo method;
        private object[] param;

        public StaticMethod(Assembly assembly, string typeName, string methodName)
        {
            method = assembly.GetType(typeName).GetMethod(methodName, new Type[1] { TypeCache<A>.TYPE });
            param = new object[2];
        }

        void IStaticMethod.Run(object[] args)
        {
            method.Invoke(null, param);
        }

        public void Run(A a)
        {
            this.param[0] = a;

            (this as IStaticMethod).Run(param);

            this.param[0] = null;
        }
    }

    public class StaticMethod<A, B> : IStaticMethod
    {
        private MethodInfo method;
        private object[] param;

        public StaticMethod(Assembly assembly, string typeName, string methodName)
        {
            method = assembly.GetType(typeName).GetMethod(methodName, new Type[2] { TypeCache<A>.TYPE, TypeCache<B>.TYPE });
            param = new object[2];
        }

        void IStaticMethod.Run(object[] args)
        {
            method.Invoke(null, param);
        }

        public void Run(A a, B b)
        {
            this.param[0] = a;
            this.param[1] = b;

            (this as IStaticMethod).Run(param);

            this.param[0] = null;
            this.param[1] = null;
        }
    }

    public class StaticMethod<A, B, C> : IStaticMethod
    {
        private MethodInfo method;
        private object[] param;

        public StaticMethod(Assembly assembly, string typeName, string methodName)
        {
            method = assembly.GetType(typeName).GetMethod(methodName, new Type[3] { TypeCache<A>.TYPE, TypeCache<B>.TYPE, TypeCache<C>.TYPE });
            param = new object[3];
        }

        void IStaticMethod.Run(object[] args)
        {
            method.Invoke(null, param);
        }

        public void Run(A a, B b, C c)
        {
            this.param[0] = a;
            this.param[1] = b;
            this.param[2] = c;

            (this as IStaticMethod).Run(param);

            this.param[0] = null;
            this.param[1] = null;
            this.param[2] = null;
        }
    }

    public class StaticMethod<A, B, C, D> : IStaticMethod
    {
        private MethodInfo method;
        private object[] param;

        public StaticMethod(Assembly assembly, string typeName, string methodName)
        {
            method = assembly.GetType(typeName).GetMethod(methodName, new Type[4] { TypeCache<A>.TYPE, TypeCache<B>.TYPE, TypeCache<C>.TYPE, TypeCache<D>.TYPE });
            param = new object[4];
        }

        void IStaticMethod.Run(object[] args)
        {
            method.Invoke(null, param);
        }

        public void Run(A a, B b, C c, D d)
        {
            this.param[0] = a;
            this.param[1] = b;
            this.param[2] = c;
            this.param[3] = d;

            (this as IStaticMethod).Run(param);

            this.param[0] = null;
            this.param[1] = null;
            this.param[2] = null;
            this.param[3] = null;
        }
    }
}