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
    public class CommandDispatcher
    {
        private LinkedList<ICommand> undoList = new LinkedList<ICommand>();
        private Stack<ICommand> redoList = new Stack<ICommand>();
        private CommandGroup group;
        private int recordLimit;

        public CommandDispatcher(int recordLimit = 0)
        {
            this.recordLimit = recordLimit;
        }

        public void BeginGroup()
        {
            if (group != null)
            {
                throw new Exception("Current is already in a group");
            }
            
            group = new CommandGroup();
            undoList.AddLast(group);
            while (recordLimit >= 0 && undoList.Count > recordLimit)
            {
                undoList.RemoveFirst();
            }

            if (undoList.Count == 0)
            {
                group = null;
            }
        }

        public void EndGroup()
        {
            if (group == null)
            {
                throw new Exception("Current is not in a group");
            }
            
            group = null;
        }

        public void Do(Action @do, Action @undo)
        {
            Do(new ActionCommand(@do, @do, @undo));
        }

        public void Do(ICommand command)
        {
            redoList.Clear();
            if (group != null)
            {
                group.commands.Add(command);
            }
            else
            {
                undoList.AddLast(command);
                while (recordLimit > 0 && undoList.Count > recordLimit)
                {
                    undoList.RemoveFirst();
                }
            }

            if (command != null)
            {
                command.Do();
            }
        }

        public virtual void Redo()
        {
            if (redoList.Count == 0)
                return;
            var command = redoList.Pop();
            undoList.AddLast(command);
            while (recordLimit >= 0 && undoList.Count > recordLimit)
            {
                undoList.RemoveFirst();
            }

            if (command != null)
            {
                command.Redo();
            }
        }

        public virtual void Undo()
        {
            if (undoList.Count == 0)
                return;
            var command = undoList.Last.Value;
            undoList.RemoveLast();
            redoList.Push(command);

            if (command != null)
            {
                command.Undo();
            }
        }

        public virtual void Clear()
        {
            undoList.Clear();
            redoList.Clear();
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