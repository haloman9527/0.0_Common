//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace CZToolKit.GameFramework.DataNode
{
    public class DataNode : IDataNode
    {
        private char seperator;
        private string m_Name;
        private string m_FullName;
        private IDataNode m_Parent;
        private object m_Data;
        private Dictionary<string, IDataNode> m_Children = new Dictionary<string, IDataNode>();

        public string Name => m_Name;

        public string FullName => m_FullName;

        public IDataNode Parent => m_Parent;
        
        public int ChildCount => m_Children.Count;

        public DataNode()
        {
            this.m_Name = string.Empty;
            this.m_FullName = string.Empty;
            this.m_Parent = null;
        }

        public static DataNode Create(string name, char seperator, DataNode parent)
        {
            var dataNode = ObjectPools.Spawn<DataNode>();
            dataNode.m_Name = name;
            dataNode.m_FullName = parent == null ? name : $"{parent.FullName}{seperator}{name}";
            dataNode.m_Parent = parent;
            dataNode.seperator = seperator;
            parent?.m_Children.Add(name, parent);
            return dataNode;
        }
        
        public T GetData<T>() where T : class
        {
            return m_Data as T;
        }

        public object GetData()
        {
            return m_Data;
        }

        public void SetData<T>(T data) where T : class
        {
            this.m_Data = data;
        }

        public void SetData(object data)
        {
            this.m_Data = data;
        }

        public bool HasChild(string name)
        {
            return m_Children.ContainsKey(name);
        }

        public IDataNode GetChild(string name)
        {
            m_Children.TryGetValue(name, out var dataNode);
            return dataNode;
        }

        public IDataNode GetOrAddChild(string name)
        {
            if (!m_Children.TryGetValue(name, out var dataNode))
            {
                m_Children[name] = dataNode = Create(name, this.seperator, this);
            }

            return dataNode;
        }

        public IDataNode[] GetAllChild()
        {
            return m_Children.Values.ToArray();
        }

        public void GetAllChild(List<IDataNode> results)
        {
            results.Clear();
            results.AddRange(m_Children.Values);
        }

        public void RemoveChild(string name)
        {
            m_Children.Remove(name);
        }

        public void Clear()
        {
            foreach (var pair in m_Children)
            {
                pair.Value.Clear();
                ObjectPools.Recycle(typeof(DataNode), pair.Value);
            }

            m_Children.Clear();
        }
    }
}