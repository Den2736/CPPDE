using System;
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
            /// Receives the path to the source. Generates a lexeme dictionary and a list of variables.
            /// </summary>
            /// <param name="source">путь к файлу, который необходимо скомпилировать</param>
            /// <exception cref="SourceNotFoundException"></exception>
            /// <exception cref="WrongIdentifierException"></exception>
            public static void Parse(string source)
            {
                source = source.Replace("\"", "");  // remove all commas
                Content = GetContent(source);
                Lexemes = GetLexemes();
                Variables = GetVariables();
            }

            private static Dictionary<int, IEnumerable<string>> GetLexemes()
            {
                int lineNumber = 0;
                var lexemes = new Dictionary<int, IEnumerable<string>>();
                foreach (var line in Content)
                {
                    lexemes.Add(++lineNumber, line.Split());
                }

                return lexemes;
            }
            private static List<Variable> GetVariables()
            {
                var variables = new List<Variable>();
                var types = "";
                foreach (var type in Types)
                {
                    types += $"{type}|";
                }
                types = types.Remove(types.Length-1);  // odd '|'

                var reDeclaring = new Regex($@"\b({types}).+=");
                var reType = new Regex(types);
                var reIdentifier = new Regex(@"\b[\w]+[\w\d]*\b");
                int lineNumber = 0;

                foreach (var line in Content)
                {
                    lineNumber++;
                    foreach (Match declaring in reDeclaring.Matches(line))
                    {
                        string type = reType.Match(declaring.Value)?.Value;
                        string identifier = reIdentifier.Match(declaring.Value.Replace(type, ""))?.Value;

                        if(string.IsNullOrEmpty(identifier) && !Types.Contains(identifier))
                        {
                            throw new WrongIdentifierException(lineNumber, line);
                        }
                        else
                        {
                            variables.Add(new Variable()
                            {
                                Name = identifier,
                                Type = type,
                                IsDeclared = true
                            });
                        }
                    }
                }
                return variables;
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
