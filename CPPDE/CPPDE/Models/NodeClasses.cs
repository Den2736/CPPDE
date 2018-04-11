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
    }

    public abstract class BlockNode : Node
    {
        public List<Node> ChildrenOperators; //Ссылки на все внутренние операторы
        public List<Variable> BlockVariables; //Переменные блока

        public void AddOperator(Node Operator)//метод добавления оператора в список дочерних
        {
            ChildrenOperators.Add(Operator);
            Operator.parentBlock = this;
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
        public ConstantNode(string Type, string ConstValue)
        {
            TypeOfNode = NodeType.Constant;
            MainVariable = new Variable();//создаём новую временную переменную
            MainVariable.IsDeclared = true;
            MainVariable.WasUsed=true;
            //Это никуда в таблицу переменных не заносится, просто само себе
        }
    }

    public class VariableNode : AtomNode
    {
        VariableNode(Variable Var)
        {
            MainVariable = Var;//просто тупо создаётся ссылка
            TypeOfNode = NodeType.Variable;
        }
    }

    public class BinaryOperatorNode: AtomNode //логический, сравнения или арифметический
    {
        //может быть и унарный - как частный случай
        public bool IsUnary;
        public string Operator;
        public AtomNode FirstOperand;
        public AtomNode SecondOperand;
        public BinaryOperatorNode(string Operation, AtomNode First, AtomNode Second, NodeType OperatorType)
        {
            Operator = Operation;
            FirstOperand = First;
            SecondOperand = Second;
            TypeOfNode = OperatorType;
            IsUnary = false;
            //переменную, возможно, создавать не понадобится
        }
        public BinaryOperatorNode(string Operation, AtomNode First, NodeType OperatorType)
        {
            Operator = Operation;
            FirstOperand = First;
            SecondOperand = null;
            TypeOfNode = OperatorType;
            IsUnary = false;
        }
    }

    public class VariableDeclarationNode: AtomNode
    {
        //дополнительных полей нет
        //при семантическом анализе поставить isDeclared=true у mainVariable
        public VariableDeclarationNode(Variable var)
        {
            MainVariable = var;
            MainVariable.IsDeclared = true;
            TypeOfNode = NodeType.VariableDeclaration;
        }
    }

    public class ConditionalOperatorNode: BlockNode
    {
        public AtomNode Condition;
        public List<Node> ElseOperators; //если нет ветки else - пусто либо null
        public ConditionalOperatorNode(AtomNode ConditionNode)
        {
            Condition = ConditionNode;//ссылка на узел-условие (на переменную, константу или последнее действие выражения)
            ConditionNode.parentBlock = this;
            ElseOperators = new List<Node>();
            ChildrenOperators = new List<Node>();
            TypeOfNode = NodeType.ConditionalOperator;
        }
        
        public void AddElseOperator(Node Operator)
        {
            ChildrenOperators.Add(Operator);
            Operator.parentBlock = this;
        }
    }

    public class CycleOperator: BlockNode
    {
        public AtomNode BeginningActivity;
        public AtomNode ContinueCondition;
        public AtomNode IterationActivity; // у while первое и третье null
        public bool IsPredCondition; //true - c предусловием, false - с постусловием

        public CycleOperator(AtomNode Beg, AtomNode Cond, AtomNode IterAct)//конструктор для цикла for
        {
            BeginningActivity = Beg;
            ContinueCondition = Cond;
            IterationActivity = IterAct;
            IsPredCondition = true;
            Beg.parentBlock = this;
            Cond.parentBlock = this;
            IterAct.parentBlock = this;
            TypeOfNode = NodeType.CycleOperator;
        }

        public CycleOperator(bool IsPred, AtomNode Cond)//конструктор для цикла while
        {
            BeginningActivity = null;
            ContinueCondition = Cond;
            IterationActivity = null;
            IsPredCondition = IsPred;
            Cond.parentBlock = this;
            TypeOfNode = NodeType.CycleOperator;
        }
    }

    public class AssignmentOperator: AtomNode
    {
        //оператор присваивания, сюда входят также операторы += -= и так далее
        public AtomNode RightPart; //присваиваться может только для переменной
        public AssignmentOperator(Variable Var, AtomNode Expression)
        {
            MainVariable = Var;
            RightPart = Expression;
            TypeOfNode = NodeType.AssignmentOperator;
        }
    }

    public class MainRootNode: BlockNode
    {
        public MainRootNode()
        {
            TypeOfNode = NodeType.RootNode;
        }
    }

    //С чтением и записью хз как, пока строка будет
    public class ReadOperator: AtomNode
    {
        public string Source;
        public ReadOperator(string File)
        {
            Source = File;
            TypeOfNode = NodeType.ReadNode;
        }
    }

    public class WriteOperator : AtomNode
    {
        public string Source;
        public WriteOperator(string File)
        {
            Source = File;
            TypeOfNode = NodeType.WriteNode;
        }
    }
}
