#region 注 释

/***
 *
 *  Title:
 *      主题: 单例基类
 *  Description:
 *      功能: 自动创建单例对象
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */

#endregion

using System;

namespace CZToolKit.Common.Singletons
{
    public interface ISingleton : IDisposable
    {
        void Register();
        void Destroy();
        bool IsDisposed();
    }

    public interface ISingletonAwake
    {
        void Awake();
    }

    public interface ISingletonFixedUpdate
    {
        void FixedUpdate();
    }

    public interface ISingletonUpdate
    {
        void Update();
    }

    public interface ISingletonLateUpdate
    {
        void LateUpdate();
    }
}