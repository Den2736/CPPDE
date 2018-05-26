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

        public static class SemanticAnalyzer
        {
            /// <summary>
            /// 
            /// </summary>
            public static void Parse()
            {
                Root.SemanticAnalysis();
            }
        }
    }
}
