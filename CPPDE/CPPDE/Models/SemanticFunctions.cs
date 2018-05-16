using C__DE.Models.Exceptions.SemanticExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models
{
    public abstract partial class Node
    {
        public abstract bool SemanticAnalysis(); //возвращает true, если нет семантических ошибок
    }

    public abstract partial class BlockNode : Node
    {
        public List<Variable> BlockVariables; //Переменные блока
    }

    public abstract partial class AtomNode : Node
    {
        public abstract void SetMainVariable(Counters count);
        //в зависимости от типа узла ссылка на переменную в таблице или на временную переменную
    }

    public partial class ConstantNode : AtomNode
    {
        public override void SetMainVariable(Counters count)
        {
            MainVariable.AlternativeName = "const_" + count.consts.ToString();
            ++count.consts;
        }

        public override bool SemanticAnalysis()
        {
            return true;
        }
    }

    public partial class VariableNode : AtomNode
    {
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

        public override bool SemanticAnalysis()
        {
            if (MainVariable.IsDeclared)
                return true;
            else
            {
                throw new UndefinedVariableException(LineNumber, MainVariable.Name);
            }
        }
    }

    public partial class BinaryOperatorNode : AtomNode //логический, сравнения или арифметический
    {
        public void CheckTypesAdd()//если оператор плюс или минус
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
            string type1 = FirstOperand.MainVariable.Type, type2 = SecondOperand.MainVariable.Type;
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
                        {
                            if (Operator == "+" || Operator == "-")
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

    public partial class VariableDeclarationNode : AtomNode
    {
        public override void SetMainVariable(Counters count)
        {
            MainVariable = new Variable(); //создаём новую переменную и наделяем её новыми свойствами
            MainVariable.Name = DeclaratedVariable.VariableName;
            MainVariable.AlternativeName = "var_" + count.vars.ToString();
            MainVariable.IsDeclared = true;
            count.vars++;
            parentBlock.BlockVariables.Add(MainVariable); //помещаем в список переменных данного блока
            DeclaratedVariable.SetMainVariable(count);
        }

    }

    public partial class ConditionalOperatorNode : BlockNode
    {
 
    }

    public partial class ConditionalBranchNode : BlockNode//какая-то из веток if или else
    {
    }

    public partial class CycleOperator : BlockNode
    {
        
    }

    public partial class AssignmentOperator : AtomNode
    {

        public override void SetMainVariable(Counters count)
        {
            MainVariable = new Variable();
            MainVariable.Name = AssignedVariable.MainVariable.Name;
            MainVariable.AlternativeName = AssignedVariable.MainVariable.AlternativeName;
            MainVariable.IsDeclared = AssignedVariable.MainVariable.IsDeclared;
            MainVariable.Type = AssignedVariable.MainVariable.Type;
        }
    }

    public partial class MainRootNode : BlockNode
    {
    }

    //С чтением и записью хз как, пока строка будет
    public partial class ReadOperator : AtomNode
    {
        public override void SetMainVariable(Counters count)
        {
            MainVariable = new Variable();
            MainVariable.Name = ReadVariable.MainVariable.Name;
            MainVariable.AlternativeName = ReadVariable.MainVariable.AlternativeName;
            MainVariable.IsDeclared = ReadVariable.MainVariable.IsDeclared;
            MainVariable.Type = ReadVariable.MainVariable.Type;
        }
    }

    public partial class WriteOperator : AtomNode
    {
        public override void SetMainVariable(Counters count)
        {
            MainVariable = new Variable();
            MainVariable.Name = WriteVariable.MainVariable.Name;
            MainVariable.AlternativeName = WriteVariable.MainVariable.AlternativeName;
            MainVariable.IsDeclared = WriteVariable.MainVariable.IsDeclared;
            MainVariable.Type = WriteVariable.MainVariable.Type;
        }
    }
}
