using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace CZToolKit.Core.Editors
{
    public class BuildSubAsset : BasicEditorWindow
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
            public UnityObject children;
        }

        public UnityObject parent;
        public List<ObjectInfo> childrens = new List<ObjectInfo>();

        SerializedObject serializedObject;
        ReorderableList reorderableList;

        private void OnEnable()
        {
            titleContent = new GUIContent("Build SubAsset");
            minSize = new Vector2(200, 300);

            serializedObject = new SerializedObject(this);
            reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("childrens"), false, true, false, false);
            reorderableList.footerHeight = 5;
            reorderableList.drawElementCallback += DrawElement;
            reorderableList.drawHeaderCallback += DrawHeader;

            LoadAsset(parent);
        }

        private void LoadAsset(UnityObject _parent)
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

            GUILayout.Button(new GUIContent("Button", "Tip"));


            if (index >= childrens.Count) return;

            GUILayout.BeginHorizontal();
            Rect r = rect;

            // 绘制名称
            r.width = rect.width * 0.3f;
            ObjectInfo objectInfo = childrens[index];
            objectInfo.name = EditorGUI.TextField(r, "", objectInfo.name);

            // 绘制资源
            r.x += r.width + 5;
            r.width = rect.width * 0.7f - 30;
            UnityObject tempObj = EditorGUI.ObjectField(r, objectInfo.children, typeof(UnityObject), false);
            if (parent != tempObj && objectInfo.children != tempObj)
            {
                objectInfo.children = tempObj;
                if (objectInfo.children != null)
                    objectInfo.name = objectInfo.children.name;
            }

            r.x += r.width + 5;
            r.width = 25;
            if (GUI.Button(r, EditorGUIUtility.IconContent("winbtn_mac_close_h")))
                childrens.RemoveAt(index); Repaint();

            GUILayout.EndHorizontal();
        }

        Vector2 scroll;
        private void OnGUI()
        {
            scroll = GUILayout.BeginScrollView(scroll, false, false);
            EditorGUILayout.HelpBox("构建SubAsset", MessageType.Warning, true);

            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            // 绘制父级资源
            UnityObject tempParent = EditorGUILayout.ObjectField(parent, typeof(UnityObject), false);
            if (GUILayout.Button(EditorGUIUtility.FindTexture("Refresh"), GUILayout.Width(25)))
                LoadAsset(parent);
            if (GUILayout.Button(EditorGUIUtility.FindTexture("winbtn_mac_close_h"), GUILayout.Width(25)))
                tempParent = null;
            GUILayout.EndHorizontal();

            // 绘制一个拖拽区域，接受拖进来的资源
            Rect dragDropArea = GUILayoutUtility.GetRect(position.width, 40);
            dragDropArea.x += 3;
            dragDropArea.width -= 6;
            GUI.Box(dragDropArea, "拖拽资源到此区域", (GUIStyle)"GroupBox");
            UnityObject t = EditorGUIExtension.DragDropAreaSingle(dragDropArea);
            if (t != null)
                tempParent = t;

            // 不接受子级资源
            if (tempParent == null || AssetDatabase.IsMainAsset(tempParent))
            {
                if (parent != tempParent)
                {
                    parent = tempParent;
                    LoadAsset(parent);
                }
            }

            // 绘制所有子级资源
            GUILayout.Space(15);
            reorderableList.DoLayoutList();

            // 绘制一个拖拽区域，接受拖进来的资源
            Rect dragDropAreaM = GUILayoutUtility.GetRect(position.width, 40);
            dragDropAreaM.x += 3;
            dragDropAreaM.width -= 6;
            GUI.Box(dragDropAreaM, "批量添加子级", (GUIStyle)"GroupBox");
            UnityObject[] objs = EditorGUIExtension.DragDropAreaMulti(dragDropAreaM);
            if (objs != null)
            {
                foreach (var obj in objs)
                {
                    if (obj == parent) continue;

                    childrens.Add(new ObjectInfo() { name = obj.name, children = obj });
                }
            }
            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            // 点击按钮开始构建
            if (GUILayout.Button("Build", GUILayout.Height(50)) && parent != null)
            {
                List<UnityObject> rawChildrens = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(parent)).ToList();

                // 添加列表中新增加的资源到子级
                foreach (var child in childrens)
                {
                    if (rawChildrens.Contains(child.children))
                    {
                        child.children.name = child.name;
                        continue;
                    }

                    UnityObject obj = Instantiate(child.children);
                    obj.name = child.name;
                    AssetDatabase.AddObjectToAsset(obj, parent);
                }

                // 把列表中删除的资源从子级移除
                foreach (var child in rawChildrens)
                {
                    if (childrens.Find(c => c.children == child) != null) continue;

                    AssetDatabase.RemoveObjectFromAsset(child);
                }

                // 保存
                AssetDatabase.SaveAssets();
                // 重载
                LoadAsset(parent);
            }

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
    }
}
