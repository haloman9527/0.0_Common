using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace CZToolKit.Core.Blackboards
{
    [CreateAssetMenu(menuName = "CZToolKit/New Blackboard", fileName = "New Blackboard")]
#if ODIN_INSPECTOR
    public class CZBlackboard : SerializedScriptableObject
#else
public class CZBlackboard : ScriptableObject
#endif
    {
#if ODIN_INSPECTOR
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
#else
        [SerializeReference]
#endif
        [SerializeField]
        Dictionary<string, ICZType> blackboard = new Dictionary<string, ICZType>();

        public Dictionary<string, ICZType> Blackboard { get { return blackboard; } }
    }

}
