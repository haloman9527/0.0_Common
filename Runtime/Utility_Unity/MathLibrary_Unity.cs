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
        public static Vector2 Reverse(Vector2 _targetValue)
        {
            return new Vector2(-_targetValue.x, -_targetValue.y);
        }

        public static Vector2 AbsReverse(Vector2 _targetValue)
        {
            return new Vector2(-Mathf.Abs(_targetValue.x), -Mathf.Abs(_targetValue.y));
        }

        public static Vector2Int AbsReverse(Vector2Int _targetValue)
        {
            return new Vector2Int(-Mathf.Abs(_targetValue.x), -Mathf.Abs(_targetValue.y));
        }

        public static Vector3 AbsReverse(Vector3 _targetValue)
        {
            return new Vector3(-Mathf.Abs(_targetValue.x), -Mathf.Abs(_targetValue.y), -Mathf.Abs(_targetValue.z));
        }

        public static Vector3Int AbsReverse(Vector3Int _targetValue)
        {
            return new Vector3Int(-Mathf.Abs(_targetValue.x), -Mathf.Abs(_targetValue.y), -Mathf.Abs(_targetValue.z));
        }

        public static Vector4 AbsReverse(Vector4 _targetValue)
        {
            return new Vector4(-Mathf.Abs(_targetValue.x), -Mathf.Abs(_targetValue.y), -Mathf.Abs(_targetValue.z), -Mathf.Abs(_targetValue.w));
        }

        public static Rect Scale(Rect _targetValue, float _scale, Vector2 _pivot)
        {
            Vector2 absPosition = _targetValue.position + _targetValue.size * _pivot;
            _targetValue.size *= _scale;
            _targetValue.position = absPosition - _targetValue.size * _pivot;
            return _targetValue;
        }
    }
}
