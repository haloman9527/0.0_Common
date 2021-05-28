using System.Collections.Generic;
using UnityEngine;
using CZToolKit.Core.Blackboards;

public class BlackboardDemo : MonoBehaviour
{
    public CZBlackboardAsset blackboard;

    private void Update()
    {
        if (blackboard.Blackboard.TryGetValue("123", out Vector4 v))
        {
            Debug.Log(v);
        }
    }
}
