#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CZToolKit.Core.Editors
{
    public static class EditorStylesExtension
    {
        static GUIStyle transparent;
        /// <summary> 完全透明的Style </summary>
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

        static GUIStyle labelStyle;
        public static GUIStyle LabelStyle
        {
            get
            {
                if (labelStyle == null)
                {
                    labelStyle = new GUIStyle(EditorStyles.label);
                    labelStyle.richText = true;
                }
                return labelStyle;
            }
        }


        static GUIStyle foldoutStyle;
        public static GUIStyle FoldoutStyle
        {
            get
            {
                if (foldoutStyle == null)
                {
                    foldoutStyle = new GUIStyle(EditorStyles.foldout);
                    foldoutStyle.richText = true;
                }
                return foldoutStyle;
            }
        }

        static GUIStyle textFieldStyle;
        public static GUIStyle TextFieldStyle
        {
            get
            {
                if (textFieldStyle == null)
                {
                    textFieldStyle = new GUIStyle(EditorStyles.textField);
                    textFieldStyle.richText = true;
                }
                return textFieldStyle;
            }
        }

        static GUIStyle numberFieldStyle;
        public static GUIStyle NumberFieldStyle
        {
            get
            {
                if (numberFieldStyle == null)
                {
                    numberFieldStyle = new GUIStyle(EditorStyles.numberField);
                    numberFieldStyle.richText = true;
                }
                return numberFieldStyle;
            }
        }

        static GUIStyle middleLabelStyle;
        /// <summary> 居中Label </summary>
        public static GUIStyle MiddleLabelStyle
        {
            get
            {
                if (middleLabelStyle == null)
                {
                    middleLabelStyle = new GUIStyle(EditorStyles.label);
                    middleLabelStyle.alignment = TextAnchor.MiddleCenter;
                }
                return middleLabelStyle;
            }
        }

        static GUIStyle leftLabelStyle;
        /// <summary> 左对齐Label </summary>
        public static GUIStyle LeftLabelStyle
        {
            get
            {
                if (leftLabelStyle == null)
                {
                    leftLabelStyle = new GUIStyle(EditorStyles.label);
                    leftLabelStyle.alignment = TextAnchor.MiddleLeft;
                }
                return leftLabelStyle;
            }
        }

        static GUIStyle rightLabelStyle;
        /// <summary> 右对齐Label </summary>
        public static GUIStyle RightLabelStyle
        {
            get
            {
                if (rightLabelStyle == null)
                {
                    rightLabelStyle = new GUIStyle(EditorStyles.label);
                    rightLabelStyle.alignment = TextAnchor.MiddleRight;
                }
                return rightLabelStyle;
            }
        }

        static GUIStyle onlyIconButtonStyle;
        /// <summary> 无背景无边框，Button样式 </summary>
        public static GUIStyle OnlyIconButtonStyle
        {
            get
            {
                if (onlyIconButtonStyle == null)
                {
                    onlyIconButtonStyle = new GUIStyle(GUI.skin.button);
                    onlyIconButtonStyle.normal.background = AlphaTexture;
                    onlyIconButtonStyle.padding.left = 0;
                    onlyIconButtonStyle.padding.right = 0;
                    onlyIconButtonStyle.padding.bottom = 0;
                    onlyIconButtonStyle.padding.top = 0;
                }
                return onlyIconButtonStyle;
            }
        }

        static GUIStyle roundedBoxStyle;
        public static GUIStyle RoundedBoxStyle
        {
            get
            {
                if (roundedBoxStyle == null)
                {
                    roundedBoxStyle = new GUIStyle((GUIStyle)"FrameBox");
                }
                return roundedBoxStyle;
            }
        }

        static Texture2D whiteTexture;
        /// <summary> 白色Texture(1,1) </summary>
        public static Texture2D WhiteTexture
        {
            get
            {
                if (whiteTexture == null)
                    whiteTexture = MakeTex(1, 1, Color.white);
                return whiteTexture;
            }
        }


        static Texture2D alphaTexture;
        /// <summary> 透明Texture(1,1) </summary>
        public static Texture2D AlphaTexture
        {
            get
            {
                if (alphaTexture == null)
                    alphaTexture = MakeTex(1, 1, new Color(0, 0, 0, 0));
                return alphaTexture;
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
#endif