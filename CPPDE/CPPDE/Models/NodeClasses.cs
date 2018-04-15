using C__DE.Models.Exceptions.SemanticExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models
{
    public class Counters
    {
        public int vars = 0;
        public int consts = 0;
        public int temps = 0; //счётчики для переменных, констант и временных ячеек
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
        public abstract void SetMainVariable(Counters count);
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

        public override void SetMainVariable(Counters count)
        {
            MainVariable.AlternativeName = "const_" + count.consts.ToString();
            ++count.consts;
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

        public override void SetMainVariable(Counters count)
        {
            BlockNode parent = parentBlock; //ищем переменную в родительском блоке
            while (true)
            {
                Variable possibleVariable = parent.BlockVariables.FirstOrDefault(var => var.Name == VariableName);
                if (possibleVariable.Name != null)
                {
                    MainVariable = possibleVariable;
                    return;
                }
                if (parent.TypeOfNode == NodeType.RootNode) //если добрались до родительского и не нашли, то переменная не объявлена
                    throw new UndefinedVariableException(LineNumber, VariableName);
                parent = parent.parentBlock;//идём дальше вверх по иерархии блоков
            }
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

        public void CheckTypesAdd()//если оператор плюс или минус
        {
            switch (FirstOperand.MainVariable.Type)
            {
                case "int":
                    {
                        switch (SecondOperand.MainVariable.Type)
                        {
                            case "int":{ MainVariable.Type = "int"; break;}
                            case "float": { MainVariable.Type = "float"; break; }
                            default: { throw new IncompatibleTypesException(LineNumber, "int", SecondOperand.MainVariable.Type); }
                        }
                        break;
                    }
                case "float":
                    {
                        switch (SecondOperand.MainVariable.Type)
                        {
                            case "int": { MainVariable.Type = "float"; break; }
                            case "float": { MainVariable.Type = "float"; break; }
                            default: { throw new IncompatibleTypesException(LineNumber, "int", SecondOperand.MainVariable.Type); }
                        }
                        break;
                    }
                case "string":
                    {
                        switch (SecondOperand.MainVariable.Type)
                        {
                            case "string": { MainVariable.Type = "string"; break; }
                            case "char": { MainVariable.Type = "string"; break; }
                            default: { throw new IncompatibleTypesException(LineNumber, "string", SecondOperand.MainVariable.Type); }
                        }
                        break;
                    }
                case "char":
                    {
                        switch (SecondOperand.MainVariable.Type)
                        {
                            case "string": { MainVariable.Type = "string"; break; }
                            case "char": { MainVariable.Type = "string"; break; }
                            default: { throw new IncompatibleTypesException(LineNumber, "string", SecondOperand.MainVariable.Type); }
                        }
                        break;
                    }
                case "bool":
                    {
                        throw new InvalidTypeException(LineNumber, "bool", Operator);
                    }
            }
        }

        public void CheckTypesMul() //умножение и деление
        {
            switch (FirstOperand.MainVariable.Type)
            {
                case "int":
                    {
                        switch (SecondOperand.MainVariable.Type)
                        {
                            case "int": { MainVariable.Type = "int"; break; }
                            case "float": { MainVariable.Type = "float"; break; }
                            default: { throw new IncompatibleTypesException(LineNumber, "int", SecondOperand.MainVariable.Type); }
                        }
                        break;
                    }
                case "float":
                    {
                        switch (SecondOperand.MainVariable.Type)
                        {
                            case "int": { MainVariable.Type = "float"; break; }
                            case "float": { MainVariable.Type = "float"; break; }
                            default: { throw new IncompatibleTypesException(LineNumber, "float", SecondOperand.MainVariable.Type); }
                        }
                        break;
                    }
                default:
                    { throw new InvalidTypeException(LineNumber, FirstOperand.MainVariable.Type, Operator); }
            }
        }

        public void CheckTypesComparison()//сравнивать можно число с числом, строку со строкой или bool с bool
        {
            string type1=FirstOperand.MainVariable.Type, type2= SecondOperand.MainVariable.Type;
            if (((type1 == "int" || type1 == "float") && (type2 == "int" || type2 == "float"))//если оба числа
                || ((type1 == "char" || type1 == "string") && (type2 == "char" || type2 == "string"))//или оба строковые
                || (type1 == "bool" && type2 == "bool"))
                MainVariable.Type = "bool";
            else throw new IncompatibleTypesException(LineNumber, type1, type2);
        }

        public void CheckTypesBit()//побитовые операции, допустимы только целые числа и логические значения
        {
            if (FirstOperand.MainVariable.Type == "bool" && SecondOperand.MainVariable.Type == "bool")
                MainVariable.Type = "bool";
            else
            if ((FirstOperand.MainVariable.Type == "int" || FirstOperand.MainVariable.Type == "bool") &&
                (SecondOperand.MainVariable.Type == "int" || SecondOperand.MainVariable.Type == "bool"))
                MainVariable.Type = "int";
            else throw new IncompatibleTypesException(LineNumber, FirstOperand.MainVariable.Type, SecondOperand.MainVariable.Type);
        }

        public void CheckTypesLogical()//логические операции
        {
            string type1 = FirstOperand.MainVariable.Type, type2 = FirstOperand.MainVariable.Type;
            if (((type1 == "int") || (type1 == "float") || (type1 == "bool")) || ((type2 == "int") || (type2 == "float") || (type2 == "bool")))
                MainVariable.Type = "bool";
            else throw new IncompatibleTypesException(LineNumber, FirstOperand.MainVariable.Type, SecondOperand.MainVariable.Type);
        }
        public override void SetMainVariable(Counters count)
        {
            MainVariable = new Variable();
            MainVariable.AlternativeName = "temp_" + count.temps.ToString();
            count.temps++;
            FirstOperand.SetMainVariable(count);
            if (IsUnary)
            {
                if (Operator == "+" || Operator == "-")
                    if (FirstOperand.MainVariable.Type == "int" || FirstOperand.MainVariable.Type == "float")
                    {
                        MainVariable.Type = FirstOperand.MainVariable.Type; //если унарный, то тип совпадает с типом операнда
                        return;
                    }
                    else throw new InvalidTypeException(LineNumber, FirstOperand.MainVariable.Type, Operator);
                if (Operator == "!")
                {
                    if (FirstOperand.MainVariable.Type == "bool" || FirstOperand.MainVariable.Type == "int")
                    {
                        MainVariable.Type = FirstOperand.MainVariable.Type; //если унарный, то тип совпадает с типом операнда
                        return;
                    }
                    else throw new InvalidTypeException(LineNumber, FirstOperand.MainVariable.Type, Operator);
                }
            }
            else
            {
                SecondOperand.SetMainVariable(count);//дальше проверка совместимости типов и корректности операций
                switch (TypeOfNode)
                {
                    case NodeType.ArithmeticOperator:
                        { if (Operator == "+" || Operator == "-")
                                CheckTypesAdd();
                            else CheckTypesMul();
                            break;
                        }
                    case NodeType.ComparisonOperator:
                        {
                            CheckTypesComparison();
                            break;
                        }
                    case NodeType.BitOperator:
                        {
                            CheckTypesBit();
                            break;
                        }
                    case NodeType.LogicalOperator:
                        {
                            CheckTypesLogical();
                            break;
                        }
                }
            }
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
        public override void SetMainVariable(Counters count)
        {
            MainVariable = new Variable(); //создаём новую переменную и наделяем её новыми свойствами
            MainVariable.Name = DeclaratedVariable.VariableName;
            MainVariable.AlternativeName = "var_" + count.vars.ToString();
            count.vars++;
            parentBlock.BlockVariables.Add(MainVariable); //помещаем в список переменных данного блока
            DeclaratedVariable.SetMainVariable(count);
        }

    }

    public class ConditionalOperatorNode: BlockNode
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

    public class ConditionalBranchNode: BlockNode//какая-то из веток if или else
    {
        public ConditionalBranchNode()
        {
            ChildrenOperators = new List<Node>();
            TypeOfNode = NodeType.ConditionalBranch;
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

        public override void SetMainVariable(Counters count)
        {
           
        }
    }

    public class MainRootNode: BlockNode
    {
        public MainRootNode(int numLine)
        {
            TypeOfNode = NodeType.RootNode;
            Counters count = new Counters();
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
