using UnityEngine;

namespace CZToolKit.ET
{
    public class EntityPreviewRoot : MonoBehaviour
    {
        private static EntityPreviewRoot s_Instance;

        public static EntityPreviewRoot Instance
        {
            get { return s_Instance; }
        }

        private void Awake()
        {
            if (s_Instance != null)
                return;
            
            s_Instance = this;
        }
    }
}