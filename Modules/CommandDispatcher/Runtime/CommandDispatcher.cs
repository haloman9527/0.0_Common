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

        int groupLevel = 0;

        public void BeginGroup()
        {
            groupLevel++;
            if (groupLevel == 1)
                undo.Push(new CommandsGroup());
        }

        public void EndGroup()
        {
            groupLevel--;
            if (groupLevel < 0)
                throw new System.Exception($"{nameof(CommandDispatcher)}的{nameof(BeginGroup)}{nameof(EndGroup)}数量不一致");
        }

        public void Do(Action @do, Action @undo)
        {
            Do(new ActionCommand(@do, @undo));
        }

        public void Do(ICommand command)
        {
            command.Do();
            redo.Clear();
            if (groupLevel > 0)
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
