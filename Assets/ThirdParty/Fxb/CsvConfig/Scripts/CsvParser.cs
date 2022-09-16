//参考RedScarf.EasyCSV修改

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Fxb.CsvConfig
{
    public static class CsvParser
    {
        private const char SEPARATOR = ',';

        private const char ESCAPE_CHAR = '"';

        private const char CR = '\r';

        private const char LF = '\n';

        private const string CRLF = "\r\n";

        public static List<List<string>> Parse(string csvStr)
        {
            var colEndReached = false;

            var lineEndReached = false;

            var escapeReached = false;

            var isNonemptyLine = false;

            var csvDatas = new List<List<string>>();

            var colDatas = new List<string>();

            var sb = new StringBuilder();
             
            for (int i = 0, len = csvStr.Length; i <= len; i++)
            {
                if (colEndReached)
                {
                    colEndReached = false;

                    //一列内容完成
                    colDatas.Add(sb.ToString());

                    sb.Clear();
                }

                if (lineEndReached)
                {
                    lineEndReached = false;

                    //一行的内容完成
                    if (isNonemptyLine)
                    {
                        csvDatas.Add(colDatas);
 
                        colDatas = new List<string>();

                        isNonemptyLine = false;
                    }
                    else
                    {
                        colDatas.Clear();
                    }
                }

                if (i >= len)
                    break;

                var c = csvStr[i];

                //双引号
                if (c == ESCAPE_CHAR)
                {
                    if (!escapeReached)
                    {
                        escapeReached = true;

                        continue;
                    }
 
                    //在转义区块内，如果文本的内容也包括双引号，会额外使用一个双引号来进行转义。只写入一个双引号。
                    if (i < len - 1 && csvStr[i + 1] == ESCAPE_CHAR)      
                        i++;            
                    else
                    {
                        escapeReached = false;

                        continue;
                    }
                }
                
                if (!escapeReached)
                {
                    //新行
                    if (c == CR || c == LF)
                    {
                        if (c == CR && i < len - 1 && csvStr[i + 1] == LF)
                            i++;

                        colEndReached = true;

                        lineEndReached = true;

                        continue;
                    }

                    //新列
                    if (c == SEPARATOR)
                    {
                        colEndReached = true;

                        continue;
                    }
                }

                isNonemptyLine = true;

                sb.Append(c);
            }

            return csvDatas;
        }
    }
}