#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

namespace CZToolKit.Editors
{
    public class Menus
    {
        public const string WORLD_TREE_PREVIEW_DEFINE = "WORLD_TREE_PREVIEW";

        [MenuItem("Tools/CZToolKit/ET/Enable World Tree Preview")]
        public static void SwitchPreview()
        {
            var targetGroup = BuildTargetGroup.Standalone; // 选择目标平台
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';').ToList();
            if (defines.Contains(WORLD_TREE_PREVIEW_DEFINE))
                defines.Remove(WORLD_TREE_PREVIEW_DEFINE);
            else
                defines.Add(WORLD_TREE_PREVIEW_DEFINE);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines.ToArray());
        }

        [MenuItem("Tools/CZToolKit/ET/Enable World Tree Preview", validate = true)]
        public static bool EnablePreviewValid()
        {
            var targetGroup = BuildTargetGroup.Standalone; // 选择目标平台
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';').ToList();
            Menu.SetChecked("Tools/CZToolKit/ET/Enable World Tree Preview", defines.Contains(WORLD_TREE_PREVIEW_DEFINE));
            return true;
        }
    }
}
#endif