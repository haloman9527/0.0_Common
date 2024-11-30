using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Moyo
{
    public class WorldTreeNodePreview : MonoBehaviour
    {
#if ODIN_INSPECTOR
        [FormerlySerializedAs("Component")]
        [Sirenix.OdinInspector.HideReferenceObjectPicker]
        [Sirenix.OdinInspector.HideLabel]
#endif
        [SerializeReference]
        public Node component;
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(WorldTreeNodePreview))]
#if ODIN_INSPECTOR
    public class NodePreviewInspector : Sirenix.OdinInspector.Editor.OdinEditor
#else
    public class NodePreviewInspector : Editor
#endif
    {
#if ODIN_INSPECTOR
        protected override void DrawTree()
        {
            this.Tree.DrawMonoScriptObjectField = false;
            base.DrawTree();
        }
#endif
        
        public override void OnInspectorGUI()
        {
            var node = target as WorldTreeNodePreview;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.IntField("Instance Id", node.component.InstanceId);
            EditorGUI.EndDisabledGroup();
            base.OnInspectorGUI();
        }
    }
#endif
}