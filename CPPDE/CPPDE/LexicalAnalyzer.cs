﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using C__DE.Models;
using CPPDE.Models.Exceptions.LexicalExceptions;

namespace CPPDE
{
    partial class Program
    {
        public static class LexicalAnalyzer
        {
            private static IEnumerable<string> Content { get; set; }

            /// <summary>
            /// Receives the path to the source. Generates a lexeme dictionary.
            /// </summary>
            /// <param name="source">путь к файлу, который необходимо скомпилировать</param>
            /// <exception cref="SourceNotFoundException"></exception>
            /// <exception cref="WrongIdentifierException"></exception>
            public static void Parse(string source)
            {
                source = source.Replace("\"", "");  // remove all commas
                Content = GetContent(source);
                Lexemes = GetLexemes();
            }

            private static Dictionary<int, IEnumerable<string>> GetLexemes()
            {
                int lineNumber = 0;
                var lexemes = new Dictionary<int, IEnumerable<string>>();

                foreach (var line in Content)
                {
                    var lineLexemes = LexicalParser.GetLexemes(line);
                    if (lineLexemes.Any())
                    {
                        lexemes.Add(++lineNumber, lineLexemes);
                    }
                    else
                    {
                        lineNumber++;
                    }
                }

                return lexemes;
            }
            private static IEnumerable<string> GetContent(string source)
            {
                List<string> content = new List<string>();
                try
                {
                    using (var file = new StreamReader(source))
                    {
                        while (!file.EndOfStream)
                        {
                            content.Add(file.ReadLine());
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    throw new SourceNotFoundException();
                }

                return content.ToArray();
            }
        }
    }
}
