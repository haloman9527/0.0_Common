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

namespace Atom
{
    public class BindableList<T> : BindableCollection<List<T>, T>
    {
        public BindableList() : base(new List<T>())
        {
        }

        public BindableList(List<T> list) : base(list)
        {
        }

        public BindableList(Func<List<T>> getter, Action<List<T>> setter) : base(getter, setter)
        {
        }
    }
}