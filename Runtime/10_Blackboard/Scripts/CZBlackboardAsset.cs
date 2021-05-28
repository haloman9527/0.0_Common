using OdinSerializer;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Blackboards
{
    [CreateAssetMenu(menuName = "CZToolKit/New Blackboard", fileName = "New Blackboard")]
    public class CZBlackboardAsset : Sirenix.OdinInspector.SerializedScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector]
        string serializedBlackboard = string.Empty;

        [SerializeField, HideInInspector]
        List<UnityObject> unityReference = new List<UnityObject>();

        [SerializeField]
        CZBlackboard blackboard = new CZBlackboard();

        public CZBlackboard Blackboard { get { return blackboard; } }

        //public void OnBeforeSerialize()
        //{
        //    serializedBlackboard = Encoding.UTF8.GetString(SerializationUtility.SerializeValue(blackboard, DataFormat.JSON, out unityReference));
        //}

        //public void OnAfterDeserialize()
        //{
        //    blackboard = SerializationUtility.DeserializeValue<CZBlackboard>(Encoding.UTF8.GetBytes(serializedBlackboard), DataFormat.JSON, unityReference);
        //}
    }

}
