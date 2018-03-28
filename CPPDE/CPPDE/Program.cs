using CPPDE.Models.Exceptions;
using CPPDE.Models.Exceptions.LexicalExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPPDE
{
    public partial class Program
    {
        /// <summary>
        /// Lexemes in format (line number - lexeme)
        /// </summary>
        public static Dictionary<int, string> Lexemes { get; set; }

        /// <summary>
        /// List of variables in format (identifier - type)
        /// </summary>
        public static List<KeyValuePair<string, Type>> Variables { get; set; }
        
        static void Main(string[] args)
        {
            // TODO get sourcePath from args

            try
            {
                Console.Write($"Paste path to the source file:{Environment.NewLine}" +
                    $">");
                string sourcePath = Console.ReadLine();
                LexicalAnalyzer.Parse(sourcePath);
            }
            catch(CompilerException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
