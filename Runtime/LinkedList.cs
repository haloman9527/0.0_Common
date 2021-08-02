//#region 注 释
///***
// *
// *  Title:
// *  
// *  Description:
// *  
// *  Date:
// *  Version:
// *  Writer: 半只龙虾人
// *  Github: https://github.com/HalfLobsterMan
// *  Blog: https://www.crosshair.top/
// *
// */
//#endregion
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;

//namespace CZToolKit.Core
//{

//    public class LinkedList<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList
//    {
//        public class LinkedElement<ET>
//        {
//            public ET value;

//            public LinkedElement<ET> next;
//        }

//        LinkedElement<T> start;

//        public int Count => throw new System.NotImplementedException();

//        public bool IsReadOnly => throw new System.NotImplementedException();

//        public T this[int index]
//        {
//            get
//            {
//                foreach (var item in this)
//                {
//                    if (index <= 0)
//                        return item;
//                    index--;
//                }
//                throw new IndexOutOfRangeException();
//            }
//            set
//            {
//                foreach (var item in GetEEnumerator())
//                {
//                    if (index <= 0)
//                    {
//                        item.value = value;
//                        return;
//                    }
//                    index--;
//                }
//                throw new IndexOutOfRangeException();
//            }
//        }

//        IEnumerable<LinkedElement<T>> GetEEnumerator()
//        {
//            var current = start;

//            while (current != null)
//            {
//                yield return current;
//                current = current.next;
//            }
//        }

//        public IEnumerator<T> GetEnumerator()
//        {
//            var current = start;

//            while (current != null)
//            {
//                yield return current.value;
//                current = current.next;
//            }
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return GetEnumerator();
//        }

//        public int IndexOf(T _item)
//        {
//            int index = 0;
//            foreach (var item in this)
//            {
//                if (item.Equals(_item))
//                {
//                    return index;
//                }
//                index++;
//            }
//            return -1;
//        }

//        public void Insert(int index, T item)
//        {
//            throw new System.NotImplementedException();
//        }

//        public void RemoveAt(int index)
//        {

//        }

//        public void Add(T _item)
//        {
//            var current = start;
//            foreach (var item in GetEEnumerator())
//            {
//                current = item;
//            }
//            if (current == null)
//                start = new LinkedElement<T>() { value = _item };
//            else
//                current.next = new LinkedElement<T>() { value = _item };
//        }

//        public void Clear()
//        {
//            start = null;
//        }

//        public bool Contains(T item)
//        {
//            throw new System.NotImplementedException();
//        }

//        public void CopyTo(T[] array, int arrayIndex)
//        {
//            throw new System.NotImplementedException();
//        }

//        public bool Remove(T item)
//        {
//            throw new System.NotImplementedException();
//        }
//    }
//}
