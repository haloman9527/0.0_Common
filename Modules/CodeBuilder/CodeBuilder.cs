using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CZToolKit
{
    public class CodeBuilder
    {
        private StringBuilder codeBuilder = new StringBuilder();
        private StringBuilder tempBuilder = new StringBuilder();
        private Dictionary<string, int> marks = new Dictionary<string, int>();
        public int indentLevel = 0;

        public int Lenght
        {
            get { return codeBuilder.Length; }
        }

        public void Clear()
        {
            codeBuilder.Clear();
            tempBuilder.Clear();
            indentLevel = 0;
        }

        public void Mark(string key)
        {
            marks[key] = codeBuilder.Length;
        }

        public void RemoveMark(string key)
        {
            marks.Remove(key);
        }

        public int GetMark(string key)
        {
            return marks[key];
        }

        public void WriteLine()
        {
            codeBuilder.AppendLine();
        }

        public void WriteLine(string code)
        {
            var text = $"{new string(' ', indentLevel * 4)}{code}";
            codeBuilder.AppendLine(text);
        }

        public void WriteLineWithoutIndent(string code)
        {
            codeBuilder.AppendLine(code);
        }

        public void BeginRegion(string region)
        {
            WriteLine($"#region {region}");
        }

        public void EndRegion()
        {
            WriteLine($"#endregion");
        }

        public void BeginCodeBlock()
        {
            WriteLine("{");
            indentLevel++;
        }

        public void EndCodeBlock()
        {
            indentLevel--;
            WriteLine("}");
        }

        public void EndCodeBlockSemicolon()
        {
            indentLevel--;
            WriteLine("};");
        }

        public void InsertLine(int index, string code)
        {
            var text = $"{new string(' ', indentLevel * 4)}{code}\n";
            codeBuilder.Insert(index, text);
            foreach (var key in marks.Keys.ToArray())
            {
                if (marks[key] >= index)
                {
                    marks[key] += text.Length;
                }
            }
        }

        public void InsertLineWithoutIndent(int index, string code)
        {
            var text = code + '\n';
            codeBuilder.Insert(index, text);
            foreach (var key in marks.Keys.ToArray())
            {
                if (marks[key] >= index)
                {
                    marks[key] += text.Length;
                }
            }
        }

        public void WriteUsingNamespace(string name)
        {
            WriteLine($"using {name};");
        }

        public void WriteRenameType(string newTypeName, string sourceTypeName)
        {
            WriteLine($"using {newTypeName} = {sourceTypeName};");
        }
        
        public void BeginNamespace(string name)
        {
            WriteLine($"namespace {name}");
            BeginCodeBlock();
        }

        public void EndNamespace()
        {
            EndCodeBlock();
        }

        public void MemberSummay(string summary, Dictionary<string, string> paramsSummary = null)
        {
            WriteLine("/// <summary>");
            WriteLine($"/// {summary.Replace('\n', ' ')}");
            WriteLine("/// </summary>");

            if (paramsSummary != null)
            {
                foreach (var pair in paramsSummary)
                {
                    WriteLine($"/// <param name=\"{pair.Key}\"> {pair.Value.Replace('\n', ' ')} </param>");
                }
            }
        }

        public void BeginClass(string name, bool isStatic)
        {
            var staticKey = isStatic ? " static" : "";
            WriteLine($"public{staticKey} class {name}");
            BeginCodeBlock();
        }

        public void EndClass()
        {
            EndCodeBlock();
        }

        public void BeginMethod(string name, Type returnType, bool isStatic, Dictionary<string, string> parameters = null)
        {
            var staticKey = isStatic ? " static" : "";
            tempBuilder.Clear();
            if (parameters != null)
            {
                var first = true;
                foreach (var pair in parameters)
                {
                    if (first)
                    {
                        tempBuilder.Append($"{pair.Value} {pair.Key}");
                        first = false;
                    }
                    else
                    {
                        tempBuilder.Append($", {pair.Value} {pair.Key}");
                    }
                }
            }

            var returnTypeName = returnType == typeof(void) ? "void" : returnType.FullName;
            WriteLine($"public{staticKey} {returnTypeName} {name}({tempBuilder})");
            BeginCodeBlock();
        }

        public void EndMethod()
        {
            EndCodeBlock();
        }

        public void BeginIf(string condition)
        {
            WriteLine($"if({condition})");
            BeginCodeBlock();
        }

        public void EndIf()
        {
            EndCodeBlock();
        }

        public void BeginElseIf(string condition)
        {
            WriteLine($"if({condition})");
            BeginCodeBlock();
        }

        public void EndElseIf()
        {
            EndCodeBlock();
        }

        public void BeginElse()
        {
            WriteLine($"else");
            BeginCodeBlock();
        }

        public void EndElse()
        {
            EndCodeBlock();
        }

        public override string ToString()
        {
            return codeBuilder.ToString();
        }
    }

    public struct Line
    {
        public string text;
        public int startIndex;
        public int indent;
    }
}