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

using UnityEngine;

namespace CZToolKit.Core
{
    public static partial class MathLibrary
    {
        public static Vector2 Reverse(Vector2 value)
        {
            return new Vector2(-value.x, -value.y);
        }

        public static Vector2 AbsReverse(Vector2 value)
        {
            return new Vector2(-Mathf.Abs(value.x), -Mathf.Abs(value.y));
        }

        public static Vector2Int AbsReverse(Vector2Int value)
        {
            return new Vector2Int(-Mathf.Abs(value.x), -Mathf.Abs(value.y));
        }

        public static Vector3 AbsReverse(Vector3 value)
        {
            return new Vector3(-Mathf.Abs(value.x), -Mathf.Abs(value.y), -Mathf.Abs(value.z));
        }

        public static Vector3Int AbsReverse(Vector3Int value)
        {
            return new Vector3Int(-Mathf.Abs(value.x), -Mathf.Abs(value.y), -Mathf.Abs(value.z));
        }

        public static Vector4 AbsReverse(Vector4 value)
        {
            return new Vector4(-Mathf.Abs(value.x), -Mathf.Abs(value.y), -Mathf.Abs(value.z), -Mathf.Abs(value.w));
        }

        public static Rect Scale(Rect value, float scale, Vector2 pivot)
        {
            Vector2 absPosition = value.position + value.size * pivot;
            value.size *= scale;
            value.position = absPosition - value.size * pivot;
            return value;
        }
    }
}
