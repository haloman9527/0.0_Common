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

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core
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
        public void Add()
        {
            string key = string.Empty;
            do
            {
                key = UnityEngine.Random.Range(int.MinValue, int.MaxValue).ToString();
            } while (ReferencesDict.ContainsKey(key));
            references.Add(new ReferencePair() { key = key });
        }

        public void RemoveAt(int index)
        {
            references.RemoveAt(index);
        }

        public void RemoveAt(ReferencePair pair)
        {
            references.Remove(pair);
        }

        public void Clear()
        {
            references.Clear();
        }
#endif

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