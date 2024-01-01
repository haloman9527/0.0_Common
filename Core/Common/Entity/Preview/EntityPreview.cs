using UnityEngine;

namespace CZToolKit.ET
{
    public class EntityPreview : MonoBehaviour
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HideReferenceObjectPicker]
        [Sirenix.OdinInspector.HideLabel]
#endif
        [SerializeReference]
        public Entity Component;
    }
}