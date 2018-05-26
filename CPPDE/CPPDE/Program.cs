using C__DE.Models;
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
        public static IEnumerable<string> Types = new string[] { "int", "char", "string", "float", "bool" };

        /// <summary>
        /// Lexemes in format (line number - lexemes)
        /// </summary>
        public static Dictionary<int, IEnumerable<string>> Lexemes { get; set; }

        /// <summary>
        /// List of variables
        /// </summary>
        public static List<Variable> Variables { get; set; }

        static void Main(string[] args)
        {
            // TODO get sourcePath from args

            try
            {
                Console.Write($"Paste path to the source file:{Environment.NewLine}");
                string sourcePath = Console.ReadLine();
                LexicalAnalyzer.Parse(sourcePath);
                var a = 0;
            }
            catch (CompilerException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
