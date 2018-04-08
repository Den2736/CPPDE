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
        public enum NodeType
        {
            //Types of nodes of parse tree
            Variable = 1,
            ArithmeticOperator = 2,
            LogicalOperator = 3,
            AssignmentOperator = 4,
            ConditionalOperator = 5,
            CycleOperator = 6,
            VariableDeclaration = 7,
            MainNode = 8
        }

        public static class SemanticAnalyzer
        {
            /// <summary>
            /// 
            /// </summary>
            public static void Parse()
            {
                throw new NotImplementedException();
            }
        }
    }
}
