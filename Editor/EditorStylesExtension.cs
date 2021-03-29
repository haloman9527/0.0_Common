 using UnityEngine;

namespace CZToolKit.Core.Editors
{
    public static class EditorStylesExtension
    {
        static GUIStyle transparent;
        public static GUIStyle Transparent
        {
            get
            {
                if (transparent == null)
                {
                    transparent = new GUIStyle();
                    transparent.normal.background = MakeTex(1, 1, new Color(1, 1, 1, 0));
                }
                return transparent;
            }
        }

        static Texture2D whiteTexture;
        public static Texture2D WhiteTexture
        {
            get
            {
                if (whiteTexture == null)
                    whiteTexture = MakeTex(1, 1, Color.white);
                return whiteTexture;
            }
        }

        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}