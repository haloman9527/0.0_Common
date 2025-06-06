using System;
using System.Reflection;

namespace Atom
{
    /// <summary>
    /// 反射调用静态方法
    /// </summary>
    public class StaticMethod : IStaticMethod
    {
        private MethodInfo m_Method;
        private object[] m_Arguments;

        public StaticMethod(Assembly assembly, string typeName, string methodName)
        {
            m_Method = assembly.GetType(typeName).GetMethod(methodName, new Type[] { });
            m_Arguments = new object[0];
        }

        void IStaticMethod.Run(object[] args)
        {
            m_Method.Invoke(null, m_Arguments);
        }

        public void Run()
        {
            (this as IStaticMethod).Run(m_Arguments);
        }
    }
}