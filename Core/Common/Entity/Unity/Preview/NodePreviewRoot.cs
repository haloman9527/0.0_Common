using UnityEngine;

namespace CZToolKit
{
    public class NodePreviewRoot : MonoBehaviour
    {
        private static NodePreviewRoot s_Instance;

        public static NodePreviewRoot Instance
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