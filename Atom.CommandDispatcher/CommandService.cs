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
using System.Linq;

namespace Atom
{
    public sealed class CommandService : ICommandService
    {
        private object               m_Lock          = new object();
        private LinkedList<ICommand> m_UndoList      = new LinkedList<ICommand>();
        private Stack<ICommand>      m_RedoList      = new Stack<ICommand>();
        private CommandGroup         m_CurrentGroup  = null;
        private int                  m_RecordLimit   = 100;

        public CommandService(int recordLimit = 0)
        {
            this.m_RecordLimit = recordLimit;
        }

        private void Register(ICommand command)
        {
            m_RedoList.Clear();
            m_UndoList.AddLast(command);
            while (m_RecordLimit > 0 && m_UndoList.Count > m_RecordLimit)
            {
                m_UndoList.RemoveFirst();
            }
        }

        public int UndoCount => m_UndoList.Count;

        public int RedoCount => m_RedoList.Count;

        public bool CanUndo() => m_UndoList.Count > 0;

        public bool CanRedo() => m_RedoList.Count > 0;

        public void BeginGroup()
        {
            if (m_CurrentGroup != null)
                throw new Exception("Current is already in a group");

            lock (m_Lock)
            {
                m_CurrentGroup = new CommandGroup();
            }
        }

        public void EndGroup()
        {
            if (m_CurrentGroup == null)
                throw new Exception("Current is not in a group");

            lock (m_Lock)
            {
                if (m_CurrentGroup.CommandCount != 0)
                    Register(m_CurrentGroup);
                m_CurrentGroup = null;
            }
        }

        public CommandResult Execute(ICommand command)
        {
            CommandResult result;
            lock (m_Lock)
            {
                try
                {
                    result = command.Execute();
                    if (result.IsSuccess)
                    {
                        if (m_CurrentGroup != null)
                            m_CurrentGroup.Add(command);
                        else
                            Register(command);
                    }
                }
                catch (Exception e)
                {
                    result = CommandResult.Failure(new CommandIssue(CommandIssueSeverity.Error, e.Message));
                }
            }

            return result;
        }

        public CommandResult Execute(Action @do, Action @undo, string description = "Custom Action Command")
        {
            return Execute(new ActionCommand(@do, undo, description));
        }

        public CommandResult Undo()
        {
            CommandResult result;
            lock (m_Lock)
            {
                if (m_UndoList.Count == 0)
                    return CommandResult.Success();
                
                var last = m_UndoList.Last;
                if (last == null)
                    return CommandResult.Success();

                var command = last.Value;
                try
                {
                    result = command.Undo();
                    if (result.IsSuccess)
                    {
                        m_UndoList.RemoveLast();
                        m_RedoList.Push(command);
                    }
                }
                catch (Exception e)
                {
                    result = CommandResult.Failure(new CommandIssue(CommandIssueSeverity.Error, e.Message));
                }
            }

            return result;
        }

        public CommandResult Redo()
        {
            CommandResult result;
            lock (m_Lock)
            {
                if (m_RedoList.Count == 0)
                    return CommandResult.Success();

                var command = m_RedoList.Peek();
                try
                {
                    result = command.Execute();
                    if (result.IsSuccess)
                    {
                        m_RedoList.Pop();
                        m_UndoList.AddLast(command);
                    }
                }
                catch (Exception e)
                {
                    result = CommandResult.Failure(new CommandIssue(CommandIssueSeverity.Error, e.Message));
                }
            }

            return result;
        }

        public void Clear()
        {
            lock (m_Lock)
            {
                m_UndoList.Clear();
                m_RedoList.Clear();
            }
        }

        public IEnumerable<string> GetUndoDescriptions()
        {
            foreach (var command in m_UndoList)
            {
                yield return command.Description;
            }
        }

        public IEnumerable<string> GetRedoDescriptions()
        {
            foreach (var command in m_RedoList)
            {
                yield return command.Description;
            }
        }
        

        public class CombinedCommands : ICommand
        {
            protected List<ICommand> m_Commands = new List<ICommand>();
            
            public int CommandCount => m_Commands.Count;
            
            public virtual string Description => "Combined Commands";

            public CombinedCommands Add(ICommand command)
            {
                m_Commands.Add(command);
                return this;
            }

            CommandResult ICommand.Execute()
            {
                for (int i = 0; i < m_Commands.Count; i++)
                {
                    var command = m_Commands[i];
                    if (command == null)
                        continue;

                    CommandResult commandResult;
                    try
                    {
                        commandResult = command.Execute();
                    }
                    catch (Exception e)
                    {
                        commandResult = CommandResult.Failure(new CommandIssue(CommandIssueSeverity.Error, e.Message));
                    }

                    if (!commandResult.IsSuccess)
                    {
                        for (int j = i - 1; j >= 0; j--)
                        {
                            try
                            {
                                m_Commands[j].Undo();
                            }
                            catch (Exception e)
                            {
                                // ignore
                            }
                        }
                        return commandResult;
                    }
                }
                
                return CommandResult.Success();
            }

            CommandResult ICommand.Undo()
            {
                for (int i = m_Commands.Count - 1; i >= 0; i--)
                {
                    var command = m_Commands[i];
                    if (command == null)
                        continue;

                    CommandResult commandResult;
                    try
                    {
                        commandResult = command.Undo();
                    }
                    catch (Exception e)
                    {
                        commandResult = CommandResult.Failure(new CommandIssue(CommandIssueSeverity.Error, e.Message));
                    }

                    if (!commandResult.IsSuccess)
                    {
                        for (int j = i + 1; j < m_Commands.Count; j++)
                        {
                            try
                            {
                                m_Commands[j].Execute();
                            }
                            catch (Exception e)
                            {
                                // ignore
                            }
                        }
                        return commandResult;
                    }
                }
                    
                return CommandResult.Success();
            }
        }

        private class CommandGroup : CombinedCommands
        {
            public override string Description => "CommandGroup";
        }

        public class ActionCommand : ICommand
        {
            private Action m_Do;
            private Action m_Undo;

            public ActionCommand(Action @do, Action @undo, string description = "Custom Action Command")
            {
                this.m_Do = @do;
                this.m_Undo = @undo;
                this.Description = description;
            }

            public string Description { get; }

            CommandResult ICommand.Execute()
            {
                try
                {
                    m_Do?.Invoke();
                    return CommandResult.Success();
                }
                catch (Exception e)
                {
                    return CommandResult.Failure(new CommandIssue(CommandIssueSeverity.Error, e.Message));
                }
            }

            CommandResult ICommand.Undo()
            {
                try
                {
                    m_Undo?.Invoke();
                    return CommandResult.Success();
                }
                catch (Exception e)
                {
                    return CommandResult.Failure(new CommandIssue(CommandIssueSeverity.Error, e.Message));
                }
            }
        }
    }
}