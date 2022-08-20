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
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
using System.Text.RegularExpressions;
using System;
using System.Text;

namespace CZToolKit.Core
{
    public static class CSVLoader
    {
        const char LINE_SPERATOR = '\n';
        static string fieldSperator = "\",\"";
        static Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

        public static string SerializeTableLine(string[] fields)
        {

            for (int f = 0; f < fields.Length; f++)
            {
                if (string.IsNullOrEmpty(fields[f]))
                    fields[f] = "";
                else
                    fields[f] = fields[f].Replace("\"", "\"\"");
            }
            return string.Concat("\"", string.Join(fieldSperator, fields), "\"");
        }

        public static string SerializeTable(string[][] dataTable)
        {
            StringBuilder sb = new StringBuilder();
            for (int lineIndex = 0; lineIndex < dataTable.Length; lineIndex++)
            {
                sb.AppendLine(SerializeTableLine(dataTable[lineIndex]));
            }
            return sb.ToString();
        }

        public static string[] DeserializeTableLine(string line)
        {
            string[] fields = CSVParser.Split(line);
            for (int f = 0; f < fields.Length; f++)
            {
                if (fields[f].Contains(","))
                {
                    fields[f] = fields[f].Substring(1);
                    fields[f] = fields[f].Remove(fields[f].LastIndexOf("\""));
                }
                fields[f] = fields[f].Replace("\"\"", "\"");
            }
            return fields;
        }

        public static string[][] DeserializeTable(string text)
        {
            string[] lines = text.Split(LINE_SPERATOR);
            string[][] dataTable = new string[lines.Length][];
            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i])) continue;
                string[] fields = DeserializeTableLine(lines[i]);
                dataTable[i] = fields;
            }
            return dataTable;
        }

        public static void DeserializeEachLine(string text, Action<string[]> eachLineCallback)
        {
            string[] lines = text.Split(LINE_SPERATOR);
            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i])) continue;
                string[] fields = DeserializeTableLine(lines[i]);
                eachLineCallback(fields);
            }
        }
    }
}
