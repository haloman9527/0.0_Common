#region 注 释

/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.mindgear.net/
 *
 */

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace CZToolKit.RPN
{
    public class TextUtil
    {
        public static readonly object _lock = new object();

        private static Queue<StringBuilder> SBCaches { get; } = new Queue<StringBuilder>();

        public static StringBuilder SpawnSB()
        {
            lock (_lock)
            {
                if (SBCaches.Count == 0)
                    return new StringBuilder();
                return SBCaches.Dequeue();
            }
        }

        public static void RecycleSB(StringBuilder sb)
        {
            lock (_lock)
            {
                sb.Clear();
                SBCaches.Enqueue(sb);
            }
        }
    }

    public enum TokenType
    {
        Number = 0,
        Symbol = 1,
        Eof = 2
    }

    public unsafe struct Token : IEquatable<Token>
    {
        private bool initialized;
        private string text;
        private TokenType tokenType;
        private int priority;

        public bool Initialized
        {
            get { return initialized; }
        }

        public TokenType TokenType
        {
            get { return tokenType; }
        }

        public int Priority
        {
            get { return priority; }
        }

        public Token(char* source, int startIndex, int endIndex, TokenType tokenType, int priority)
        {
            this.initialized = true;
            this.tokenType = tokenType;
            this.priority = priority;
            var sb = TextUtil.SpawnSB();
            for (int i = startIndex; i <= endIndex; i++)
            {
                sb.Append(*(source + i));
            }

            text = sb.ToString();
            TextUtil.RecycleSB(sb);
        }

        public override string ToString()
        {
            return text;
        }

        public bool Equals(Token other)
        {
            return text == other.text;
        }

        public static implicit operator string(Token token)
        {
            return token.text;
        }
    }


    public unsafe class Lexer
    {
        private bool isRuning;
        private char* text;
        private int length;
        private int currentIndex;
        private Token lastToken;

        public void Begin(string text)
        {
            if (isRuning)
                throw new Exception("Lexer已经启动");
            isRuning = true;
            length = text.Length;
            fixed (char* p = text)
                this.text = p;
        }

        public void End()
        {
            currentIndex = 0;
            lastToken = default;
            text = null;
        }

        public Token GetNextToken()
        {
            if (!isRuning)
                throw new Exception("需要先启动Lexer");
            SkipWhiteSpace();

            if (currentIndex >= length)
            {
                var token = new Token(text, currentIndex, currentIndex, TokenType.Eof, -1);
                End();
                return token;
            }

            char current = *(text + currentIndex);
            switch (current)
            {
                case '*':
                case '/':
                {
                    var token = new Token(text, currentIndex, currentIndex, TokenType.Symbol, 2);
                    MoveToNext();
                    lastToken = token;
                    return token;
                }
            }

            switch (current)
            {
                case '+':
                case '-':
                {
                    Token token = default;
                    if (!lastToken.Initialized)
                        token = ScanNumber();
                    else
                    {
                        switch (lastToken.TokenType)
                        {
                            case TokenType.Number:
                                token = new Token(text, currentIndex, currentIndex, TokenType.Symbol, 1);
                                MoveToNext();
                                break;
                            case TokenType.Symbol:
                                token = ScanNumber();
                                break;
                        }
                    }

                    lastToken = token;
                    return token;
                }
            }

            switch (current)
            {
                case '(':
                case ')':
                {
                    var token = new Token(text, currentIndex, currentIndex, TokenType.Symbol, 0);
                    MoveToNext();
                    lastToken = token;
                    return token;
                }
            }

            if (char.IsDigit(current) || current == '.')
            {
                var token = ScanNumber();
                lastToken = token;
                return token;
            }

            throw new Exception();
        }

        private Token ScanNumber()
        {
            var startIndex = currentIndex;
            var endIndex = -1;
            while (true)
            {
                if (currentIndex >= length)
                {
                    endIndex = currentIndex - 1;
                    break;
                }

                var current = *(text + currentIndex);
                if (!char.IsDigit(current) && current != '.')
                {
                    endIndex = currentIndex - 1;
                    break;
                }

                MoveToNext();
            }

            return new Token(text, startIndex, endIndex, TokenType.Number, 0);
        }

        private void MoveToNext(int step = 1)
        {
            currentIndex += step;
        }

        private void SkipWhiteSpace()
        {
            while (currentIndex < length)
            {
                char current = *(text + currentIndex);
                if (IsWhiteSpace(current))
                    MoveToNext();
                else
                    break;
            }
        }

        private bool IsWhiteSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\r' || c == '\n';
        }
    }

    public class RPN
    {
        public Lexer lexer = new Lexer();
        public Stack<Token> tokenStack = new Stack<Token>();
        public Stack<float> numberStack = new Stack<float>();
        public Stack<float> boolStack = new Stack<float>();

        /// <summary> 中缀转后缀表达式 </summary>
        public IEnumerable<Token> ToRPN(string expr)
        {
            lexer.Begin(expr);
            tokenStack.Clear();
            while (true)
            {
                var token = lexer.GetNextToken();

                if (token.TokenType == TokenType.Eof)
                    break;
                switch (token.TokenType)
                {
                    case TokenType.Number:
                    {
                        yield return token;
                        continue;
                    }
                    case TokenType.Symbol:
                    {
                        if (token.ToString() == ")")
                        {
                            while (tokenStack.Count > 0)
                            {
                                var t = tokenStack.Pop();
                                if (t.ToString() == "(")
                                    break;

                                yield return t;
                            }
                        }
                        else
                        {
                            if (token.ToString() != "(")
                            {
                                while (tokenStack.Count > 0)
                                {
                                    var t = tokenStack.Peek();
                                    if (t.Priority < token.Priority)
                                        break;
                                    yield return tokenStack.Pop();
                                }
                            }

                            tokenStack.Push(token);
                        }

                        continue;
                    }
                }
            }

            while (tokenStack.Count > 0)
            {
                yield return tokenStack.Pop();
            }

            lexer.End();
        }

        public float Calculate(Token[] tokens)
        {
            numberStack.Clear();
            foreach (var token in tokens)
            {
                if (token.TokenType == TokenType.Eof)
                    break;

                switch (token.TokenType)
                {
                    case TokenType.Number:
                        numberStack.Push(float.Parse(token.ToString()));
                        break;
                    case TokenType.Symbol:
                        var t1 = numberStack.Pop();
                        var t2 = numberStack.Pop();
                        switch (token.ToString())
                        {
                            case "+":
                                numberStack.Push(t2 + t1);
                                break;
                            case "-":
                                numberStack.Push(t2 - t1);
                                break;
                            case "*":
                                numberStack.Push(t2 * t1);
                                break;
                            case "/":
                                numberStack.Push(t2 / t1);
                                break;
                        }
                        break;
                }
            }

            return numberStack.Pop();
        }
    }
}