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

namespace CZToolKit
{
    [DisallowMultipleComponent]
    public class ReferenceCollector : MonoBehaviour
    {
        [Serializable]
        public struct ReferencePair
        {
            public string key;
            public UnityObject value;
        }

#if !UNITY_EDITOR
        [HideInInspector] public List<ReferencePair> references = new List<ReferencePair>();
#else
        [SerializeField]
        public List<ReferencePair> references = new List<ReferencePair>();
#endif

        private Dictionary<string, UnityObject> referencesMap;

        private Dictionary<string, UnityObject> ReferencesMap_Internal
        {
            get
            {
                if (referencesMap == null)
                {
                    referencesMap = new Dictionary<string, UnityObject>();
                    foreach (var pair in references)
                    {
                        if (string.IsNullOrEmpty(pair.key))
                            continue;
                        ReferencesMap_Internal[pair.key] = pair.value;
                    }
                }
                return referencesMap;
            }
        }

        public IReadOnlyDictionary<string, UnityObject> ReferencesMap
        {
            get { return ReferencesMap_Internal; }
        }

        public IList<ReferencePair> References
        {
            get { return references; }
        }

        public T Get<T>(string key) where T : UnityObject
        {
            if (ReferencesMap.TryGetValue(key, out var obj))
            {
                return obj as T;
            }

            return null;
        }
    }
}