using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models
{
    public static class Counters
    {
        public static int vars = 0; //переменные посчитаны в семантическом анализаторе
        public static int consts = 0; //константы можно писать в лоб
        public static int temps = 0; //временные переменные, будут посчитаны тут
        public static int ifs = 0; //условные операторы, счётчик нужен для нумерации веток
        public static int cycles = 0; //циклы, то же самое
        public static int comparsions = 0; //сравнния, там тоже нужны метки для присваивания значений
        public static int graphs = 0; //графы, считаем их отдельно для удобства
        public static int matricies = 0; //матрицы для промежуточных вычислений при работе с графами
        public static int arrays = 0;
    }

    //статический класс для хранения промежуточного кода, возможно, будет пополняться
    public static class IntermediateCodeList
    {
        public static List<IntermediateCodeNode> IntermediateList= new List<IntermediateCodeNode>(); //Список промежуточных узлов
        public static List<Variable> AllVariables = new List<Variable>(); //все переменные программы
        public static void push(IntermediateCodeNode Command)
        {
            IntermediateList.Add(Command);
        }
        public static void addVar(Variable v)
        {
            //переменные будут записаны в отдельном сегменте
            AllVariables.Add(v);
        }
    }

    public abstract partial class Node
    {
        public abstract void GenerateIntermediateCode(); //генерация промежуточного кода
    }

    public partial class ConstantNode
    {
        public override void GenerateIntermediateCode()
        {
            
        }
    }

    public partial class VariableNode
    {
        public override void GenerateIntermediateCode()
        {
            //переменная где-то объявлена уже, так что ту ничего
        }
    }

    public partial class BinaryOperatorNode
    {
        public override void GenerateIntermediateCode()
        {
            MainVariable.AlternativeName = "temp_"+(++Counters.temps).ToString(); //присваиваем временной переменной имя 
            IntermediateCodeList.addVar(MainVariable); //временную переменную - в список переменных
            FirstOperand.GenerateIntermediateCode();
            //вот тут самое веселье
            if (IsUnary)
            {
                //из унарных получается только унарный минус и отрицание
                switch (Value)
                {
                    case ("!"):
                        {
                            IntermediateCodeList.push(new NegativeInterNode(FirstOperand.MainVariable, MainVariable));
                            break;
                        }
                    case ("-"):
                        {
                            IntermediateCodeList.push(new UnaryMinusInterNode(FirstOperand.MainVariable, MainVariable));
                            break;
                        }
                }
            }
            else
            {
                SecondOperand.GenerateIntermediateCode();
                switch (Value)
                {
                    case ("+"):
                        {
                            IntermediateCodeList.push(new AddInterNode(FirstOperand.MainVariable,SecondOperand.MainVariable ,MainVariable));
                            break;
                        }
                    case ("-"):
                        {
                            IntermediateCodeList.push(new SubInterNode(FirstOperand.MainVariable, SecondOperand.MainVariable, MainVariable));
                            break;
                        }
                    case ("*"):
                        {
                            IntermediateCodeList.push(new MulInterNode(FirstOperand.MainVariable, SecondOperand.MainVariable, MainVariable));
                            break;
                        }
                    case ("/"):
                        {
                            IntermediateCodeList.push(new DivInterNode(FirstOperand.MainVariable, SecondOperand.MainVariable, MainVariable));
                            break;
                        }
                    case ("%"):
                        {
                            IntermediateCodeList.push(new ModInterNode(FirstOperand.MainVariable, SecondOperand.MainVariable, MainVariable));
                            break;
                        }
                    case ("&&"):
                        {
                            IntermediateCodeList.push(new AndInterNode(FirstOperand.MainVariable, SecondOperand.MainVariable, MainVariable));
                            break;
                        }
                    case ("||"):
                        {
                            IntermediateCodeList.push(new OrInterNode(FirstOperand.MainVariable, SecondOperand.MainVariable, MainVariable));
                            break;
                        }
                    case ("&"):
                        {
                            IntermediateCodeList.push(new BitAndInterNode(FirstOperand.MainVariable, SecondOperand.MainVariable, MainVariable));
                            break;
                        }
                    case ("|"):
                        {
                            IntermediateCodeList.push(new BitOrInterNode(FirstOperand.MainVariable, SecondOperand.MainVariable, MainVariable));
                            break;
                        }
                    case ("^"):
                        {
                            IntermediateCodeList.push(new BitXorInterNode(FirstOperand.MainVariable, SecondOperand.MainVariable, MainVariable));
                            break;
                        }
                    //с операторами сравнения посложнее
                    case ("=="):
                        {
                            //сравниваем два операнда
                            IntermediateCodeList.push(new CmpNode(FirstOperand.MainVariable, SecondOperand.MainVariable));
                            //если не равно, то переходим дальше
                            IntermediateCodeList.push(new GoToLabel("comp_label_" + (++Counters.comparsions).ToString(), "jne"));
                            //если равно, то идём дальше
                            IntermediateCodeList.push(new AssignmentInterNode(new ConstantNode("bool", "true", 0).MainVariable, MainVariable));
                            //присвоили и уходим
                            IntermediateCodeList.push(new GoToLabel("exit_comp_label_" + (Counters.comparsions).ToString(), "jmp"));
                            //если не равно, идём сюда
                            IntermediateCodeList.push(new PutLabel("comp_label_" + (Counters.comparsions).ToString()));
                            //присваиваем false
                            IntermediateCodeList.push(new AssignmentInterNode(new ConstantNode("bool", "false", 0).MainVariable, MainVariable));
                            //и на выход
                            IntermediateCodeList.push(new PutLabel("exit_comp_label_"+(Counters.comparsions).ToString()));
                            break;
                        }
                    //с другими операторами сравнения аналогично, только условие другое
                    case ("!="):
                        {
                            //сравниваем два операнда
                            IntermediateCodeList.push(new CmpNode(FirstOperand.MainVariable, SecondOperand.MainVariable));
                            //если равно, то переходим дальше
                            IntermediateCodeList.push(new GoToLabel("comp_label_" + (++Counters.comparsions).ToString(), "je"));
                            //иначе, если всё хорошо, то присваиваем true
                            IntermediateCodeList.push(new AssignmentInterNode(new ConstantNode("bool", "true", 0).MainVariable, MainVariable));
                            //присвоили и уходим
                            IntermediateCodeList.push(new GoToLabel("exit_comp_label_" + (Counters.comparsions).ToString(), "jmp"));
                            //если не равно, идём сюда
                            IntermediateCodeList.push(new PutLabel("comp_label_" + (Counters.comparsions).ToString()));
                            //присваиваем false
                            IntermediateCodeList.push(new AssignmentInterNode(new ConstantNode("bool", "false", 0).MainVariable, MainVariable));
                            //и на выход
                            IntermediateCodeList.push(new PutLabel("exit_comp_label_" + (Counters.comparsions).ToString()));
                            break;
                        }
                    case (">="):
                        {
                            //сравниваем два операнда
                            IntermediateCodeList.push(new CmpNode(FirstOperand.MainVariable, SecondOperand.MainVariable));
                            //если меньше, то переходим дальше
                            IntermediateCodeList.push(new GoToLabel("comp_label_" + (++Counters.comparsions).ToString(), "jl"));
                            //иначе, если всё хорошо, то присваиваем true
                            IntermediateCodeList.push(new AssignmentInterNode(new ConstantNode("bool", "true", 0).MainVariable, MainVariable));
                            //присвоили и уходим
                            IntermediateCodeList.push(new GoToLabel("exit_comp_label_" + (Counters.comparsions).ToString(), "jmp"));
                            //если не равно, идём сюда
                            IntermediateCodeList.push(new PutLabel("comp_label_" + (Counters.comparsions).ToString()));
                            //присваиваем false
                            IntermediateCodeList.push(new AssignmentInterNode(new ConstantNode("bool", "false", 0).MainVariable, MainVariable));
                            //и на выход
                            IntermediateCodeList.push(new PutLabel("exit_comp_label_" + (Counters.comparsions).ToString()));
                            break;
                        }
                    case ("<="):
                        {
                            //сравниваем два операнда
                            IntermediateCodeList.push(new CmpNode(FirstOperand.MainVariable, SecondOperand.MainVariable));
                            //если больше, то переходим дальше
                            IntermediateCodeList.push(new GoToLabel("comp_label_" + (++Counters.comparsions).ToString(), "jg"));
                            //иначе, если всё хорошо, то присваиваем true
                            IntermediateCodeList.push(new AssignmentInterNode(new ConstantNode("bool", "true", 0).MainVariable, MainVariable));
                            //присвоили и уходим
                            IntermediateCodeList.push(new GoToLabel("exit_comp_label_" + (Counters.comparsions).ToString(), "jmp"));
                            //если не равно, идём сюда
                            IntermediateCodeList.push(new PutLabel("comp_label_" + (Counters.comparsions).ToString()));
                            //присваиваем false
                            IntermediateCodeList.push(new AssignmentInterNode(new ConstantNode("bool", "false", 0).MainVariable, MainVariable));
                            //и на выход
                            IntermediateCodeList.push(new PutLabel("exit_comp_label_" + (Counters.comparsions).ToString()));
                            break;
                        }
                    case ("<"):
                        {
                            //сравниваем два операнда
                            IntermediateCodeList.push(new CmpNode(FirstOperand.MainVariable, SecondOperand.MainVariable));
                            //если больше, то переходим дальше
                            IntermediateCodeList.push(new GoToLabel("comp_label_" + (++Counters.comparsions).ToString(), "jge"));
                            //иначе, если всё хорошо, то присваиваем true
                            IntermediateCodeList.push(new AssignmentInterNode(new ConstantNode("bool", "true", 0).MainVariable, MainVariable));
                            //присвоили и уходим
                            IntermediateCodeList.push(new GoToLabel("exit_comp_label_" + (Counters.comparsions).ToString(), "jmp"));
                            //если не равно, идём сюда
                            IntermediateCodeList.push(new PutLabel("comp_label_" + (Counters.comparsions).ToString()));
                            //присваиваем false
                            IntermediateCodeList.push(new AssignmentInterNode(new ConstantNode("bool", "false", 0).MainVariable, MainVariable));
                            //и на выход
                            IntermediateCodeList.push(new PutLabel("exit_comp_label_"+(Counters.comparsions).ToString()));
                            break;
                        }
                    case (">"):
                        {
                            //сравниваем два операнда
                            IntermediateCodeList.push(new CmpNode(FirstOperand.MainVariable, SecondOperand.MainVariable));
                            //если больше, то переходим дальше
                            IntermediateCodeList.push(new GoToLabel("comp_label_" + (++Counters.comparsions).ToString(), "jle"));
                            //иначе, если всё хорошо, то присваиваем true
                            IntermediateCodeList.push(new AssignmentInterNode(new ConstantNode("bool", "true", 0).MainVariable, MainVariable));
                            //присвоили и уходим
                            IntermediateCodeList.push(new GoToLabel("exit_comp_label_" + (Counters.comparsions).ToString(), "jmp"));
                            //если не равно, идём сюда
                            IntermediateCodeList.push(new PutLabel("comp_label_" + (Counters.comparsions).ToString()));
                            //присваиваем false
                            IntermediateCodeList.push(new AssignmentInterNode(new ConstantNode("bool", "false", 0).MainVariable, MainVariable));
                            //и на выход
                            IntermediateCodeList.push(new PutLabel("exit_comp_label_"+(Counters.comparsions).ToString()));
                            break;
                        }
                }
            }
        }
    }

    public partial class VariableDeclarationNode
    {
        public override void GenerateIntermediateCode()
        {
            //добавили переменную
            MainVariable.IsConst = false;//не константа
            IntermediateCodeList.addVar(MainVariable);
        }
    }

    public partial class ConditionalOperatorNode
    {
        public override void GenerateIntermediateCode()
        {
            //номер условного оператора, нужен для формирования меток
            int num_if = ++Counters.ifs;
            Condition.GenerateIntermediateCode();
            //то, что получилось, сравниваем с нулём
            IntermediateCodeList.push(new CmpNode(Condition.MainVariable, new ConstantNode("int", "0", 0).MainVariable));
            if (ElseBranch == null)
                //если else-ветки нет, формируем метку на выход, если условие не выполнено
                //если равно нулю, то условие ложно и уходим
                IntermediateCodeList.push(new GoToLabel("exit_if_" + num_if.ToString(), "je"));
            else
                //если вторая ветка есть, формируем метку на неё
                IntermediateCodeList.push(new GoToLabel("else_" + num_if.ToString(), "je"));
            //генерим код для if-ветки, она будет в любом случае
            IfBranch.GenerateIntermediateCode();
            //если есть вторая ветка, надо сгенерить код для неё
            if (ElseBranch!=null)
            {
                //из if-ветки отправляем на выход
                IntermediateCodeList.push(new GoToLabel("exit_if_" + num_if.ToString(), "jmp"));
                //ставим метку else
                IntermediateCodeList.push(new PutLabel("else_" + num_if.ToString()));
                //Генерируем код else-ветки
                ElseBranch.GenerateIntermediateCode();
            }
            //ставим метку на выход
            IntermediateCodeList.push(new PutLabel("exit_if_" + num_if.ToString()));
        }
    }

    public partial class CycleOperator
    {
        public override void GenerateIntermediateCode()
        {
            int num_cycle = ++Counters.cycles;
            if (BeginningActivity != null)
                foreach (var oper in BeginningActivity)
                    oper.GenerateIntermediateCode();

            //ставим метку - сюда будем возвращаться после каждой итерации
            IntermediateCodeList.push(new PutLabel("again_cycle_" + num_cycle.ToString()));
            if (IsPredCondition)
            {
                ContinueCondition.GenerateIntermediateCode();
                //то, что получилось, сравниваем с нулём
                IntermediateCodeList.push(new CmpNode(ContinueCondition.MainVariable, new ConstantNode("int", "0", 0).MainVariable));
                //если ложь, то на выход
                IntermediateCodeList.push(new GoToLabel("exit_cycle_" + num_cycle.ToString(), "je"));
            }
            
            //генерим код для тела цикла
            foreach (var oper in ChildrenOperators)
                oper.GenerateIntermediateCode();
            //если есть последующее действие - генерим код для него
            if (IterationActivity != null)
                IterationActivity.GenerateIntermediateCode();
            //если цикл с постусловием, то генерить здесть
            if (!IsPredCondition)
            {
                ContinueCondition.GenerateIntermediateCode();
                //то, что получилось, сравниваем с нулём
                IntermediateCodeList.push(new CmpNode(ContinueCondition.MainVariable, new ConstantNode("int", "0", 0).MainVariable));
                //если ложь, то на выход
                IntermediateCodeList.push(new GoToLabel("exit_cycle_" + num_cycle.ToString(), "je"));
            }
            //переход на следующую итерацию
            IntermediateCodeList.push(new GoToLabel("again_cycle_" + num_cycle.ToString(), "jmp"));
            //последняя метка - на выход
            IntermediateCodeList.push(new PutLabel("exit_cycle_" + num_cycle.ToString()));
        }
    }

    public partial class ConditionalBranchNode
    {
        public override void GenerateIntermediateCode()
        {
            //это протсо последовательность операторов, тупо генерим их подряд
            foreach (var oper in ChildrenOperators)
                oper.GenerateIntermediateCode();
        }
    }

    public partial class AssignmentOperator
    {
        public override void GenerateIntermediateCode()
        {
            if (RightPart != null)
                RightPart.GenerateIntermediateCode();
            switch (AssignmentOperation)
            {
                case ("="):
                    {
                        IntermediateCodeList.push(new AssignmentInterNode(RightPart.MainVariable, MainVariable));
                        break;
                    }
                case ("+="):
                    {
                        IntermediateCodeList.push(new AddInterNode(AssignedVariable.MainVariable, RightPart.MainVariable, AssignedVariable.MainVariable));
                        break;
                    }
                case ("-="):
                    {
                        IntermediateCodeList.push(new SubInterNode(AssignedVariable.MainVariable, RightPart.MainVariable, AssignedVariable.MainVariable));
                        break;
                    }
                case ("*="):
                    {
                        IntermediateCodeList.push(new MulInterNode(AssignedVariable.MainVariable, RightPart.MainVariable, AssignedVariable.MainVariable));
                        break;
                    }
                case ("/="):
                    {
                        IntermediateCodeList.push(new DivInterNode(AssignedVariable.MainVariable, RightPart.MainVariable, AssignedVariable.MainVariable));
                        break;
                    }
                case ("%="):
                    {
                        IntermediateCodeList.push(new ModInterNode(AssignedVariable.MainVariable, RightPart.MainVariable, AssignedVariable.MainVariable));
                        break;
                    }
                case ("&&="):
                    {
                        IntermediateCodeList.push(new AndInterNode(AssignedVariable.MainVariable, RightPart.MainVariable, AssignedVariable.MainVariable));
                        break;
                    }
                case ("||="):
                    {
                        IntermediateCodeList.push(new OrInterNode(AssignedVariable.MainVariable, RightPart.MainVariable, AssignedVariable.MainVariable));
                        break;
                    }
                case ("++"):
                    {
                        IntermediateCodeList.push(new IncrementInterNode(AssignedVariable.MainVariable));
                        break;
                    }
                case ("--"):
                    {
                        IntermediateCodeList.push(new DecrementInterNode(AssignedVariable.MainVariable));
                        break;
                    }
            }
        }
    }

    public partial class ReadOperator
    {
        public override void GenerateIntermediateCode()
        {
            IntermediateCodeList.push(new ReadVarInterNode(MainVariable));
        }
    }

    public partial class WriteOperator
    {
        public override void GenerateIntermediateCode()
        {
            //тут может быть выражение
            WriteVariable.GenerateIntermediateCode();
            IntermediateCodeList.push(new WriteVarInterNode(MainVariable));
        }
    }

    public partial class MainRootNode
    {
        public override void GenerateIntermediateCode()
        {
            // это протсо последовательность операторов, тупо генерим их подряд
            foreach (var oper in ChildrenOperators)
                oper.GenerateIntermediateCode();
        }
    }
}
