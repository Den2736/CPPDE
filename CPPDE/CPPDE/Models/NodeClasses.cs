using C__DE.Models.Exceptions.SemanticExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models
{

    public static class Counters
    {
        public static int vars = 0;
        public static int consts = 0;
        public static int temps = 0; //счётчики для переменных, констант и временных ячеек
        public static int ifs = 0;
        public static int cycles = 0;
    }

    public static class Operations
    {
        public static List<string> LogicalOperations = new List<string> { "&&", "||", "!" };
        public static List<string> ArithmeticOperations = new List<string> { "+", "-", "*", "/", "%" };
        public static List<string> BitOperations = new List<string> { "&", "|", "^" };
        public static List<string> ComparationOperations = new List<string> { "==", "!=", ">", "<", "<=", ">=" };
        public static List<string> AssignmentOperations = new List<string> { "=", "+=", "-=", "*=", "/=", "%=", "&&=", "||+"};
    }

    public enum NodeType
    {
        //Types of nodes of parse tree
        Constant,
        Variable,
        ArithmeticOperator,
        ComparisonOperator,
        LogicalOperator,
        BitOperator, //побитовые операции
        AssignmentOperator,
        ConditionalOperator,
        ConditionalBranch, //условная ветка, это тоже блок, возможно, со своими переменными
        CycleOperator,
        VariableDeclaration,
        RootNode,
        ReadNode,
        WriteNode
    }

    public abstract partial class Node
    {
        public NodeType TypeOfNode; //Тип узла
        public BlockNode parentBlock; //ссылка на родительский блок (именно блок - if, while, корневой и т.д.)
        public int LineNumber; //номер строки, где начинается узел (его надо присваивать вручную при синтаксическом анализе)
        public abstract void SetParentBlock(BlockNode Parent); //установка родительского блока (рекурсивная процедура)
    }

    public abstract partial class BlockNode : Node
    {
        public List<Node> ChildrenOperators; //Ссылки на все внутренние операторы
        public void AddOperator(Node Operator)//метод добавления оператора в список дочерних
        {
            ChildrenOperators.Add(Operator);
            Operator.SetParentBlock(this);
        }

        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
        }
    }

    public abstract partial class AtomNode : Node
    {
        //это для синтаксического анализа
        public string Value;

        public Variable MainVariable;
        //в зависимости от типа узла ссылка на переменную в таблице или на временную переменную
    }

    public partial class ConstantNode: AtomNode
    {
        public string ConstantValue; //значение вне зависимости от типа будет храниться в строковом виде
        public ConstantNode(string Type, string ConstValue, int numLine)
        {
            TypeOfNode = NodeType.Constant;
            MainVariable = new Variable();//создаём новую временную переменную
            MainVariable.IsDeclared = true;
            MainVariable.WasUsed=true;
            LineNumber = numLine;
            Value = ConstValue;
            //Это никуда в таблицу переменных не заносится, просто само себе
        }
        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
        }

    }

    public partial class VariableNode : AtomNode
    {
        //public string VariableName;
        public VariableNode(string Var, int numLine)
        {
            //VariableName = Var;
            Value = Var;
            TypeOfNode = NodeType.Variable;
            LineNumber = numLine;
        }

        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
        }
    }

    public partial class BinaryOperatorNode: AtomNode //логический, сравнения или арифметический
    {
        //может быть и унарный - как частный случай
        public bool IsUnary;
        //public string Operator;
        public AtomNode FirstOperand;
        public AtomNode SecondOperand;

        //Создание бинарного оператора
        public BinaryOperatorNode(string Operation, AtomNode First, AtomNode Second, int numLine)
        {
            //Operator = Operation;
            Value = Operation;
            FirstOperand = First;
            SecondOperand = Second;
            IsUnary = false;
            LineNumber = numLine;
            if (Operations.ArithmeticOperations.Contains(Operation))
                TypeOfNode = NodeType.ArithmeticOperator;
            else if (Operations.BitOperations.Contains(Operation))
                TypeOfNode = NodeType.BitOperator;
            else if (Operations.ComparationOperations.Contains(Operation))
                TypeOfNode = NodeType.ComparisonOperator;
            else if (Operations.LogicalOperations.Contains(Operation))
                TypeOfNode = NodeType.LogicalOperator;
                
            //переменную, возможно, создавать не понадобится
        }

        //создание унарного оператора
        public BinaryOperatorNode(string Operation, AtomNode First, int numLine)
        {
            //Operator = Operation;
            Value = Operation;
            FirstOperand = First;
            SecondOperand = null;
            IsUnary = false;
            LineNumber = numLine;
            if (Operation == "-")
                TypeOfNode = NodeType.ArithmeticOperator;
            else
                TypeOfNode = NodeType.LogicalOperator;
        }
        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
            FirstOperand.SetParentBlock(Parent);
            if (!IsUnary)
                SecondOperand.SetParentBlock(Parent);
        }
    }

    public partial class VariableDeclarationNode: AtomNode
    {
        public string Type;
        //при семантическом анализе поставить isDeclared=true у mainVariable
        VariableNode DeclaratedVariable;
        public VariableDeclarationNode(VariableNode var, string VarType, int numLine)
        {
            DeclaratedVariable = var;
            Type = VarType;
            TypeOfNode = NodeType.VariableDeclaration;
            LineNumber = numLine;
        }
        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
            DeclaratedVariable.SetParentBlock(Parent);
        }
    }

    public partial class ConditionalOperatorNode: BlockNode
    {
        public bool ExistedElse; //есть ли ветка else
        public AtomNode Condition;
        public ConditionalBranchNode IfBranch;
        public ConditionalBranchNode ElseBranch;//если ветки else нет, то тут null

        public ConditionalOperatorNode(AtomNode ConditionNode, int numLine, ConditionalBranchNode If, ConditionalBranchNode Else)
        {
            Condition = ConditionNode;//ссылка на узел-условие (на переменную, константу или последнее действие выражения)
            ConditionNode.SetParentBlock(this);
            ChildrenOperators = new List<Node>();
            TypeOfNode = NodeType.ConditionalOperator;
            LineNumber = numLine;
            IfBranch = If;
            ElseBranch = Else;
            If.SetParentBlock(this);
            Else.SetParentBlock(this);
            ExistedElse = true;
        }

        public ConditionalOperatorNode(AtomNode ConditionNode, int numLine, ConditionalBranchNode If)//создание узла без ветки else
        {
            Condition = ConditionNode;//ссылка на узел-условие (на переменную, константу или последнее действие выражения)
            ConditionNode.SetParentBlock(this);
            ChildrenOperators = new List<Node>();
            TypeOfNode = NodeType.ConditionalOperator;
            LineNumber = numLine;
            IfBranch = If;
            ElseBranch = null;
            If.SetParentBlock(this);
            ExistedElse = false;
        }
    }

    public partial class ConditionalBranchNode: BlockNode//какая-то из веток if или else
    {
        public ConditionalBranchNode(int numLine)
        {
            ChildrenOperators = new List<Node>();
            TypeOfNode = NodeType.ConditionalBranch;
        }
    }

    public partial class CycleOperator: BlockNode
    {
        public List<AtomNode> BeginningActivity;
        public AtomNode ContinueCondition;
        public AtomNode IterationActivity; // у while первое и третье null
        public bool IsPredCondition; //true - c предусловием, false - с постусловием

        public CycleOperator(List<AtomNode> Beg, AtomNode Cond, AtomNode IterAct, int numLine)//конструктор для цикла for
        {
            BeginningActivity = Beg;
            ContinueCondition = Cond;
            IterationActivity = IterAct;
            IsPredCondition = true;

            foreach (var oper in Beg)
                oper.SetParentBlock(this);
            Cond.SetParentBlock(this);
            IterAct.SetParentBlock(this);
            TypeOfNode = NodeType.CycleOperator;
            LineNumber = numLine;
        }

        public CycleOperator(bool IsPred, AtomNode Cond, int numLine)//конструктор для цикла while
        {
            BeginningActivity = null;
            ContinueCondition = Cond;
            IterationActivity = null;
            IsPredCondition = IsPred;
            Cond.SetParentBlock(this);
            TypeOfNode = NodeType.CycleOperator;
            LineNumber = numLine;
        }

    }

    public partial class AssignmentOperator: AtomNode
    {
        //оператор присваивания, сюда входят также операторы += -= и так далее
        public string AssignmentOperation; //сама операция (просто присваивание или ещё что-то)
        public AtomNode AssignedVariable;//либо узел -переменная, либо оператор объявления переменной
        public AtomNode RightPart; //присваиваться может только для переменной
        public AssignmentOperator(AtomNode Var, AtomNode Expression, string Operation, int numLine)
        {
            AssignedVariable = Var;
            RightPart = Expression;
            TypeOfNode = NodeType.AssignmentOperator;
            AssignmentOperation = Operation;
            LineNumber = numLine;
        }

        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
            AssignedVariable.SetParentBlock(Parent);
        }

    }

    public partial class MainRootNode: BlockNode
    {
        public MainRootNode(int numLine)
        {
            TypeOfNode = NodeType.RootNode;
            LineNumber = numLine;
            ChildrenOperators = new List<Node>();
        }
    }

    //С чтением и записью хз как, пока строка будет
    public partial class ReadOperator: AtomNode
    {
        public VariableNode ReadVariable;
        public ReadOperator(VariableNode ReadVar, int numLine)
        {
            ReadVariable = ReadVar;
            TypeOfNode = NodeType.ReadNode;
            LineNumber = numLine;
        }
        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
            ReadVariable.SetParentBlock(Parent);
        }
    }

    public partial class WriteOperator : AtomNode
    {
        public AtomNode WriteVariable;
        public WriteOperator( AtomNode WriteVar, int numLine)
        {
            WriteVariable = WriteVar;
            TypeOfNode = NodeType.WriteNode;
            LineNumber = numLine;
        }

        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
            WriteVariable.SetParentBlock(Parent);
        }
    }
}
