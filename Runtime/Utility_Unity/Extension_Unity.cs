using CZToolKit.Core;
using UnityEngine;

public static partial class Extension
{
    #region Vector
    public static bool IsZero(this Vector2 _self)
    {
        return _self == Vector2.zero;
    }

    /// <summary> 检查向量是否在允许范围内存在小幅度误差 </summary>
    public static bool IsExceeding(this Vector2 _self, float _magnitude)
    {
        // 允许百分之1的误差
        const float errorTolerance = 1.01f;
        return _self.sqrMagnitude > _magnitude * _magnitude * errorTolerance;
    }

    public static bool IsZero(this Vector2Int v2Int)
    {
        return v2Int == Vector2Int.zero;
    }

    /// <summary> 检查向量是否在允许范围内存在小幅度误差 </summary>
    public static bool IsExceeding(this Vector2Int _self, float _magnitude)
    {
        // 允许百分之1的误差
        const float errorTolerance = 1.01f;
        return _self.sqrMagnitude > _magnitude * _magnitude * errorTolerance;
    }

    public static bool IsZero(this Vector3 v3)
    {
        return v3 == Vector3.zero;
    }

    /// <summary> 检查向量是否在允许范围内存在小幅度误差 </summary>
    public static bool IsExceeding(this Vector3 _self, float _magnitude)
    {
        // 允许百分之1的误差
        const float errorTolerance = 1.01f;
        return _self.sqrMagnitude > _magnitude * _magnitude * errorTolerance;
    }

    public static bool IsZero(this Vector3Int v3Int)
    {
        return v3Int == Vector3Int.zero;
    }

    /// <summary> 检查向量是否在允许范围内存在小幅度误差 </summary>
    public static bool IsExceeding(this Vector3Int _self, float _magnitude)
    {
        // 允许百分之1的误差
        const float errorTolerance = 1.01f;
        return _self.sqrMagnitude > _magnitude * _magnitude * errorTolerance;
    }

    public static bool IsZero(this Vector4 _self)
    {
        return _self == Vector4.zero;
    }
    #endregion

    /// <summary> 获取CC的真实高度 </summary>
    public static float GetRealHeight(this CharacterController _self)
    {
        return Mathf.Max(_self.radius * 2, _self.height);
    }

    /// <summary> 获取CC顶部半圆中心 </summary>
    public static Vector3 GetTopCenter(this CharacterController _self)
    {
        return Vector3.down * _self.radius + Vector3.up * _self.GetRealHeight() / 2 + _self.center;
    }

    /// <summary> 获取CC底部半圆中心 </summary>
    public static Vector3 GetBottomCenter(this CharacterController _self)
    {
        return Vector3.up * _self.radius + Vector3.down * _self.GetRealHeight() / 2 + _self.center;
    }

    /// <summary> 获取颜色明度 </summary>
    public static float GetLuminance(this Color _color)
    {
        return 0.299f * _color.r + 0.587f * _color.g + 0.114f * _color.b;
    }

    public static string GetRelativePath(this Transform _self, Transform _root)
    {
        string path = _self.name;
        Transform trans = _self.parent;
        while (trans != null && trans != _root)
        {
            path = trans.name + "/" + path;
            trans = trans.parent;
        }
        return path;
    }

    public static Rect GetSide(this Rect _self, UIDirection _sideDirection, float _side, float _offset = 0)
    {
        switch (_sideDirection)
        {
            case UIDirection.MiddleCenter:
                return new Rect(_self.x + _side / 2, _self.y + _side / 2, _self.width - _side, _self.height - _side);
            case UIDirection.Top:
                return new Rect(_self.x + _side / 2, _self.y - _side / 2 + _offset, _self.width - _side, _side);
            case UIDirection.Bottom:
                return new Rect(_self.x + _side / 2, _self.y + _self.height - _side / 2 + _offset, _self.width - _side, _side);
            case UIDirection.Left:
                return new Rect(_self.x - _side / 2 + _offset, _self.y + _side / 2, _side, _self.height - _side);
            case UIDirection.Right:
                return new Rect(_self.x + _self.width - _side / 2 + _offset, _self.y + _side / 2, _side, _self.height - _side);
            case UIDirection.TopLeft:
                return new Rect(_self.x - _side / 2 + _offset, _self.y - _side / 2 + _offset, _side, _side);
            case UIDirection.TopRight:
                return new Rect(_self.x + _self.width - _side / 2 + _offset, _self.y - _side / 2 + _offset, _side, _side);
            case UIDirection.BottomLeft:
                return new Rect(_self.x - _side / 2 + _offset, _self.y + _self.height - _side / 2 + _offset, _side, _side);
            case UIDirection.BottomRight:
                return new Rect(_self.x + _self.width - _side / 2 + _offset, _self.y + _self.height - _side / 2 + _offset, _side, _side);
        }
        return new Rect();
    }
}