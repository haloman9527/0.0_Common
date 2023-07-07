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
using UnityEngine;
using CZToolKit.Common.Collection;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Common
{
    [DisallowMultipleComponent]
    public class ReferenceCollector : MonoBehaviour, ISerializationCallbackReceiver
    {
        [Serializable]
        public class ReferencePair
        {
            public string key;
            public UnityObject value;
        }

        [SerializeField]
        [HideInInspector]
        private List<ReferencePair> references = new List<ReferencePair>();

        private Dictionary<string, UnityObject> referencesMap_Internal;

        internal Dictionary<string, UnityObject> ReferencesMap_Internal
        {
            get
            {
                if (referencesMap_Internal == null)
                    referencesMap_Internal = new Dictionary<string, UnityObject>();
                return referencesMap_Internal;
            }
        }

#if UNITY_EDITOR
        public void Add(string key, UnityObject value)
        {
            ReferencesMap_Internal.Add(key, value);
            references.Add(new ReferencePair() { key = key, value = value});
        }

        public void RemoveAt(int index)
        {
            references.RemoveAt(index);
        }

        public void Remove(ReferencePair pair)
        {
            references.Remove(pair);
        }

        public void Clear()
        {
            references.Clear();
        }

        public void ClearEmpty()
        {
            references.RemoveAll(pair => string.IsNullOrEmpty(pair.key) || pair.value == null);
        }

        public void Sort()
        {
            references.QuickSort((a, b) =>
            {
                return a.key.CompareTo(b.key);
            });
        }
#endif

        public UnityObject Get(string key)
        {
            if (ReferencesMap_Internal.TryGetValue(key, out var value))
                return value;
            return null;
        }

        public T Get<T>(string key) where T : UnityObject
        {
            if (ReferencesMap_Internal.TryGetValue(key, out var value))
                return value as T;
            return null;
        }

        private void RefreshDict()
        {
            ReferencesMap_Internal.Clear();
            foreach (var pair in references)
            {
                if (string.IsNullOrEmpty(pair.key))
                    continue;
                ReferencesMap_Internal[pair.key] = pair.value;
            }
        }

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            RefreshDict();
        }
    }
}