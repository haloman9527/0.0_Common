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
 *  Blog: https://www.mindgear.net/
 *
 */

#endregion

using System;

namespace CZToolKit.Singletons
{
    public interface ISingleton : IDisposable
    {
        void Register();
        
        bool IsDisposed();
    }

    public interface ISingletonAwake
    {
        void Awake();
    }

    public interface ISingletonUpdate
    {
        void Update();
    }

    public interface ISingletonLateUpdate
    {
        void LateUpdate();
    }

    public interface ISingletonDestory
    {
        void Destroy();
    }
}