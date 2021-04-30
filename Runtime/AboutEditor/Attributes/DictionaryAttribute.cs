using System;
using UnityEngine;

namespace CZToolKit.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DictionaryAttribute : PropertyAttribute { }
}
