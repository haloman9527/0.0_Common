#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 
 *
 */
#endregion
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace CZToolKit.Core.Editors.Build
{
    public class DoSomethineBeforeBuilding : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {

        }
    }
}
