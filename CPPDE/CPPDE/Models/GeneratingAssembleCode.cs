using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models
{
    //генерация для каждого типа узла
    public partial class IntermediateCodeNode
    {
        //будет отдельно прописано для каждого типа узла
        public abstract void GenerateAsmCode();
    }

    public partial class AddInterNode
    {
        //надо ли сохранять eax в стек?
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("mov eax, {0}", FirstOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("add eax, {0}", SecondOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("mov {0}, eax", result.AlternativeName);
        }
    }

    public partial class SubInterNode
    {
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("mov eax, {0}", FirstOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("sub eax, {0}", SecondOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("mov {0}, eax", result.AlternativeName);
        }
    }

    public partial class MulInterNode
    {
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("mov eax, {0}", FirstOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("mov ebx, {0}", SecondOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("mul ebx");
            //если переполнение - отсекаем всё, что вывалилось
            GeneratingAssembleCode.outFile.WriteLine("mov {0}, eax", result.AlternativeName);
        }
    }

    public partial class DivInterNode
    {
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("mov eax, {0}", FirstOperand.AlternativeName);
            //сохраняем edx, вдруг там что-то важное?
            GeneratingAssembleCode.outFile.WriteLine("push edx");
            GeneratingAssembleCode.outFile.WriteLine("mov edx, 0");
            GeneratingAssembleCode.outFile.WriteLine("mov ebx, {0}", SecondOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("div ebx");
            GeneratingAssembleCode.outFile.WriteLine("mov {0}, eax", result.AlternativeName);
            //восстановить edx
            GeneratingAssembleCode.outFile.WriteLine("pop edx");
        }
    }

    public partial class ModInterNode
    {
        //так же, только сохраняем остаток
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("mov eax, {0}", FirstOperand.AlternativeName);
            //сохраняем edx, вдруг там что-то важное?
            GeneratingAssembleCode.outFile.WriteLine("push edx");
            GeneratingAssembleCode.outFile.WriteLine("mov edx, 0");
            GeneratingAssembleCode.outFile.WriteLine("mov ebx, {0}", SecondOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("div ebx");
            GeneratingAssembleCode.outFile.WriteLine("mov {0}, edx", result.AlternativeName);
            //восстановить edx
            GeneratingAssembleCode.outFile.WriteLine("pop edx");
        }
    }

    //логические переменные - однобайтовые
    public partial class AndInterNode
    {
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("mov al, {0}", FirstOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("mov bl, {0}", SecondOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("and al, bl");
            GeneratingAssembleCode.outFile.WriteLine("mov {0}, al", result.AlternativeName);
        }
    }

    public partial class OrInterNode
    {
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("mov al, {0}", FirstOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("mov bl, {0}", SecondOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("or al, bl");
            GeneratingAssembleCode.outFile.WriteLine("mov {0}, al", result.AlternativeName);
        }
    }

    //дальше побитовые операции - для целых чисел, четырёхбайтовые
    public partial class BitAndInterNode
    {
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("mov eax, {0}", FirstOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("mov ebx, {0}", SecondOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("and eax, ebx");
            GeneratingAssembleCode.outFile.WriteLine("mov {0}, eax", result.AlternativeName);
        }
    }

    public partial class BitOrInterNode
    {
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("mov eax, {0}", FirstOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("mov ebx, {0}", SecondOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("or eax, ebx");
            GeneratingAssembleCode.outFile.WriteLine("mov {0}, eax", result.AlternativeName);
        }
    }

    public partial class BitXorInterNode
    {
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("mov eax, {0}", FirstOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("mov ebx, {0}", SecondOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("xor eax, ebx");
            GeneratingAssembleCode.outFile.WriteLine("mov {0}, eax", result.AlternativeName);
        }
    }

    public partial class AssignmentInterNode
    {
        public override void GenerateAsmCode()
        {
            //соответствие типов проверено в семантике, тут всё хорошо должно быть
            if (result.Type=="int")
            {
                GeneratingAssembleCode.outFile.WriteLine("mov eax, {0}", FirstOperand.AlternativeName);
                GeneratingAssembleCode.outFile.WriteLine("mov {0}, eax", result.AlternativeName);
            }
            else if (result.Type=="bool")
            //то же самое, только однобайтовые
            {
                GeneratingAssembleCode.outFile.WriteLine("mov al, {0}", FirstOperand.AlternativeName);
                GeneratingAssembleCode.outFile.WriteLine("mov {0}, al", result.AlternativeName);
            }
        }
    }

    public partial class UnaryMinusInterNode
    {
        public override void GenerateAsmCode()
        {
            //унарный минус - вычитаем из нуля
            GeneratingAssembleCode.outFile.WriteLine("mov eax, 0");
            GeneratingAssembleCode.outFile.WriteLine("sub eax, {0}", FirstOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("mov {0}, eax", result.AlternativeName);
        }
    }

    //логическое отрицание (только для bool)
    public partial class NegativeInterNode
    {
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("mov al, {0}", FirstOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("not al");
            GeneratingAssembleCode.outFile.WriteLine("mov {0}, al", result.AlternativeName);
        }
    }

    public partial class IncrementInterNode
    {
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("inc {0}", FirstOperand.AlternativeName);
        }
    }

    public partial class DecrementInterNode
    {
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("dec {0}", FirstOperand.AlternativeName);
        }
    }

    //Сравнения и метки
    public partial class CmpNode
    {
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("mov eax, {0}", FirstOperand.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("cmp eax, {0}", SecondOperand.AlternativeName);
            //тут только сравниваем, переход - другой узел
        }
    }

    public partial class PutLabel
    {
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("{0}:", label);//тупо ставим метку
        }
    }

    public partial class GoToLabel
    {
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("{0} {1}", condition, label);//переход по метке и условию (безусловный переход - тоже условие)
        }

    }

    //Чтение и запись
    public partial class ReadVarInterNode
    {
        public override void GenerateAsmCode()
        {
            //получение дескрипторов (надеюсь, будет работать)
            GeneratingAssembleCode.outFile.WriteLine("invoke GetStdHandle, STD_INPUT_HANDLE");
            GeneratingAssembleCode.outFile.WriteLine("mov stdin, eax");
            //считывание числа как последовательности символов
            GeneratingAssembleCode.outFile.WriteLine("ReadConsole, stdin, ADDR buf, 20, ADDR cRead, NULL");
            //превращаем символ в число
            GeneratingAssembleCode.outFile.WriteLine("invoke crt_atoi, ADDR buf");
            if (variable.Type=="int")
            {
                //просто помещаем результат в переменнную
                GeneratingAssembleCode.outFile.WriteLine("mov {0}, eax", variable.AlternativeName);
            }
            else if (variable.Type=="bool")
            {
                //тут посложнее, если ввели 0, то false, если что-то другое - true
                GeneratingAssembleCode.outFile.WriteLine("cmp eax,0");
                string label = "comp_label_" + (++Counters.comparsions);
                GeneratingAssembleCode.outFile.WriteLine("jne {0}", label);
                GeneratingAssembleCode.outFile.WriteLine("mov {0}, 0", variable.AlternativeName);
                GeneratingAssembleCode.outFile.WriteLine("jmp exit_{0}",label);
                GeneratingAssembleCode.outFile.WriteLine("{0}:", label);
                GeneratingAssembleCode.outFile.WriteLine("mov {0}, 127", variable.AlternativeName);
                GeneratingAssembleCode.outFile.WriteLine("exit_{0}", label);
            }
        }
    }

    public partial class WriteVarInterNode
    {
        public override void GenerateAsmCode()
        {
            //получение дескрипторов (надеюсь, будет работать)
            GeneratingAssembleCode.outFile.WriteLine("invoke GetStdHandle, STD_OUTPUT_HANDLE");
            GeneratingAssembleCode.outFile.WriteLine("mov stdout, eax");
            if (variable.Type == "int")
            {
                //просто выводим на экран
                GeneratingAssembleCode.outFile.WriteLine("")
            }
            else if (variable.Type == "bool")
            {
               GeneratingAssembleCode.outFile.WriteLine(". IF")
            }
        }
    }

    public static class GeneratingAssembleCode
    {
        public static StreamWriter outFile = new StreamWriter("assemble_code.asm");

        //объявление переменной в сегменте данных (временные переменные будут тут же!)
        public static void PutVariable(Variable v)
        {
            //обращение по альтернативному имени, поскольку обычные имена могут повторяться (а у временных их нетю)
            outFile.Write(v.AlternativeName);
            outFile.Write(" ");
            if (v.Type == "int")
            {
                outFile.Write("dd");
                if (v.IsConst)
                    outFile.WriteLine(" {0} ", v.Value); //инициализируем константу
                else outFile.WriteLine("?");
            }
            else if (v.Type == "bool")
            {
                outFile.WriteLine("db"); //для логической одного байта хватит
                if (v.IsConst)
                    if (bool.Parse(v.Value))
                        outFile.WriteLine(" 127"); //true будет 127
                    else outFile.WriteLine(" 0");//false будет 0
                else outFile.WriteLine("?"); //если не константа, то не инициализируем
            }

            //тут графы и массивы, констант как таковых не будет
            else if (v.Type == "graph") //по сути это матрица
                //раз нормально скомпилилось, можно сразу размер указать
                outFile.WriteLine("dd {0} dup(-1)", (int.Parse(v.Value)* int.Parse(v.Value)));
            else if (v.Type=="array")
                //массивы вспомогательные, для функций с графами
                outFile.WriteLine("db {0} dup(0)", int.Parse(v.Value));
        }

        public static void Generate()
        {
            //начало файла
            outFile.WriteLine(".386");
            outFile.WriteLine(".model flat, stdcall");
            outFile.WriteLine("option casemap :none");
            outFile.WriteLine("include \\masm32\\include\\windows.inc");
            outFile.WriteLine("include \\masm32\\macros\\macros.asm");
            outFile.WriteLine("includelib \\masm32\\lib\\msvcrt.lib");
            outFile.WriteLine("uselib kernel32, user32, masm32, comctl32");
            //начало сегмента данных
            outFile.WriteLine(".data");
            //буфер для чтения
            outFile.WriteLine("buf db 128 dup(?)");
            //счётчик считанных символов для буфера
            outFile.WriteLine("cRead dd?");
            //для ввода-вывода
            outFile.WriteLine("stdin DWORD?");
            outFile.WriteLine("stdout DWORD?");
            //объявляем все переменные
            foreach (var v in IntermediateCodeList.AllVariables)
                PutVariable(v);
            //дальше код
            outFile.WriteLine(".code");
        }
    }
}
