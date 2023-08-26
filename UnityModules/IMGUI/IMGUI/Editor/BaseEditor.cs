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
#if UNITY_EDITOR
using UnityEditor;

namespace CZToolKitEditor
{
    public abstract class BaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            SerializedProperty iterator = serializedObject.GetIterator();
            
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                EditorGUILayout.PropertyField(iterator, true);
            }

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnPropertyGUI(SerializedProperty property)
        {
            EditorGUILayout.PropertyField(property);
        }
    }
}
#endif