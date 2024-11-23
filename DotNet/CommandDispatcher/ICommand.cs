#region 注 释

/***
 *
 *  Title:
 *      主题: 指令调度器
 *  Description:
 *      功能: 
 *          提供Do Redo Undo的功能
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/haloman9527
 *  Blog: https://www.haloman.net/
 *
 */

#endregion

namespace Jiange
{
    public interface ICommand
    {
        void Do();
        
        void Redo();
        
        void Undo();
    }
}