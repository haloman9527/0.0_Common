using System.Linq;
using UnityEditor;

namespace CZToolKit.Editors
{
    public class Menus
    {
        public const string ENTITY_PREVIEW_DEFINE = "ENTITY_PREVIEW";

        [MenuItem("Tools/CZToolKit/ET/Enable Entity Preview")]
        public static void SwitchPreview()
        {
            var targetGroup = BuildTargetGroup.Standalone; // 选择目标平台
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';').ToList();
            if (defines.Contains(ENTITY_PREVIEW_DEFINE))
                defines.Remove(ENTITY_PREVIEW_DEFINE);
            else
                defines.Add(ENTITY_PREVIEW_DEFINE);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines.ToArray());
        }

        [MenuItem("Tools/CZToolKit/ET/Enable Entity Preview", validate = true)]
        public static bool EnablePreviewValid()
        {
            var targetGroup = BuildTargetGroup.Standalone; // 选择目标平台
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';').ToList();
            Menu.SetChecked("Tools/CZToolKit/ET/Enable Entity Preview", defines.Contains(ENTITY_PREVIEW_DEFINE));
            return true;
        }
    }
}