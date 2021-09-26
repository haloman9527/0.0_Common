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

        Stack<ICommand> undo = new Stack<ICommand>();
        Stack<ICommand> redo = new Stack<ICommand>();

        int groupIndent = 0;

        public void BeginGroup()
        {
            groupIndent++;
            if (groupIndent == 1)
                undo.Push(new CommandsGroup());
        }

        public void EndGroup()
        {
            groupIndent--;
        }

        public void Do(ICommand command)
        {
            command.Do();
            redo.Clear();
            if (groupIndent > 0)
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
    }
}
