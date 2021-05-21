using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Events;

//#if ODIN_INSPECTOR
//using Sirenix.OdinInspector.Editor;
//#endif

namespace CZToolKit.Core.Editors
{
    public abstract class BasicEditor : UnityEditor.Editor
    {
        Dictionary<string, UnityAction<SerializedProperty>> customDrawers;
        //#if ODIN_INSPECTOR
        //        PropertyTree tree;
        //#endif

        protected virtual void OnEnable()
        {
            EditorApplication.update += Update;
            customDrawers = new Dictionary<string, UnityAction<SerializedProperty>>();
            RegisterDrawers();

            //#if ODIN_INSPECTOR
            //            tree = PropertyTree.Create(serializedObject);
            //#endif
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        Stack<EditorCoroutine> coroutineStack = new Stack<EditorCoroutine>();

        protected virtual void Update()
        {
            int count = coroutineStack.Count;
            while (count-- > 0)
            {
                EditorCoroutine coroutine = coroutineStack.Pop();
                if (!coroutine.IsRunning) continue;
                ICondition condition = coroutine.Current as ICondition;
                if (condition == null || condition.Result(coroutine))
                {
                    if (!coroutine.MoveNext())
                        continue;
                }
                coroutineStack.Push(coroutine);
            }
        }

        public EditorCoroutine StartCoroutine(IEnumerator _coroutine)
        {
            EditorCoroutine coroutine = new EditorCoroutine(_coroutine);
            coroutineStack.Push(coroutine);
            return coroutine;
        }

        public void StopCoroutine(EditorCoroutine _coroutine)
        {
            _coroutine.Stop();
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
            //#if ODIN_INSPECTOR
            //            if (tree != null)
            //            {
            //                tree.BeginDraw(true);
            //                tree.Draw(true);
            //                tree.EndDraw();
            //            }
            //#else
            EditorGUI.BeginChangeCheck();

            SerializedProperty iterator = serializedObject.GetIterator();
            iterator.NextVisible(true);
            do
            {
                UnityAction<SerializedProperty> drawer;
                if (customDrawers.TryGetValue(iterator.propertyPath, out drawer))
                    drawer(iterator);
                else
                {
                    EditorGUILayout.PropertyField(iterator);
                }
            } while (iterator.NextVisible(false));

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
            serializedObject.Update();
            //#endif
        }

        private void DrawScript(SerializedProperty _serializedProperty)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_serializedProperty);
            EditorGUI.EndDisabledGroup();
        }
    }
}
