#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

namespace CZToolKit.Editors
{
    public class Menus
    {
        public const string WORLD_TREE_PREVIEW_DEFINE = "WORLD_TREE_PREVIEW";

#if HALOMAN
        [MenuItem("Tools/CZToolKit/World Tree/Enable World Tree Preview")]
#endif
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

#if HALOMAN
        [MenuItem("Tools/CZToolKit/World Tree/Enable World Tree Preview", validate = true)]
#endif
        public static bool EnablePreviewValid()
        {
            var targetGroup = BuildTargetGroup.Standalone; // 选择目标平台
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';').ToList();
            Menu.SetChecked("Tools/CZToolKit/World Tree/Enable World Tree Preview", defines.Contains(WORLD_TREE_PREVIEW_DEFINE));
            return true;
        }
    }
}
#endif