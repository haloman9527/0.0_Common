//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Collections.Generic;

namespace Atom
{
    /// <summary>
    /// 数据结点接口。
    /// </summary>
    public interface IDataNode
    {
        /// <summary>
        /// 获取数据结点的名称。
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// 获取数据结点的完整名称。
        /// </summary>
        string FullName
        {
            get;
        }

        /// <summary>
        /// 获取父数据结点。
        /// </summary>
        IDataNode Parent
        {
            get;
        }

        /// <summary>
        /// 获取子数据结点的数量。
        /// </summary>
        int ChildCount
        {
            get;
        }
        
        /// <summary>
        /// 所有子数据节点。
        /// </summary>
        IEnumerator<IDataNode> Children { get; }

        /// <summary>
        /// 根据类型获取数据结点的数据。
        /// </summary>
        /// <typeparam name="T">要获取的数据类型。</typeparam>
        /// <returns>指定类型的数据。</returns>
        T GetData<T>() where T : class;

        /// <summary>
        /// 获取数据结点的数据。
        /// </summary>
        /// <returns>数据结点数据。</returns>
        object GetData();

        /// <summary>
        /// 设置数据结点的数据。
        /// </summary>
        /// <typeparam name="T">要设置的数据类型。</typeparam>
        /// <param name="data">要设置的数据。</param>
        void SetData<T>(T data) where T : class;

        /// <summary>
        /// 设置数据结点的数据。
        /// </summary>
        /// <param name="data">要设置的数据。</param>
        void SetData(object data);

        /// <summary>
        /// 根据名称检查是否存在子数据结点。
        /// </summary>
        /// <param name="name">子数据结点名称。</param>
        /// <returns>是否存在子数据结点。</returns>
        bool HasChild(string name);

        /// <summary>
        /// 根据名称获取子数据结点。
        /// </summary>
        /// <param name="name">子数据结点名称。</param>
        /// <returns>指定名称的子数据结点，如果没有找到，则返回空。</returns>
        IDataNode GetChild(string name);

        /// <summary>
        /// 根据名称获取或增加子数据结点。
        /// </summary>
        /// <param name="name">子数据结点名称。</param>
        /// <returns>指定名称的子数据结点，如果对应名称的子数据结点已存在，则返回已存在的子数据结点，否则增加子数据结点。</returns>
        IDataNode GetOrAddChild(string name);
        
        /// <summary>
        /// 根据名称移除子数据结点。
        /// </summary>
        /// <param name="name">子数据结点名称。</param>
        void RemoveChild(string name);

        /// <summary>
        /// 移除当前数据结点的数据和所有子数据结点。
        /// </summary>
        void Clear();

        /// <summary>
        /// 获取数据结点字符串。
        /// </summary>
        /// <returns>数据结点字符串。</returns>
        string ToString();
    }
}