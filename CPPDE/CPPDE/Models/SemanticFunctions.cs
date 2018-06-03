using C__DE.Models.Exceptions.SemanticExceptions;
using C__DE.Models.Warnings;
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
        public List<Variable> BlockVariables=new List<Variable>(); //Переменные блока

        //проверяется все ли переменные и их значения были использованы
        public void CheckVariables()
        {
            foreach (var BlockVar in BlockVariables)
            {
                try
                {
                    if (!BlockVar.WasUsed)
                        throw new UnusedVariableWarning(BlockVar.DeclaredLine, BlockVar.Name);
                    if (!BlockVar.WasNewValueUsed)
                        throw new UnusedValueWarning(BlockVar.WasAssignedNewValue, BlockVar.Name);
                }
                catch (WarningMessage e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }

    public abstract partial class AtomNode : Node
    {
    }

    public partial class ConstantNode : AtomNode
    {
        public override bool SemanticAnalysis()
        {
            IsSemanticCorrect = true;
            MainVariable.IsConst = true;
            MainVariable.Value = Value;
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
                if (possibleVariable != null)
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
#region old_functions
        /*
        public void CheckTypesAdd()//если оператор плюс или минус
        {
            switch (FirstOperand.MainVariable.Type)
            {
                case "int":
                    {
                        switch (SecondOperand.MainVariable.Type)
                        {
                            case "int":
                                {
                                    MainVariable.Type = "int";
                                    break;
                                }
                            case "float":
                                {
                                    MainVariable.Type = "float";
                                    break;
                                }
                            default: { throw new IncompatibleTypesException(LineNumber, "int", SecondOperand.MainVariable.Type); }
                        }
                        break;
                    }
                case "float":
                    {
                        switch (SecondOperand.MainVariable.Type)
                        {
                            case "int":
                            case "float":
                                {
                                    MainVariable.Type = "float";
                                    /*if (IsConst)
                                    {
                                        float first = float.Parse(FirstOperand.MainVariable.Value);
                                        float second = float.Parse(SecondOperand.MainVariable.Value);
                                        if (Value == "+")
                                            MainVariable.Value = (first + second).ToString();
                                        else //если минус
                                            MainVariable.Value = (first - second).ToString();
                                    }
                                    break;
                                }
                            default: { throw new IncompatibleTypesException(LineNumber, "int", SecondOperand.MainVariable.Type); }
                        }
                        break;
                    }
                case "string":
                    {
                        if (Value == "-")
                            throw new InvalidTypeException(LineNumber, "string", "-");
                        switch (SecondOperand.MainVariable.Type)
                        {
                            case "string":
                            case "char": {
                                    MainVariable.Type = "string";
                                    //if (IsConst)
                                       // MainVariable.Value = FirstOperand.MainVariable.Value + SecondOperand.MainVariable.Value;
                                    break;
                                }
                            default: { throw new IncompatibleTypesException(LineNumber, "string", SecondOperand.MainVariable.Type); }
                        }
                        break;
                    }
                case "char":
                    {
                        if (Value == "-")
                            throw new InvalidTypeException(LineNumber, "string", "-");
                        switch (SecondOperand.MainVariable.Type)
                        {
                            case "string":
                            case "char": {
                                    MainVariable.Type = "string";
                                    //if (IsConst)
                                      //  MainVariable.Value = FirstOperand.MainVariable.Value + SecondOperand.MainVariable.Value;
                                    break;
                                }
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
            //bool IsConst = FirstOperand.MainVariable.IsConst && SecondOperand.MainVariable.IsConst;
            switch (FirstOperand.MainVariable.Type)
            {
                case "int":
                    {
                        switch (SecondOperand.MainVariable.Type)
                        {
                            case "int":
                                {
                                    MainVariable.Type = "int";
                                    /*if (IsConst)
                                    {
                                        int first = int.Parse(FirstOperand.MainVariable.Value);
                                        int second = int.Parse(SecondOperand.MainVariable.Value);
                                        switch (Value)
                                        {
                                            case ("*"):
                                                {
                                                    MainVariable.Value = (first * second).ToString();
                                                    break;
                                                }
                                            case ("/"):
                                                {
                                                    try
                                                    {
                                                        if (second == 0)
                                                            throw new DividingByZeroWarning(LineNumber);
                                                        MainVariable.Value = (first / second).ToString();
                                                    }
                                                    catch (WarningMessage e)
                                                    {
                                                        Console.WriteLine(e.Message);
                                                    }
                                                    break;
                                                }
                                            case ("%"):
                                                {
                                                    try
                                                    {
                                                        if (second == 0)
                                                            throw new DividingByZeroWarning(LineNumber);
                                                        MainVariable.Value = (first / second).ToString();
                                                    }
                                                    catch (WarningMessage e)
                                                    {
                                                        Console.WriteLine(e.Message);
                                                    }
                                                    break;
                                                }
                                        }
                                    }
                                    break;
                                }
                            case "float":
                                {
                                    MainVariable.Type = "float";
                                    /*if (IsConst)
                                    {
                                        float first = float.Parse(FirstOperand.MainVariable.Value);
                                        float second = float.Parse(SecondOperand.MainVariable.Value);
                                        switch (Value)
                                        {
                                            case ("*"):
                                                {
                                                    MainVariable.Value = (first * second).ToString();
                                                    break;
                                                }
                                            case ("/"):
                                                {
                                                    try
                                                    {
                                                        if (second == 0)
                                                            throw new DividingByZeroWarning(LineNumber);
                                                        MainVariable.Value = (first / second).ToString();
                                                    }
                                                    catch (WarningMessage e)
                                                    {
                                                        Console.WriteLine(e.Message);
                                                    }
                                                    break;
                                                }
                                            case ("%"):
                                                {
                                                    try
                                                    {
                                                        if (second == 0)
                                                            throw new DividingByZeroWarning(LineNumber);
                                                        MainVariable.Value = (first / second).ToString();
                                                    }
                                                    catch (WarningMessage e)
                                                    {
                                                        Console.WriteLine(e.Message);
                                                    }
                                                    break;
                                                }
                                        }
                                    }
                                    break;
                                }
                            default: { throw new IncompatibleTypesException(LineNumber, "int", SecondOperand.MainVariable.Type); }
                        }
                        break;
                    }
                case "float":
                    {
                        switch (SecondOperand.MainVariable.Type)
                        {
                            case "int":
                            case "float":
                                {
                                    MainVariable.Type = "float";
                                    /*if (IsConst)
                                    {
                                        float first = float.Parse(FirstOperand.MainVariable.Value);
                                        float second = float.Parse(SecondOperand.MainVariable.Value);
                                        switch (Value)
                                        {
                                            case ("*"):
                                                {
                                                    MainVariable.Value = (first * second).ToString();
                                                    break;
                                                }
                                            case ("/"):
                                                {
                                                    try
                                                    {
                                                        if (second == 0)
                                                            throw new DividingByZeroWarning(LineNumber);
                                                        MainVariable.Value = (first / second).ToString();
                                                    }
                                                    catch (WarningMessage e)
                                                    {
                                                        Console.WriteLine(e.Message);
                                                    }
                                                    break;
                                                }
                                            case ("%"):
                                                {
                                                    try
                                                    {
                                                        if (second == 0)
                                                            throw new DividingByZeroWarning(LineNumber);
                                                        MainVariable.Value = (first / second).ToString();
                                                    }
                                                    catch (WarningMessage e)
                                                    {
                                                        Console.WriteLine(e.Message);
                                                    }
                                                    break;
                                                }
                                        }
                                    }
                                    break;
                                }
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
            {
                MainVariable.Type = "bool";
                bool IsConst = FirstOperand.MainVariable.IsConst && SecondOperand.MainVariable.IsConst;
                /*if (IsConst)
                {
                    if (type1=="int" || type1=="float")
                    {
                        float first = float.Parse(FirstOperand.MainVariable.Value);
                        float second = float.Parse(SecondOperand.MainVariable.Value);
                        switch (Value)
                        {
                            case (">"):
                                {
                                    MainVariable.Value = (first > second).ToString();
                                    break;
                                }
                            case (">="):
                                {
                                    MainVariable.Value = (first >= second).ToString();
                                    break;
                                }
                            case ("<"):
                                {
                                    MainVariable.Value = (first < second).ToString();
                                    break;
                                }
                            case ("<="):
                                {
                                    MainVariable.Value = (first <= second).ToString();
                                    break;
                                }
                            case ("=="):
                                {
                                    MainVariable.Value = (first == second).ToString();
                                    break;
                                }
                            case ("!="):
                                {
                                    MainVariable.Value = (first != second).ToString();
                                    break;
                                }
                        }
                    }
                    else if (type1=="char" || type1=="string")
                    {
                        string first = FirstOperand.MainVariable.Value;
                        string second = SecondOperand.MainVariable.Value;
                        switch (Value)
                        {
                            case (">"):
                                {
                                    MainVariable.Value = (String.Compare(first,second)>0).ToString();
                                    break;
                                }
                            case (">="):
                                {
                                    MainVariable.Value = (String.Compare(first, second) >= 0).ToString();
                                    break;
                                }
                            case ("<"):
                                {
                                    MainVariable.Value = (String.Compare(first, second)<0).ToString();
                                    break;
                                }
                            case ("<="):
                                {
                                    MainVariable.Value = (String.Compare(first, second) <= 0).ToString();
                                    break;
                                }
                            case ("=="):
                                {
                                    MainVariable.Value = (String.Equals(first, second)).ToString();
                                    break;
                                }
                            case ("!="):
                                {
                                    MainVariable.Value = (!String.Equals(first, second)).ToString();
                                    break;
                                }
                        }
                    }
                    else //иначе булевский
                    {
                        bool first = bool.Parse(FirstOperand.MainVariable.Value);
                        bool second = bool.Parse(SecondOperand.MainVariable.Value);
                        switch (Value)
                        {
                            case (">"):
                                {
                                    MainVariable.Value = (first && !second).ToString();
                                    break;
                                }
                            case (">="):
                                {
                                    MainVariable.Value = (first).ToString();
                                    break;
                                }
                            case ("<"):
                                {
                                    MainVariable.Value = (!first && second).ToString();
                                    break;
                                }
                            case ("<="):
                                {
                                    MainVariable.Value = (second).ToString();
                                    break;
                                }
                            case ("=="):
                                {
                                    MainVariable.Value = ((first && second) || (!first && !second)).ToString();
                                    break;
                                }
                            case ("!="):
                                {
                                    MainVariable.Value = ((!first && second) || (first && !second)).ToString();
                                    break;
                                }
                        }
                    }
                }
            }
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
        }*/
#endregion


        public override bool SemanticAnalysis()
        {
            //будем считать, что определено
            MainVariable = new Variable();
            MainVariable.WasIdentified = true;

            try
            {
                IsSemanticCorrect = FirstOperand.SemanticAnalysis();
                FirstOperand.MainVariable.WasUsed = true;
                IsSemanticCorrect =FirstOperand.SemanticAnalysis();
                FirstOperand.MainVariable.WasNewValueUsed = true;
                if (!FirstOperand.MainVariable.WasIdentified)
                    throw new UnidentifiedVariableException(FirstOperand.LineNumber, FirstOperand.MainVariable.Name);
            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }

            if (IsUnary)
                if (IsSemanticCorrect)
                {
                    if (Value == "-")
                        if (FirstOperand.MainVariable.Type == "int")
                        {
                            MainVariable.Type = FirstOperand.MainVariable.Type; //если унарный, то тип совпадает с типом операнда
                            return true;
                        }
                        else throw new InvalidTypeException(LineNumber, FirstOperand.MainVariable.Type, Value);
                    if (Value == "!")
                    {
                        if (FirstOperand.MainVariable.Type == "bool")
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
                try
                {
                    IsSemanticCorrect &= SecondOperand.SemanticAnalysis();
                    SecondOperand.MainVariable.WasUsed = true;
                    SecondOperand.MainVariable.WasNewValueUsed = true;
                    if (!SecondOperand.MainVariable.WasIdentified)
                        throw new UnidentifiedVariableException(SecondOperand.LineNumber, SecondOperand.MainVariable.Name);
                }
                catch (SemanticException e)
                {
                    Console.WriteLine(e.Message);
                    IsSemanticCorrect = false;
                }
                if (IsSemanticCorrect)
                {
                    try
                    {
                        switch (TypeOfNode)
                        {
                            case NodeType.ArithmeticOperator:
                                {
                                    if (FirstOperand.MainVariable.Type != "int" || SecondOperand.MainVariable.Type != "int")
                                        throw new IncompatibleTypesException(LineNumber, FirstOperand.MainVariable.Type, SecondOperand.MainVariable.Type);
                                    MainVariable.Type = "int";
                                    break;
                                }
                            case NodeType.ComparisonOperator:
                                {
                                    if (FirstOperand.MainVariable.Type != "int" || SecondOperand.MainVariable.Type != "int")
                                        throw new IncompatibleTypesException(LineNumber, FirstOperand.MainVariable.Type, SecondOperand.MainVariable.Type);
                                    MainVariable.Type = "int";
                                    break;
                                }
                            case NodeType.BitOperator:
                                {
                                    if (FirstOperand.MainVariable.Type != "int" || SecondOperand.MainVariable.Type != "int")
                                        throw new IncompatibleTypesException(LineNumber, FirstOperand.MainVariable.Type, SecondOperand.MainVariable.Type);
                                    MainVariable.Type = "int";
                                    break;
                                }
                            case NodeType.LogicalOperator:
                                {
                                    if (FirstOperand.MainVariable.Type != "bool" || SecondOperand.MainVariable.Type != "bool")
                                        throw new IncompatibleTypesException(LineNumber, FirstOperand.MainVariable.Type, SecondOperand.MainVariable.Type);
                                    MainVariable.Type = "bool";
                                    break;
                                }
                        }
                        return true;
                    }
                    catch (SemanticException e)
                    {
                        Console.WriteLine(e.Message);
                        IsSemanticCorrect = false;
                        return false;
                    }
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
            MainVariable.DeclaredLine = LineNumber;
            MainVariable.Type = Type;
            MainVariable.WasIdentified = false;
            Counters.vars++;

            //нужно посмотреть повторное объявление
            try
            {
                Variable possibleVariable = parentBlock.BlockVariables.FirstOrDefault(var => var.Name == DeclaratedVariable.Value);
                if (possibleVariable!= null)
                {
                    IsSemanticCorrect = false;
                    possibleVariable.Type = Type; //меняем тип
                    throw new RedeclaringVariableException(LineNumber, DeclaratedVariable.Value);
                }
                parentBlock.BlockVariables.Add(MainVariable); //помещаем в список переменных данного блока
                IsSemanticCorrect = true;
            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false; //плохо
            }

            DeclaratedVariable.SemanticAnalysis();
            return IsSemanticCorrect;
        }

    }

    public partial class ConditionalOperatorNode : BlockNode
    {
        public override bool SemanticAnalysis()
        {
            IsSemanticCorrect = true;
            try
            {
                IsSemanticCorrect &= Condition.SemanticAnalysis();
            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }
 
            try
            {
                IsSemanticCorrect &= IfBranch.SemanticAnalysis();
            }
            catch
            {
                IsSemanticCorrect = false;
            }

            if (ElseBranch != null)
            {
                try
                {
                    IsSemanticCorrect = ElseBranch.SemanticAnalysis();
                }
                catch (SemanticException e)
                {
                    Console.WriteLine(e.Message);
                    IsSemanticCorrect = false;
                }
            }

            IfBranch.CheckVariables();
            if (ElseBranch != null)
                ElseBranch.CheckVariables();
            return IsSemanticCorrect;
        }
    }

    public partial class ConditionalBranchNode : BlockNode//какая-то из веток if или else
    {
        public override bool SemanticAnalysis()
        {
            IsSemanticCorrect = true;
            foreach (var oper in ChildrenOperators)
            {
                try
                {
                    IsSemanticCorrect &= oper.SemanticAnalysis();
                }
                catch (SemanticException e)
                {
                    Console.WriteLine(e.Message);
                    IsSemanticCorrect = false;
                }
            }
            return IsSemanticCorrect;
        }
    }

    public partial class CycleOperator : BlockNode
    {
        public override bool SemanticAnalysis()
        {
            IsSemanticCorrect = true;
            if (BeginningActivity!=null)
            {
                foreach (var Act in BeginningActivity)
                {
                    try
                    {
                        IsSemanticCorrect &= Act.SemanticAnalysis();
                    }
                    catch (SemanticException e)
                    {
                        Console.WriteLine(e.Message);
                        IsSemanticCorrect = false;
                    }
                }
            }

            if (IsPredCondition)
            {
                if (ContinueCondition != null)
                {
                    try
                    {
                        IsSemanticCorrect &= ContinueCondition.SemanticAnalysis();
                    }
                    catch (SemanticException e)
                    {
                        Console.WriteLine(e.Message);
                        IsSemanticCorrect = false;
                    }
                }
            }

            if (IterationActivity!=null)
            {
                try
                {
                    IsSemanticCorrect &= IterationActivity.SemanticAnalysis();
                }
                catch (SemanticException e)
                {
                    Console.WriteLine(e.Message);
                    IsSemanticCorrect = false;
                }
            }

            foreach(var oper in ChildrenOperators)
            {
                try
                {
                    IsSemanticCorrect &= oper.SemanticAnalysis();
                }
                catch (SemanticException e)
                {
                    Console.WriteLine(e.Message);
                    IsSemanticCorrect = false;
                }
            }

            if (ContinueCondition!=null) //ещё раз проверить условие, так как оно проверятся на каждой итерации
            {
                try
                {
                    IsSemanticCorrect &= ContinueCondition.SemanticAnalysis();
                }
                catch (SemanticException e)
                {
                    Console.WriteLine(e.Message);
                    IsSemanticCorrect = false;
                }
            }
            CheckVariables();
            return IsSemanticCorrect;
        }
    }

    public partial class AssignmentOperator : AtomNode
    {
        #region old_2
        public void CheckTypesAssign()
        {
            switch (AssignedVariable.MainVariable.Type)
            {
                case "int":
                    {
                        if (RightPart.MainVariable.Type == "int")
                        {
                            MainVariable.Type = "int";
                            break;
                        }
                        else
                        {
                            throw new IncompatibleTypesException(LineNumber, "int", RightPart.MainVariable.Type);
                        }

                    }
                case "float":
                    {
                        if(RightPart.MainVariable.Type == "int" || RightPart.MainVariable.Type == "float")
                        {
                            MainVariable.Type = "float";
                            break;
                        }
                        else
                        {
                            throw new IncompatibleTypesException(LineNumber, "float", RightPart.MainVariable.Type);
                        }
                    }
                case "string":
                    {
                        if (RightPart.MainVariable.Type == "string" || RightPart.MainVariable.Type == "char")
                        {
                            MainVariable.Type = "string";
                            break;
                        }
                        else
                            throw new IncompatibleTypesException(LineNumber, "string", RightPart.MainVariable.Type);
                    }
                default:
                    {
                        if (AssignedVariable.MainVariable.Type==RightPart.MainVariable.Type)
                        {
                            MainVariable.Type = AssignedVariable.MainVariable.Type;
                            break;
                        }
                        else
                            throw new InvalidTypeException(LineNumber, AssignedVariable.MainVariable.Type, AssignmentOperation);
                    }
            }
        }

        public void CheckTypesAdd()
        {
            switch (AssignedVariable.MainVariable.Type)
            {
                case "int":
                    {
                        if (RightPart.MainVariable.Type == "int")
                        {
                            MainVariable.Type = "int";
                            break;
                        }
                        else
                        {
                            throw new IncompatibleTypesException(LineNumber, "int", RightPart.MainVariable.Type);
                        }

                    }
                case "float":
                    {
                        if (RightPart.MainVariable.Type == "int" || RightPart.MainVariable.Type == "float")
                        {
                            MainVariable.Type = "float";
                            break;
                        }
                        else
                        {
                            throw new IncompatibleTypesException(LineNumber, "float", RightPart.MainVariable.Type);
                        }
                    }
                case "string":
                    {
                        if (RightPart.MainVariable.Type == "string" || RightPart.MainVariable.Type == "char")
                        {
                            MainVariable.Type = "string";
                            break;
                        }
                        else
                            throw new IncompatibleTypesException(LineNumber, "string", RightPart.MainVariable.Type);
                    }
                default:
                    {
                        throw new InvalidTypeException(LineNumber, AssignedVariable.MainVariable.Type, AssignmentOperation);
                    }
            }
        }

        public void CheckTypesArithmetic()
        {
            switch (AssignedVariable.MainVariable.Type)
            {
                case "int":
                    {
                        if (RightPart.MainVariable.Type == "int")
                        {
                            MainVariable.Type = "int";
                            break;
                        }
                        else
                        {
                            throw new IncompatibleTypesException(LineNumber, "int", RightPart.MainVariable.Type);
                        }
                    }
                case "float":
                    {
                        if (RightPart.MainVariable.Type == "int" || RightPart.MainVariable.Type == "float")
                        {
                            MainVariable.Type = "float";
                            break;
                        }
                        else
                        {
                            throw new IncompatibleTypesException(LineNumber, "float", RightPart.MainVariable.Type);
                        }
                    }
                default:
                    {
                        throw new InvalidTypeException(LineNumber, AssignedVariable.MainVariable.Type, AssignmentOperation);
                    }
            }
        }

        public void ChechTypesLogical()
        {
            if (AssignedVariable.MainVariable.Type == "bool")
                if (RightPart.MainVariable.Type == "bool")
                {
                    MainVariable.Type = "bool";
                }
                else
                {
                    throw new InvalidTypeException(LineNumber, "bool", RightPart.MainVariable.Type);
                }
            throw new InvalidTypeException(LineNumber, AssignedVariable.MainVariable.Type, AssignmentOperation);
        }
        #endregion

        public override bool SemanticAnalysis()
        {
            IsSemanticCorrect = true;
            MainVariable = new Variable();
            try
            {
                IsSemanticCorrect &= AssignedVariable.SemanticAnalysis();
                AssignedVariable.MainVariable.WasNewValueUsed = false;
                AssignedVariable.MainVariable.WasAssignedNewValue = LineNumber;
                AssignedVariable.MainVariable.WasUsed = true;
                MainVariable = AssignedVariable.MainVariable;
            }
            catch(SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }

            try
            {
                if(RightPart!=null)
                    IsSemanticCorrect &= RightPart.SemanticAnalysis();
            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }

            if (IsSemanticCorrect)
                try
                {
                    switch (AssignmentOperation)
                    {
                        case "=":
                            {
                                if (AssignedVariable.MainVariable.Type != RightPart.MainVariable.Type)
                                    throw new IncompatibleTypesException(LineNumber, AssignedVariable.MainVariable.Type, RightPart.MainVariable.Type);
                                break;
                            }
                        case "+=":
                        case "-=":
                        case "*=":
                        case "/=":
                        case "%=":
                            {
                                
                                break;
                            }
                        case "&&=":
                        case "||=":
                            {
                                if (AssignedVariable.MainVariable.Type!="bool" || RightPart.MainVariable.Type!="bool")
                                    throw new IncompatibleTypesException(LineNumber, AssignedVariable.MainVariable.Type, RightPart.MainVariable.Type);
                                break;
                            }
                        case "++":
                        case "--":
                            {
                                if (MainVariable.Type != "int")
                                    throw new InvalidTypeException(LineNumber, MainVariable.Type, AssignmentOperation);
                                break;
                            }
                        default:
                            {
                                return false;
                            }
                    }
                    return true; //если всё хорошо, то true
                }
                catch (SemanticException e)
                {
                    Console.WriteLine(e.Message);
                    IsSemanticCorrect = false;
                    return false;
                }
            else
                return false;
        }
    }

    public partial class MainRootNode : BlockNode
    {
        public override bool SemanticAnalysis()
        {
            IsSemanticCorrect = true;
            foreach (var oper in ChildrenOperators)
            {
                try
                {
                    IsSemanticCorrect &= oper.SemanticAnalysis();
                }
                catch (SemanticException e)
                {
                    Console.WriteLine(e.Message);
                    IsSemanticCorrect = false;
                }
            }
            CheckVariables();
            return IsSemanticCorrect;
        }
    }

    public partial class ReadOperator : AtomNode
    {
        public override bool SemanticAnalysis()
        {
            try
            {
                IsSemanticCorrect = ReadVariable.SemanticAnalysis();
                ReadVariable.MainVariable.WasIdentified = true;
                ReadVariable.MainVariable.WasNewValueUsed = false;
                ReadVariable.MainVariable.WasUsed = true;
                ReadVariable.MainVariable.WasAssignedNewValue = LineNumber;
            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }
            return IsSemanticCorrect;
        }
    }

    public partial class WriteOperator : AtomNode
    {
        public override bool SemanticAnalysis()
        {
            try
            {
                IsSemanticCorrect = WriteVariable.SemanticAnalysis();
                WriteVariable.MainVariable.WasUsed = true;
                WriteVariable.MainVariable.WasNewValueUsed = true;
                if (!WriteVariable.MainVariable.WasIdentified)
                    throw new UnidentifiedVariableException(WriteVariable.LineNumber, WriteVariable.MainVariable.Name);
            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }
            return IsSemanticCorrect;
        }
    }
}
