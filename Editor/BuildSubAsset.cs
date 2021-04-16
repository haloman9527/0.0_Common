using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using Object = UnityEngine.Object;
using System;

namespace CZToolKit.Core.Editors
{
    public class BuildSubAsset : EditorWindow
    {
        [MenuItem("Tools/CZToolKit/Build SubAsset")]
        public static void Open()
        {
            GetWindow<BuildSubAsset>();
        }

        [Serializable]
        public class ObjectInfo
        {
            public string name;
            public Object children;
        }

        public Object parent;
        public List<ObjectInfo> childrens = new List<ObjectInfo>();

        SerializedObject serializedObject;
        ReorderableList reorderableList;
        private void OnEnable()
        {
            titleContent = new GUIContent("Build SubAsset");
            this.minSize = new Vector2(200, 300);

            serializedObject = new SerializedObject(this);
            reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("childrens"), true, true, true, true);
            reorderableList.drawElementCallback += DrawElement;
            reorderableList.drawHeaderCallback += DrawHeader;

            LoadAsset(parent);
        }

        private void LoadAsset(Object _parent)
        {
            if (_parent != null)
            {
                childrens.Clear();
                foreach (var child in AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(_parent)))
                {
                    childrens.Add(new ObjectInfo() { name = child.name, children = child });
                }
            }
            else
                childrens.Clear();
        }

        private void DrawHeader(Rect rect)
        {
            GUI.Label(rect, "Childrens");
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index >= childrens.Count) return;

            GUILayout.BeginHorizontal();
            Rect r = rect;

            r.width = 100;
            ObjectInfo objectInfo = childrens[index];
            objectInfo.name = EditorGUI.TextField(r, "", objectInfo.name);

            r.x = 125;
            r.width = rect.width - 100;
            Object tempParent = EditorGUI.ObjectField(r, objectInfo.children, typeof(Object), false);
            if (objectInfo.children != tempParent)
            {
                objectInfo.children = tempParent;
                if (objectInfo.children != null)
                    objectInfo.name = objectInfo.children.name;
            }

            GUILayout.EndHorizontal();
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("构建SubAsset", MessageType.Warning, true);

            EditorGUI.BeginChangeCheck();

            GUILayout.BeginHorizontal();
            Object tempParent = EditorGUILayout.ObjectField(parent, typeof(Object), false);
            if (parent != tempParent)
            {
                parent = tempParent;
                LoadAsset(parent);
            }
            if (GUILayout.Button(EditorGUIUtility.IconContent("Refresh"), GUILayout.Width(30)))
                LoadAsset(parent);
            GUILayout.EndHorizontal();


            GUILayout.Space(15);
            reorderableList.DoLayoutList();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Build", GUILayout.Height(50)) && parent != null)
            {
                List<Object> rawChildrens = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(parent)).ToList();

                foreach (var child in childrens)
                {
                    if (rawChildrens.Contains(child.children))
                    {
                        child.children.name = child.name;
                        AssetDatabase.RemoveObjectFromAsset(child.children);
                        AssetDatabase.AddObjectToAsset(child.children, parent);
                        continue;
                    }

                    Object obj = Instantiate(child.children);
                    obj.name = child.name;
                    AssetDatabase.AddObjectToAsset(obj, parent);
                }

                foreach (var child in rawChildrens)
                {
                    if (childrens.Find(c => c.children == child) != null) continue;

                    AssetDatabase.RemoveObjectFromAsset(child);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                LoadAsset(parent);
            }

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
    }
}
