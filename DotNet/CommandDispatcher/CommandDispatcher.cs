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

namespace CZToolKit
{
    public class CommandDispatcher
    {
        private LinkedList<ICommand> undo = new LinkedList<ICommand>();
        private Stack<ICommand> redo = new Stack<ICommand>();
        private CommandGroup group;
        private int recordLimit;

        public CommandDispatcher()
        {
            recordLimit = -1;
        }

        public CommandDispatcher(int recordLimit)
        {
            this.recordLimit = recordLimit;
        }

        public void BeginGroup()
        {
            group = new CommandGroup();
            undo.AddLast(group);
            while (recordLimit >= 0 && undo.Count > recordLimit)
            {
                undo.RemoveFirst();
            }

            if (undo.Count == 0)
            {
                group = null;
            }
        }

        public void EndGroup()
        {
            group = null;
        }

        public void Do(Action @do, Action @undo)
        {
            Do(new ActionCommand(@do, @do, @undo));
        }

        public void Do(ICommand command)
        {
            redo.Clear();
            if (group != null)
            {
                group.commands.Add(command);
            }
            else
            {
                undo.AddLast(command);
                while (recordLimit >= 0 && undo.Count > recordLimit)
                {
                    undo.RemoveFirst();
                }
            }

            if (command != null)
            {
                command.Do();
            }
        }

        public virtual void Redo()
        {
            if (redo.Count == 0)
                return;
            var command = redo.Pop();
            undo.AddLast(command);
            while (recordLimit >= 0 && undo.Count > recordLimit)
            {
                undo.RemoveFirst();
            }

            if (command != null)
            {
                command.Redo();
            }
        }

        public virtual void Undo()
        {
            if (undo.Count == 0)
                return;
            var command = undo.Last.Value;
            undo.RemoveLast();
            redo.Push(command);

            if (command != null)
            {
                command.Undo();
            }
        }

        public virtual void Clear()
        {
            undo.Clear();
            redo.Clear();
        }

        internal class CommandGroup : ICommand
        {
            internal List<ICommand> commands = new List<ICommand>();

            public void Do()
            {
                for (int i = 0; i < commands.Count; i++)
                {
                    var command = commands[i];
                    if (command == null)
                        continue;

                    command.Do();
                }
            }

            public void Redo()
            {
                for (int i = 0; i < commands.Count; i++)
                {
                    var command = commands[i];
                    if (command == null)
                        continue;

                    command.Redo();
                }
            }

            public void Undo()
            {
                for (int i = commands.Count - 1; i >= 0; i--)
                {
                    var command = commands[i];
                    if (command == null)
                        continue;

                    command.Undo();
                }
            }
        }

        internal class ActionCommand : ICommand
        {
            Action @do, @redo, @undo;

            public ActionCommand(Action @do, Action @redo, Action @undo)
            {
                this.@do = @do;
                this.@redo = @redo;
                this.@undo = @undo;
            }

            public void Do()
            {
                @do?.Invoke();
            }

            public void Redo()
            {
                @redo?.Invoke();
            }

            public void Undo()
            {
                @undo?.Invoke();
            }
        }
    }
}