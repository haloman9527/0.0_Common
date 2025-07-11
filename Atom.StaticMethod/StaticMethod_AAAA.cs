using System;
using System.Reflection;

namespace Atom
{
    /// <summary>
    /// 反射调用静态方法
    /// </summary>
    public class StaticMethod<A, B, C, D> : IStaticMethod
    {
        private MethodInfo m_Method;
        private object[] m_Arguments;

        public StaticMethod(Assembly assembly, string typeName, string methodName)
        {
            m_Method = assembly.GetType(typeName).GetMethod(methodName, new Type[] { TypeCache<A>.TYPE, TypeCache<B>.TYPE, TypeCache<C>.TYPE, TypeCache<D>.TYPE });
            m_Arguments = new object[4];
        }

        void IStaticMethod.Run(object[] args)
        {
            m_Method.Invoke(null, m_Arguments);
        }

        public void Run(A a, B b, C c, D d)
        {
            this.m_Arguments[0] = a;
            this.m_Arguments[1] = b;
            this.m_Arguments[2] = c;
            this.m_Arguments[3] = d;

            try
            {
                (this as IStaticMethod).Run(m_Arguments);
            }
            finally
            {
                this.m_Arguments[0] = null;
                this.m_Arguments[1] = null;
                this.m_Arguments[2] = null;
                this.m_Arguments[3] = null;
            }
        }
    }
}