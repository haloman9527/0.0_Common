using CZToolKit.Core;
using UnityEngine;

public static partial class Extension
{
    public static bool IsZero(this Vector2 v2)
    {
        return v2 == Vector2.zero;
    }

    public static bool IsZero(this Vector2Int v2Int)
    {
        return v2Int == Vector2Int.zero;
    }

    public static bool IsZero(this Vector3 v3)
    {
        return v3 == Vector3.zero;
    }

    public static bool IsZero(this Vector3Int v3Int)
    {
        return v3Int == Vector3Int.zero;
    }

    public static bool IsZero(this Vector4 v4)
    {
        return v4 == Vector4.zero;
    }

    /// <summary> 获取CC的真实高度 </summary>
    public static float GetRealHeight(this CharacterController characterController)
    {
        return Mathf.Max(characterController.radius * 2, characterController.height);
    }

    /// <summary> 获取CC顶部半圆中心 </summary>
    public static Vector3 GetTopCenter(this CharacterController characterController)
    {
        return Vector3.down * characterController.radius + Vector3.up * characterController.GetRealHeight() / 2 + characterController.center;
    }

    /// <summary> 获取CC底部半圆中心 </summary>
    public static Vector3 GetBottomCenter(this CharacterController characterController)
    {
        return Vector3.up * characterController.radius + Vector3.down * characterController.GetRealHeight() / 2 + characterController.center;
    }

    /// <summary> 获取颜色明度 </summary>
    public static float GetLuminance(this Color _color)
    {
        return 0.299f * _color.r + 0.587f * _color.g + 0.114f * _color.b;
    }

    public static string GetRelativePath(this Transform _transform, Transform _parent)
    {
        string path = _transform.name;
        Transform trans = _transform.parent;
        while (trans != null && trans != _parent)
        {
            path = trans.name + "/" + path;
            trans = trans.parent;
        }
        return path;
    }

    public static Rect GetSide(this Rect _rect, UIDirection _sideDirection, float _side, float _offset = 0)
    {
        switch (_sideDirection)
        {
            case UIDirection.MiddleCenter:
                return new Rect(_rect.x + _side / 2, _rect.y + _side / 2, _rect.width - _side, _rect.height - _side);
            case UIDirection.Top:
                return new Rect(_rect.x + _side / 2, _rect.y - _side / 2 + _offset, _rect.width - _side, _side);
            case UIDirection.Bottom:
                return new Rect(_rect.x + _side / 2, _rect.y + _rect.height - _side / 2 + _offset, _rect.width - _side, _side);
            case UIDirection.Left:
                return new Rect(_rect.x - _side / 2 + _offset, _rect.y + _side / 2, _side, _rect.height - _side);
            case UIDirection.Right:
                return new Rect(_rect.x + _rect.width - _side / 2 + _offset, _rect.y + _side / 2, _side, _rect.height - _side);
            case UIDirection.TopLeft:
                return new Rect(_rect.x - _side / 2 + _offset, _rect.y - _side / 2 + _offset, _side, _side);
            case UIDirection.TopRight:
                return new Rect(_rect.x + _rect.width - _side / 2 + _offset, _rect.y - _side / 2 + _offset, _side, _side);
            case UIDirection.BottomLeft:
                return new Rect(_rect.x - _side / 2 + _offset, _rect.y + _rect.height - _side / 2 + _offset, _side, _side);
            case UIDirection.BottomRight:
                return new Rect(_rect.x + _rect.width - _side / 2 + _offset, _rect.y + _rect.height - _side / 2 + _offset, _side, _side);
        }
        return new Rect();
    }
}