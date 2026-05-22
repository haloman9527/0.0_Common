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

using System;
using System.Collections.Generic;

namespace Atom
{
    public interface ICommandService
    {
        int UndoCount { get; }

        int RedoCount { get; }
        
        bool CanUndo();

        bool CanRedo();

        void BeginGroup();

        void EndGroup();

        CommandResult Execute(ICommand command);

        CommandResult Execute(Action @do, Action @undo, string description = "Custom Action Command");

        CommandResult Redo();

        CommandResult Undo();

        void Clear();

        IEnumerable<string> GetUndoDescriptions();

        IEnumerable<string> GetRedoDescriptions();
    }
}