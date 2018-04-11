using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models
{


    public enum NodeType
    {
        //Types of nodes of parse tree
        Constant,
        Variable,
        ArithmeticOperator,
        ComparisonOperator,
        LogicalOperator,
        AssignmentOperator,
        ConditionalOperator,
        CycleOperator,
        VariableDeclaration,
        RootNode,
        ReadNode,
        WriteNode
    }

    public abstract class Node
    {
        public NodeType TypeOfNode; //Тип узла
        public BlockNode parentBlock; //ссылка на родительский блок (именно блок - if, while, корневой и т.д.)
        public int LineNumber; //номер строки, где начинается узел (его надо присваивать вручную при синтаксическом анализе)
        public abstract void SetParentBlock(BlockNode Parent); //установка родительского блока (рекурсивная процедура)
    }

    public abstract class BlockNode : Node
    {
        public List<Node> ChildrenOperators; //Ссылки на все внутренние операторы
        public List<Variable> BlockVariables; //Переменные блока
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

    public abstract class AtomNode : Node
    {
        public Variable MainVariable;
        //в зависимости от типа узла ссылка на переменную в таблице или на временную переменную
    }

    public class ConstantNode: AtomNode
    {
        public string ConstantValue; //значение вне зависимости от типа будет храниться в строковом виде
        public ConstantNode(string Type, string ConstValue, int numLine)
        {
            TypeOfNode = NodeType.Constant;
            MainVariable = new Variable();//создаём новую временную переменную
            MainVariable.IsDeclared = true;
            MainVariable.WasUsed=true;
            LineNumber = numLine;
            //Это никуда в таблицу переменных не заносится, просто само себе
        }
        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
        }
    }

    public class VariableNode : AtomNode
    {
        public string VariableName;
        VariableNode(string Var, int numLine)
        {
            VariableName = Var;//просто тупо создаётся ссылка
            TypeOfNode = NodeType.Variable;
            LineNumber = numLine;
        }

        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
        }
    }

    public class BinaryOperatorNode: AtomNode //логический, сравнения или арифметический
    {
        //может быть и унарный - как частный случай
        public bool IsUnary;
        public string Operator;
        public AtomNode FirstOperand;
        public AtomNode SecondOperand;
        public BinaryOperatorNode(string Operation, AtomNode First, AtomNode Second, NodeType OperatorType, int numLine)
        {
            Operator = Operation;
            FirstOperand = First;
            SecondOperand = Second;
            TypeOfNode = OperatorType;
            IsUnary = false;
            LineNumber = numLine;
            //переменную, возможно, создавать не понадобится
        }
        public BinaryOperatorNode(string Operation, AtomNode First, NodeType OperatorType, int numLine)
        {
            Operator = Operation;
            FirstOperand = First;
            SecondOperand = null;
            TypeOfNode = OperatorType;
            IsUnary = false;
            LineNumber = numLine;
        }
        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
            FirstOperand.SetParentBlock(Parent);
            if (!IsUnary)
                SecondOperand.SetParentBlock(Parent);
        }
    }

    public class VariableDeclarationNode: AtomNode
    {
        string Type;
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

    public class ConditionalOperatorNode: BlockNode
    {
        public AtomNode Condition;
        public List<Node> ElseOperators; //если нет ветки else - пусто либо null

        public ConditionalOperatorNode(AtomNode ConditionNode, int numLine)
        {
            Condition = ConditionNode;//ссылка на узел-условие (на переменную, константу или последнее действие выражения)
            ConditionNode.SetParentBlock(this);
            ElseOperators = new List<Node>();
            ChildrenOperators = new List<Node>();
            TypeOfNode = NodeType.ConditionalOperator;
            LineNumber = numLine;
        }
        
        public void AddElseOperator(Node Operator)
        {
            ChildrenOperators.Add(Operator);
            Operator.SetParentBlock(this);
        }

    }

    public class CycleOperator: BlockNode
    {
        public AtomNode BeginningActivity;
        public AtomNode ContinueCondition;
        public AtomNode IterationActivity; // у while первое и третье null
        public bool IsPredCondition; //true - c предусловием, false - с постусловием

        public CycleOperator(AtomNode Beg, AtomNode Cond, AtomNode IterAct, int numLine)//конструктор для цикла for
        {
            BeginningActivity = Beg;
            ContinueCondition = Cond;
            IterationActivity = IterAct;
            IsPredCondition = true;
            Beg.SetParentBlock(this);
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

    public class AssignmentOperator: AtomNode
    {
        //оператор присваивания, сюда входят также операторы += -= и так далее
        public VariableNode AssignedVariable;
        public AtomNode RightPart; //присваиваться может только для переменной
        public AssignmentOperator(VariableNode Var, AtomNode Expression, int numLine)
        {
            AssignedVariable = Var;
            RightPart = Expression;
            TypeOfNode = NodeType.AssignmentOperator;
            LineNumber = numLine;
        }

        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
            AssignedVariable.SetParentBlock(Parent);
        }
    }

    public class MainRootNode: BlockNode
    {
        public MainRootNode(int numLine)
        {
            TypeOfNode = NodeType.RootNode;
        }
    }

    //С чтением и записью хз как, пока строка будет
    public class ReadOperator: AtomNode
    {
        public string Source;
        public VariableNode ReadVariable;
        public ReadOperator(string File, VariableNode ReadVar, int numLine)
        {
            Source = File;
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

    public class WriteOperator : AtomNode
    {
        public string Source;
        public VariableNode WriteVariable;
        public WriteOperator(string File, VariableNode WriteVar, int numLine)
        {
            Source = File;
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
