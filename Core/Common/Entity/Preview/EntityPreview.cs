using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CZToolKit.ET
{
    public class EntityPreview : MonoBehaviour
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HideReferenceObjectPicker]
        [Sirenix.OdinInspector.HideLabel]
#endif
        [SerializeReference]
        public Entity Component;
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(EntityPreview))]
#if ODIN_INSPECTOR
    public class EntityPreviewInspector : Sirenix.OdinInspector.Editor.OdinEditor
#else
    public class EntityPreviewInspector : Editor
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
            var preview = target as EntityPreview;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.IntField("Instance Id", preview.Component.InstanceId);
            EditorGUI.EndDisabledGroup();
            base.OnInspectorGUI();
        }
    }
#endif
}