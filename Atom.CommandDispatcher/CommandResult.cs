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
using JetBrains.Annotations;

namespace Atom
{
    public enum CommandIssueSeverity
    {
        Info,
        Warning,
        Error
    }

    public readonly struct CommandIssue
    {
        public readonly CommandIssueSeverity Severity;
        public readonly string Message;
        
        public CommandIssue(CommandIssueSeverity severity, string message)
        {
            Severity = severity;
            Message = message;
        }
    }
    
    public readonly struct CommandResult
    {
        private static readonly CommandResult s_Succeeded = new CommandResult(true);
        public static CommandResult Success() => s_Succeeded;
        public static CommandResult Success(CommandIssue issue) => new CommandResult(true, new[] { issue });
        public static CommandResult Success(IReadOnlyList<CommandIssue> issues) => new CommandResult(true, issues);
        public static CommandResult Failure(CommandIssue issue) => new CommandResult(false, new[] { issue });
        public static CommandResult Failure(IReadOnlyList<CommandIssue> issues) => new CommandResult(false, issues);

        public readonly bool IsSuccess;
        public readonly bool IsFailure;
        public readonly IReadOnlyList<CommandIssue> Issues;

        private CommandResult(bool succeeded, IReadOnlyList<CommandIssue> issues = null)
        {
            IsSuccess = succeeded;
            IsFailure = !succeeded;
            Issues = issues ?? Array.Empty<CommandIssue>();;
        }
    }
}