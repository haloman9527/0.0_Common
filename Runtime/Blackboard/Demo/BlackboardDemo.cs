using System.Collections.Generic;
using UnityEngine;
using CZToolKit.Core.Blackboards;

public class BlackboardDemo : MonoBehaviour
{
    public Dictionary<string, ICZType> bb = new Dictionary<string, ICZType>();
    public CZBlackboard blackboard;

    private void Update()
    {
        if (blackboard.Blackboard.TryGetData("123", out Vector4 v))
        {
            Debug.Log(v);
        }
    }
}
