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
        public static IEnumerable<string> Types = new string[] { "int", /*"char", "string", "float",*/ "bool" };

        /// <summary>
        /// Lexemes in format (line number - lexemes)
        /// </summary>
        public static Dictionary<int, IEnumerable<string>> Lexemes { get; set; }

        /// <summary>
        /// List of variables
        /// </summary>
        public static List<Variable> Variables { get; set; }

        public static MainRootNode Root { get; set; }

        static void Main(string[] args)
        {
            string sourcePath = "";
            if (args.Any())
            {
                sourcePath = args[0];
            }
            else
            {
                Console.Write($"Paste source file need to compile:{Environment.NewLine}" +
                    "> ");
                sourcePath = Console.ReadLine();
            }

            try
            {
                LexicalAnalyzer.Parse(sourcePath);
                SyntaxAnalyzer.Parse();
                Console.WriteLine("Синтаксический анализ окончен");
                SemanticAnalyzer.Parse();
                Console.WriteLine("Семантический анализ окончен");
                if (!(IsSyntaxCorrect && Root.IsSemanticCorrect))
                    Console.WriteLine("Возникли ошибки сборки. Ассемблерный код не будет сгенерирован");
                else
                {
                    Root.GenerateIntermediateCode();
                    GeneratingAssembleCode.Generate();
                    Console.WriteLine("Ассемблерный код сгенерирован");
                }
            }
            catch (CompilerException e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Press any key to finish...");
            Console.ReadLine();
        }
    }
}
