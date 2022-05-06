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
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
using System;
using System.Collections.Generic;

namespace CZToolKit.Core
{
    public interface ICommand
    {
        void Do();
        void Undo();
    }

    public class CommandDispatcher
    {
        Stack<ICommand> undo = new Stack<ICommand>();
        Stack<ICommand> redo = new Stack<ICommand>();

        bool isGrouping = false;

        public void BeginGroup()
        {
            if (isGrouping)
                return;
            undo.Push(new CommandsGroup());
            isGrouping = true;
        }

        public void EndGroup()
        {
            if (!isGrouping)
                throw new System.Exception($"在结束一个组之前需要当前存在一个组{nameof(BeginGroup)}");
            isGrouping = false;
        }

        public void Do(Action @do, Action @undo)
        {
            Do(new ActionCommand(@do, @undo));
        }

        public void Do(ICommand command)
        {
            command.Do();
            redo.Clear();
            if (isGrouping)
            {
                CommandsGroup group = undo.Peek() as CommandsGroup;
                group.undo.Push(command);
            }
            else
                undo.Push(command);
        }

        public virtual void Redo()
        {
            if (redo.Count == 0)
                return;
            ICommand command = redo.Pop();
            if (command != null)
            {
                command.Do();
            }
            undo.Push(command);
        }

        public virtual void Undo()
        {
            if (undo.Count == 0)
                return;
            ICommand command = undo.Pop();
            if (command != null)
                command.Undo();
            redo.Push(command);
        }

        public virtual void Clear()
        {
            undo.Clear();
            redo.Clear();
        }

        internal class CommandsGroup : ICommand
        {
            internal Stack<ICommand> undo = new Stack<ICommand>();
            internal Stack<ICommand> redo = new Stack<ICommand>();

            public void Do()
            {
                while (redo.Count != 0)
                {
                    ICommand command = redo.Pop();
                    if (command != null)
                        command.Do();
                    undo.Push(command);
                }
            }

            public void Undo()
            {
                while (undo.Count != 0)
                {
                    ICommand command = undo.Pop();
                    if (command != null)
                        command.Undo();
                    redo.Push(command);
                }
            }
        }

        internal class ActionCommand : ICommand
        {
            Action @do, @undo;

            public ActionCommand(Action @do, Action @undo)
            {
                this.@do = @do;
                this.@undo = @undo;
            }

            public void Do()
            {
                @do?.Invoke();
            }

            public void Undo()
            {
                @undo?.Invoke();
            }
        }
    }
}
