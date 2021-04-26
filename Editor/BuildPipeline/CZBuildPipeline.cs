using UnityEditor;
using UnityEngine;
using System.Collections;

namespace CZToolKit.Core.Editors.BuildPipelines
{
    [CreateAssetMenu(menuName = "CZToolKit/BuildPipeline/New BuildPipeline", fileName = "New BuildPipeline")]
    public class CZBuildPipeline : ScriptableObject
    {
        static CZBuildPipeline()
        {
            CZAssetModificationProcessor.onWillCreateAsset += newFile =>
            {
                GlobalEditorCoroutine.StartCoroutine(OnCreatedNew(newFile));
            };
        }

        static IEnumerator OnCreatedNew(string _newFile)
        {
            yield return new WaitUntil_E(() => { return true; });

            CZBuildPipeline pipeline = AssetDatabase.LoadAssetAtPath<CZBuildPipeline>(_newFile);
            
            if (pipeline != null)
            {
                pipeline.companyName = PlayerSettings.companyName;
                pipeline.productName = PlayerSettings.productName;
                pipeline.version = PlayerSettings.bundleVersion;
                EditorUtility.SetDirty(pipeline);
            }
        }

        public string companyName;
        public string productName;
        public string version;
    }
}
