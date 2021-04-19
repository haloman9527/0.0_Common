using CZToolKit.Core.Editors;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using UnityEngine;

public class CZMenuEditorWindow : BasicMenuEditorWindow
{
    [MenuItem("Tools/MenuEditorWindow")]
    public static void Open()
    {
        GetWindow<CZMenuEditorWindow>();
    }

    protected override float LeftMinWidth => 1;
    protected override float RightMinWidth => 200;

    protected override CZMenuTreeView BuildMenuTree(TreeViewState _treeViewState)
    {
        CZMenuTreeView treeView = new CZMenuTreeView(_treeViewState);

        treeView.AddMenuItem("Giao家族/老Giao").rightDrawer += _rect =>
        {
            GUILayout.Button("Giao~~");
        };
        treeView.AddMenuItem("Giao家族/老Giao/大Giao").rightDrawer += _rect =>
        {
            GUILayout.Button("GiaoGiao!!");
        };
        treeView.AddMenuItem("Giao家族/老Giao/女Giao").rightDrawer += _rect =>
        {
            GUILayout.Button("Giao~");
        };
        treeView.AddMenuItem("Giao家族/老Giao/大Giao/小Giao").rightDrawer += _rect =>
        {
            GUILayout.Button("Gi");
        };

        return treeView;
    }
}
