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

        private Dictionary<string, UnityObject> referencesDict;

        private Dictionary<string, UnityObject> InternalReferencesDict
        {
            get
            {
                if (referencesDict == null)
                    referencesDict = new Dictionary<string, UnityObject>();
                return referencesDict;
            }
        }

        public IReadOnlyDictionary<string, UnityObject> ReferencesDict
        {
            get
            {
                if (referencesDict == null)
                    referencesDict = new Dictionary<string, UnityObject>();
                return referencesDict;
            }
        }

#if UNITY_EDITOR
        public void Add(UnityObject value)
        {
            string key = string.Empty;
            do
            {
                key = UnityEngine.Random.Range(int.MinValue, int.MaxValue).ToString();
            } while (ReferencesDict.ContainsKey(key));
            references.Add(new ReferencePair() { key = key, value = value});
        }

        public void RemoveAt(int index)
        {
            references.RemoveAt(index);
        }

        public void RemoveAt(ReferencePair pair)
        {
            references.Remove(pair);
        }

        public void ClearEmpty()
        {
            references.RemoveAll(pair => string.IsNullOrEmpty(pair.key) || pair.value == null);
        }

        public void Clear()
        {
            references.Clear();
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
            if (InternalReferencesDict.TryGetValue(key, out var value))
                return value;
            return null;
        }

        public T Get<T>(string key) where T : UnityObject
        {
            if (InternalReferencesDict.TryGetValue(key, out var value))
                return value as T;
            return null;
        }

        private void RefreshDict()
        {
            InternalReferencesDict.Clear();
            foreach (var pair in references)
            {
                if (string.IsNullOrEmpty(pair.key))
                    continue;
                InternalReferencesDict[pair.key] = pair.value;
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