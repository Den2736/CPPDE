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
        public bool IsSemanticCorrect;
    }

    public abstract partial class BlockNode : Node
    {
        public List<Variable> BlockVariables; //Переменные блока
    }

    public abstract partial class AtomNode : Node
    {
        //public abstract void SetMainVariable();
        //в зависимости от типа узла ссылка на переменную в таблице или на временную переменную
    }

    public partial class ConstantNode : AtomNode
    {
        public override bool SemanticAnalysis()
        {
            IsSemanticCorrect = true;
            return true;
        }
    }

    public partial class VariableNode : AtomNode
    {
        public override bool SemanticAnalysis()
        {
            BlockNode parent = parentBlock; //ищем переменную в родительском блоке
            while (true)
            {
                Variable possibleVariable = parent.BlockVariables.FirstOrDefault(var => var.Name == Value);
                if (possibleVariable.Name != null)
                {
                    MainVariable = possibleVariable;
                    return true;
                }
                if (parent.TypeOfNode == NodeType.RootNode) //если добрались до родительского и не нашли, то переменная не объявлена
                    throw new UndefinedVariableException(LineNumber, Value);
                parent = parent.parentBlock;//идём дальше вверх по иерархии блоков
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
                        throw new InvalidTypeException(LineNumber, "bool", Value);
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
                    { throw new InvalidTypeException(LineNumber, FirstOperand.MainVariable.Type, Value); }
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

        public override bool SemanticAnalysis()
        {
            try
            {
                IsSemanticCorrect =FirstOperand.SemanticAnalysis();
            }
            catch (SemanticException)
            {
                IsSemanticCorrect = false;
            }

            if (IsUnary)
                if (IsSemanticCorrect)
                {
                    if (Value == "-")
                        if (FirstOperand.MainVariable.Type == "int" || FirstOperand.MainVariable.Type == "float")
                        {
                            MainVariable.Type = FirstOperand.MainVariable.Type; //если унарный, то тип совпадает с типом операнда
                            return true;
                        }
                        else throw new InvalidTypeException(LineNumber, FirstOperand.MainVariable.Type, Value);
                    if (Value == "!")
                    {
                        if (FirstOperand.MainVariable.Type == "bool" || FirstOperand.MainVariable.Type == "int")
                        {
                            MainVariable.Type = FirstOperand.MainVariable.Type; //если унарный, то тип совпадает с типом операнда
                            return true;
                        }
                        else throw new InvalidTypeException(LineNumber, FirstOperand.MainVariable.Type, Value);
                    }
                    return false;//просто чтоб компилятор не ругался, он сюда никогда не дойдёт
                }
                else
                {
                    return false;
                }
            else
            {
                bool IsSecondOperandCorrect=SecondOperand.SemanticAnalysis();//дальше проверка совместимости типов и корректности операций
                if (IsSecondOperandCorrect && IsSemanticCorrect)
                {
                    switch (TypeOfNode)
                    {
                        case NodeType.ArithmeticOperator:
                            {
                                if (Value == "+" || Value == "-")
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
                    return true;
                }
                else
                {
                    IsSemanticCorrect = false;
                    return false;
                }
            }
        }
    }

    public partial class VariableDeclarationNode : AtomNode
    {
        public override bool SemanticAnalysis()
        {
            MainVariable = new Variable(); //создаём новую переменную и наделяем её новыми свойствами
            MainVariable.Name = DeclaratedVariable.Value;
            MainVariable.AlternativeName = "var_" + Counters.vars.ToString();
            MainVariable.IsDeclared = true;
            MainVariable.Type = Type;
            Counters.vars++;

            //нужно посмотреть повторное объявление
            Variable possibleVariable = parentBlock.BlockVariables.FirstOrDefault(var => var.Name == DeclaratedVariable.Value);
            if (possibleVariable.Name != null)
            {
                IsSemanticCorrect = false;
                throw new RedeclaringVariableException(LineNumber, DeclaratedVariable.Value);
            }

            parentBlock.BlockVariables.Add(MainVariable); //помещаем в список переменных данного блока
            DeclaratedVariable.SemanticAnalysis();
            IsSemanticCorrect = true;
            return true;

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

        public override bool SemanticAnalysis()
        {
            IsSemanticCorrect = true;
            try
            {
                AssignedVariable.SemanticAnalysis();
            }
            catch(SemanticException)
            {
                IsSemanticCorrect = false;
            }

            try
            {
                RightPart.SemanticAnalysis();
            }
            catch (SemanticException)
            {
                IsSemanticCorrect = false;
            }

            switch(AssignmentOperation)
            {
                case "=":
                    {
                        if ()
                        break;
                    }
            }
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
