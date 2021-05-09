using System;
using System.Collections.Generic;

namespace CZToolKit.Core.Blackboards
{
    public class CZTypeFactory
    {
        public static Dictionary<Type, Type> TypeMap = new Dictionary<Type, Type>();
        public static Dictionary<Type, Func<ICZType>> TypeCreator = new Dictionary<Type, Func<ICZType>>();

        static CZTypeFactory()
        {
            AddType<int, CZInt>(() => { return new CZInt(); });
        }

        public static void AddType<RT, CZT>(Func<ICZType> _creator)
        {
            AddType(typeof(RT), typeof(CZT), _creator);
        }

        public static void AddType(Type _rt, Type _czt, Func<ICZType> _creator)
        {
            TypeMap[_rt] = _czt;
            TypeCreator[_rt] = _creator;
        }

        public static ICZType GetNew<RT>()
        {
            return GetNew(typeof(RT));
        }

        public static ICZType GetNew(Type _rt)
        {
            ICZType t = null;
            if (TypeCreator.TryGetValue(_rt, out Func<ICZType> _creator))
                t = _creator();
            return t;
        }
    }
}
