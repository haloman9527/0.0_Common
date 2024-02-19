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
    public class ReferenceCollector : MonoBehaviour, ISerializationCallbackReceiver
    {
        [Serializable]
        public struct ReferencePair
        {
            public string key;
            public UnityObject value;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Warning: Edit Only In Editor !!!
        /// </summary>
        [HideInInspector]
        public List<ReferencePair> references = new List<ReferencePair>();
#else
        [SerializeField]
        private List<ReferencePair> references = new List<ReferencePair>();
#endif

        private Dictionary<string, UnityObject> referencesMap;

        private Dictionary<string, UnityObject> ReferencesMap_Internal
        {
            get
            {
                if (referencesMap == null)
                    referencesMap = new Dictionary<string, UnityObject>();
                return referencesMap;
            }
        }
        
        public IReadOnlyDictionary<string, UnityObject> ReferencesMap
        {
            get
            {
                return ReferencesMap_Internal;
            }
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            ReferencesMap_Internal.Clear();
            foreach (var pair in references)
            {
                if (string.IsNullOrEmpty(pair.key))
                    continue;
                ReferencesMap_Internal[pair.key] = pair.value;
            }
#endif
        }
    }
}