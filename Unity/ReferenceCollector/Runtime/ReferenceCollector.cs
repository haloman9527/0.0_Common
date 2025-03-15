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
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Atom
{
    [DisallowMultipleComponent]
    public class ReferenceCollector : MonoBehaviour
    {
        [Serializable]
        public class ReferencePair
        {
            public string key;
            public UnityObject value;
        }

#if !UNITY_EDITOR
        [SerializeField] private List<ReferencePair> references = new List<ReferencePair>();
#else
        [SerializeField]
        public List<ReferencePair> references = new List<ReferencePair>();
#endif

        private Dictionary<string, ReferencePair> referencesMap;

        private Dictionary<string, ReferencePair> ReferencesMap
        {
            get
            {
                if (referencesMap == null)
                {
                    InitReferencesMap();
                }
                return referencesMap;
            }
        }

        public IReadOnlyList<ReferencePair> References => references;

        private void InitReferencesMap()
        {
            var tempReferencesMap = new Dictionary<string, ReferencePair>();
            foreach (var pair in references)
            {
                if (string.IsNullOrEmpty(pair.key))
                    continue;
                tempReferencesMap.Add(pair.key, pair);
            }

            referencesMap = tempReferencesMap;
        }

        public bool Contains(string key)
        {
            return ReferencesMap.ContainsKey(key);
        }

        public T Get<T>(string key) where T : UnityObject
        {
            if (ReferencesMap.TryGetValue(key, out var obj))
            {
                return obj.value as T;
            }

            return null;
        }

        public void Set(string key, UnityObject uo)
        {
            if (referencesMap != null && referencesMap.TryGetValue(key, out var pair))
            {
                pair.value = uo;
            }
            else
            {
                pair = new ReferencePair() { key = key, value = uo };
                if (referencesMap != null)
                    referencesMap.Add(pair.key, pair);
                references.Add(pair);
            }
        }

        public void Remove(string key)
        {
            if (referencesMap != null && referencesMap.TryGetValue(key, out var pair))
            {
                referencesMap.Remove(key);
                references.RemoveAll(item => item.key == key);
            }
            else
            {
                references.RemoveAll(item => item.key == key);
            }
        }

        private void OnValidate()
        {
            referencesMap = null;
        }
    }
}