using System.Linq;
using UnityEditor;

namespace CZToolKit.Editors
{
    public class Menus
    {
        public const string NODE_PREVIEW_DEFINE = "NODE_PREVIEW";

        [MenuItem("Tools/CZToolKit/ET/Enable Node Preview")]
        public static void SwitchPreview()
        {
            var targetGroup = BuildTargetGroup.Standalone; // 选择目标平台
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';').ToList();
            if (defines.Contains(NODE_PREVIEW_DEFINE))
                defines.Remove(NODE_PREVIEW_DEFINE);
            else
                defines.Add(NODE_PREVIEW_DEFINE);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines.ToArray());
        }

        [MenuItem("Tools/CZToolKit/ET/Enable Node Preview", validate = true)]
        public static bool EnablePreviewValid()
        {
            var targetGroup = BuildTargetGroup.Standalone; // 选择目标平台
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';').ToList();
            Menu.SetChecked("Tools/CZToolKit/ET/Enable Node Preview", defines.Contains(NODE_PREVIEW_DEFINE));
            return true;
        }
    }
}