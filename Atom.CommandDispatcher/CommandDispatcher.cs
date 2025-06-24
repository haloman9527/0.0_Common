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
    public sealed class CommandDispatcher
    {
        private LinkedList<ICommand> m_UndoList = new LinkedList<ICommand>();
        private Stack<ICommand> m_RedoList = new Stack<ICommand>();
        private CommandGroup m_CurrentGroup;
        private int m_RecordLimit;

        public CommandDispatcher(int recordLimit = 0)
        {
            this.m_RecordLimit = recordLimit;
        }

        public bool CanUndo()
        {
            return m_UndoList.Count > 0;
        }

        public bool CanRedo()
        {
            return m_RedoList.Count > 0;
        }

        public void BeginGroup()
        {
            if (m_CurrentGroup != null)
            {
                throw new Exception("Current is already in a group");
            }

            m_CurrentGroup = new CommandGroup();
            m_UndoList.AddLast(m_CurrentGroup);
            while (m_RecordLimit >= 0 && m_UndoList.Count > m_RecordLimit)
            {
                m_UndoList.RemoveFirst();
            }

            if (m_UndoList.Count == 0)
            {
                m_CurrentGroup = null;
            }
        }

        public void EndGroup()
        {
            if (m_CurrentGroup == null)
            {
                throw new Exception("Current is not in a group");
            }

            m_CurrentGroup = null;
        }

        public void Do(Action @do, Action @undo)
        {
            var command = new ActionCommand(@do, @do, @undo);
            Register(command);
            command.Do();
        }

        public void Do(ICommand command)
        {
            Register(command);
            command.Do();
        }

        public void Register(ICommand command)
        {
            if (command == null)
                return;

            m_RedoList.Clear();
            if (m_CurrentGroup != null)
            {
                m_CurrentGroup.m_Commands.Add(command);
            }
            else
            {
                m_UndoList.AddLast(command);
                while (m_RecordLimit > 0 && m_UndoList.Count > m_RecordLimit)
                {
                    m_UndoList.RemoveFirst();
                }
            }
        }

        public void Redo()
        {
            if (m_RedoList.Count == 0)
            {
                return;
            }

            var command = m_RedoList.Pop();
            m_UndoList.AddLast(command);

            if (command != null)
            {
                command.Redo();
            }
        }

        public void Undo()
        {
            if (m_UndoList.Count == 0)
            {
                return;
            }

            var command = m_UndoList.Last.Value;
            m_UndoList.RemoveLast();
            m_RedoList.Push(command);

            if (command != null)
            {
                command.Undo();
            }
        }

        public void Clear()
        {
            m_UndoList.Clear();
            m_RedoList.Clear();
        }

        internal class CommandGroup : ICommand
        {
            internal List<ICommand> m_Commands = new List<ICommand>();

            public void Do()
            {
                for (int i = 0; i < m_Commands.Count; i++)
                {
                    var command = m_Commands[i];
                    if (command == null)
                        continue;

                    command.Do();
                }
            }

            public void Redo()
            {
                for (int i = 0; i < m_Commands.Count; i++)
                {
                    var command = m_Commands[i];
                    if (command == null)
                        continue;

                    command.Redo();
                }
            }

            public void Undo()
            {
                for (int i = m_Commands.Count - 1; i >= 0; i--)
                {
                    var command = m_Commands[i];
                    if (command == null)
                        continue;

                    command.Undo();
                }
            }
        }

        public class ActionCommand : ICommand
        {
            private Action m_Do;
            private Action m_Redo;
            private Action m_Undo;

            public ActionCommand(Action @do, Action @redo, Action @undo)
            {
                this.m_Do = @do;
                this.m_Redo = @redo;
                this.m_Undo = @undo;
            }

            public void Do()
            {
                m_Do?.Invoke();
            }

            public void Redo()
            {
                m_Redo?.Invoke();
            }

            public void Undo()
            {
                m_Undo?.Invoke();
            }
        }
    }
}