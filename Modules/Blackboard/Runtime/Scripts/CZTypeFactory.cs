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
using System;
using System.Collections.Generic;

namespace CZToolKit.Core.Blackboards
{
    public class CZTypeFactory
    {
        public static Dictionary<Type, Func<ICZType>> TypeCreator = new Dictionary<Type, Func<ICZType>>();

        static CZTypeFactory()
        {
            foreach (var type in Util_Reflection.GetChildTypes<ICZType>())
            {
                if (!type.IsGenericType && !type.IsAbstract)
                    TypeCreator[type] = () => { return Activator.CreateInstance(type) as ICZType; };
            }
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
