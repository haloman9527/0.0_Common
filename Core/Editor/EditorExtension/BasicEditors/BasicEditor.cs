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
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Events;

namespace CZToolKit.Core.Editors
{
    public abstract class BasicEditor : Editor
    {
        Dictionary<string, UnityAction<SerializedProperty>> customDrawers;
        EditorCoroutineService CoroutineMachineController = new EditorCoroutineService();

        protected virtual void OnEnable()
        {
            EditorApplication.update += Update;
            customDrawers = new Dictionary<string, UnityAction<SerializedProperty>>();
            RegisterDrawers();
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        protected virtual void Update()
        {
            CoroutineMachineController.Update();
        }

        public EditorCoroutine StartCoroutine(IEnumerator enumerator)
        {
            return CoroutineMachineController.StartCoroutine(enumerator);
        }

        public void StopCoroutine(EditorCoroutine coroutine)
        {
            CoroutineMachineController.StopCoroutine(coroutine);
        }

        protected void RegisterDrawer(string propertyPath, UnityAction<SerializedProperty> drawer)
        {
            customDrawers[propertyPath] = drawer;
        }

        protected virtual void RegisterDrawers()
        {
            RegisterDrawer("m_Script", DrawScript);
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            SerializedProperty iterator = serializedObject.GetIterator();
            iterator.NextVisible(true);
            do
            {
                UnityAction<SerializedProperty> drawer;
                if (customDrawers.TryGetValue(iterator.propertyPath, out drawer))
                    drawer(iterator);
                else
                    EditorGUILayout.PropertyField(iterator);
            } while (iterator.NextVisible(false));

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawScript(SerializedProperty serializedProperty)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedProperty);
            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif