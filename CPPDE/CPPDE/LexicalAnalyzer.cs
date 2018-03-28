using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPPDE.Models.Exceptions.LexicalExceptions;

namespace CPPDE
{
    partial class Program
    {
        public static class LexicalAnalyzer
        {
            /// <summary>
            /// Receives the path to the source. Generates a lexeme dictionary and a list of variables.
            /// </summary>
            /// <param name="source">путь к файлу, который необходимо скомпилировать</param>
            /// <exception cref="SourceNotFoundException"></exception>
            /// <exception cref="WrongIdentifierException"></exception>
            public static void Parse(string source)
            {
                source = source.Replace("\"", "");  // remove all commas

                string content = "";
                try
                {
                    using (var file = new StreamReader(source))
                    {
                        content = file.ReadToEnd();
                    }
                }
                catch (FileNotFoundException)
                {
                    throw new SourceNotFoundException();
                }
            }
        }
    }
}
