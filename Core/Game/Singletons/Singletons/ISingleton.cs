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
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */

#endregion

using System;

namespace Moyo
{
    public interface ISingleton : IDisposable
    {
        bool IsDisposed { get; }
        
        void Register();
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

    public interface ISingletonDestory
    {
        void Destroy();
    }
}