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
using CZToolKit.Core;
using UnityEngine;

public static partial class ExtMethods
{
    #region Vector
    public static bool IsZero(this Vector2 self)
    {
        return self == Vector2.zero;
    }

    /// <summary> 检查向量是否在允许范围内存在小幅度误差 </summary>
    public static bool IsExceeding(this Vector2 self, float magnitude)
    {
        // 允许百分之1的误差
        const float errorTolerance = 1.01f;
        return self.sqrMagnitude > magnitude * magnitude * errorTolerance;
    }

    public static bool IsZero(this Vector2Int self)
    {
        return self == Vector2Int.zero;
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
    public static float GetRealHeight(this CharacterController self)
    {
        return Mathf.Max(self.radius * 2, self.height);
    }

    /// <summary> 获取CC顶部半圆中心 </summary>
    public static Vector3 GetTopCenter(this CharacterController self)
    {
        return Vector3.down * self.radius + Vector3.up * self.GetRealHeight() / 2 + self.center;
    }

    /// <summary> 获取CC底部半圆中心 </summary>
    public static Vector3 GetBottomCenter(this CharacterController self)
    {
        return Vector3.up * self.radius + Vector3.down * self.GetRealHeight() / 2 + self.center;
    }

    /// <summary> 获取Capsule的真实高度 </summary>
    public static float GetRealHeight(this CapsuleCollider self)
    {
        return Mathf.Max(self.radius * 2, self.height);
    }

    /// <summary> 获取Capsule顶部半圆中心 </summary>
    public static Vector3 GetTopCenter(this CapsuleCollider self)
    {
        return Vector3.down * self.radius + Vector3.up * self.GetRealHeight() / 2 + self.center;
    }

    /// <summary> 获取CC底部半圆中心 </summary>
    public static Vector3 GetBottomCenter(this CapsuleCollider self)
    {
        return Vector3.up * self.radius + Vector3.down * self.GetRealHeight() / 2 + self.center;
    }

    /// <summary> 获取颜色明度 </summary>
    public static float GetLuminance(this Color color)
    {
        return 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
    }

    public static string GetRelativePath(this Transform self, Transform root)
    {
        string path = self.name;
        Transform trans = self.parent;
        while (trans != null && trans != root)
        {
            path = trans.name + "/" + path;
            trans = trans.parent;
        }
        return path;
    }

    public static Rect GetSide(this Rect self, UIDirection sideDirection, float sideWidth, float offset = 0)
    {
        switch (sideDirection)
        {
            case UIDirection.MiddleCenter:
                return new Rect(self.x + sideWidth / 2, self.y + sideWidth / 2, self.width - sideWidth, self.height - sideWidth);
            case UIDirection.Top:
                return new Rect(self.x + sideWidth / 2, self.y - sideWidth / 2 + offset, self.width - sideWidth, sideWidth);
            case UIDirection.Bottom:
                return new Rect(self.x + sideWidth / 2, self.y + self.height - sideWidth / 2 + offset, self.width - sideWidth, sideWidth);
            case UIDirection.Left:
                return new Rect(self.x - sideWidth / 2 + offset, self.y + sideWidth / 2, sideWidth, self.height - sideWidth);
            case UIDirection.Right:
                return new Rect(self.x + self.width - sideWidth / 2 + offset, self.y + sideWidth / 2, sideWidth, self.height - sideWidth);
            case UIDirection.TopLeft:
                return new Rect(self.x - sideWidth / 2 + offset, self.y - sideWidth / 2 + offset, sideWidth, sideWidth);
            case UIDirection.TopRight:
                return new Rect(self.x + self.width - sideWidth / 2 + offset, self.y - sideWidth / 2 + offset, sideWidth, sideWidth);
            case UIDirection.BottomLeft:
                return new Rect(self.x - sideWidth / 2 + offset, self.y + self.height - sideWidth / 2 + offset, sideWidth, sideWidth);
            case UIDirection.BottomRight:
                return new Rect(self.x + self.width - sideWidth / 2 + offset, self.y + self.height - sideWidth / 2 + offset, sideWidth, sideWidth);
        }
        return new Rect();
    }
}