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
            RootNode = 8
        }

        public abstract class Node
        {
            public NodeType TypeOfNode;
            public abstract void Analysis();
            public BlockNode parentBlock; //ссылка на родительский блок (именно блок - if, while, корневой и т.д.)
        }

        public abstract class BlockNode:Node
        {
            List<Node> children; //Ссылки на все внутренние операторы
            //Переменные блока

        }

        public abstract class AtomNode:Node
        {
            String ValueType;
            String VariableName; //имя промежуточной переменной, где будет храниться результат
        }

        public class VariableNode:AtomNode
        {
            string Name;
            public override void Analysis()
            {
            }
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
