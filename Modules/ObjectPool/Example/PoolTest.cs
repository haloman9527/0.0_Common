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
using System.Collections;
using UnityEngine;
using CZToolKit.Core.ObjectPool;
using CZToolKit.Core.Attributes;

public class PoolTest : MonoBehaviour
{
    public UnityGOPool pool = new UnityGOPool();
    [ObjectPreviewButton]
    public UnityEngine.Object go;
    // Start is called before the first frame update
    void Start()
    {
        pool.InitCount();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("创建对象", GUILayout.Height(50), GUILayout.Width(200)))
        {
            GameObject go = pool.Spawn();
            go.transform.position = Random.insideUnitSphere * 3;
            StartCoroutine(Recycle(go));
        }
    }

    IEnumerator Recycle(GameObject go)
    {
        yield return new WaitForSeconds(3);
        pool.Recycle(go);
    }
}
