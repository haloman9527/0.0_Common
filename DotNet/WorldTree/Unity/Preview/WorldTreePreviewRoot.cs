using UnityEngine;

namespace CZToolKit
{
    public class WorldTreePreviewRoot : MonoBehaviour
    {
        private static WorldTreePreviewRoot s_Instance;

        public static WorldTreePreviewRoot Instance
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