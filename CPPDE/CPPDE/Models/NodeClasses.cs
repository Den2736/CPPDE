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
        Constant =0,
        Variable = 1,
        ArithmeticOperator = 2,
        ComparisonOperator=3,
        LogicalOperator = 4,
        AssignmentOperator = 5,
        ConditionalOperator = 6,
        CycleOperator = 7,
        VariableDeclaration = 8,
        RootNode = 9
    }

    public abstract class Node
    {
        public NodeType TypeOfNode;
        public BlockNode parentBlock; //ссылка на родительский блок (именно блок - if, while, корневой и т.д.)
        public int LineNumber; //номер строки, где начинается узел
    }

    public abstract class BlockNode : Node
    {
        List<Node> ChildrenOperators; //Ссылки на все внутренние операторы
        List<Variable> BlockVariables; //Переменные блока
    }

    public abstract class AtomNode : Node //в этом классе всё к семантике
    {
        string ValueType;
        string VariableName; //имя промежуточной переменной, где будет храниться результат
    }

    public class ConstantNode: VariableNode
    {
        ConstantNode(int ConstantNumber)
        {
            MainVariable = new Variable();
            MainVariable.AlternativeName = "const_" + ConstantNumber.ToString();
        }
    }
    //объединить тип "Константа" и "Переменная"
    public class VariableNode : AtomNode
    {
        public Variable MainVariable; //просто ссылка на переменную в таблице
    }

    public class ArithmeticOperatorNode: VariableNode
    {
        public string ArithmeticOperation;
        public VariableNode SecondOperand;//а если константа?
    }
    //Может, их как-то объединить?
    public class ComparisionOperator: VariableNode
    {
        public string ComparisionOperation;//сам оператор
        public VariableNode SecondOperand;
    }

    public class LogicalNode: VariableNode
    {
        public string LogicalOperation;
        public VariableNode SecondOperand; //Для одноместной операции тут будет null
    }

    public class VariableDeclaration: VariableNode
    {
        //при семантическом анализе поставить isDeclared=true у mainVariable
    }

    public class ConditionalOperator: BlockNode
    {
        public VariableNode Condition;
        public List<Node> ElseOperators; //если нет ветки else - пусто либо null
    }

    public class CycleOperator: BlockNode
    {
        public VariableNode BeginningActivity;
        public VariableNode ConditionActivity;
        public VariableNode IterationActivity; // у while первое и третье null
        public bool IsPredCondition; //true - c предусловием, false - с постусловием
    }

    public class AssignmentOperator
    {
        public VariableNode RightPart;
    }
    //У root никаких дополнительных полей и действий
}
