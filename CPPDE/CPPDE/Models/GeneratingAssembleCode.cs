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
            GeneratingAssembleCode.outFile.WriteLine("imul ebx");
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
            /*//получение дескрипторов (надеюсь, будет работать)
            GeneratingAssembleCode.outFile.WriteLine("invoke GetStdHandle, STD_INPUT_HANDLE");
            GeneratingAssembleCode.outFile.WriteLine("mov stdin, eax");
            //считывание числа как последовательности символов
            GeneratingAssembleCode.outFile.WriteLine("ReadConsole, stdin, ADDR buf, 20, ADDR cRead, NULL");
            //превращаем символ в число
            GeneratingAssembleCode.outFile.WriteLine("invoke crt_atoi, ADDR buf");*/
            if (variable.Type=="int")
            {
                //считали число
                GeneratingAssembleCode.outFile.WriteLine("invoke  crt_scanf, ADDR Format_in, ADDR {0}", variable.AlternativeName);
            }
            else if (variable.Type=="bool")
            {
                GeneratingAssembleCode.outFile.WriteLine("invoke  crt_scanf, ADDR Format_in, ADDR {0}", variable.AlternativeName);
                //тут посложнее, если ввели 0, то false, если что-то другое - true
                GeneratingAssembleCode.outFile.WriteLine("cmp {0},0", variable.AlternativeName);
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
            /*//доделаю потом если время останется
            //получение дескрипторов (надеюсь, будет работать)
            GeneratingAssembleCode.outFile.WriteLine("invoke GetStdHandle, STD_OUTPUT_HANDLE");
            GeneratingAssembleCode.outFile.WriteLine("mov stdout, eax");*/
            if (variable.Type == "int")
            {
                //просто выводим на экран
                GeneratingAssembleCode.outFile.WriteLine("invoke  crt_printf, ADDR Format_out, {0}", variable.AlternativeName);
            }
            else if (variable.Type == "bool")
            {
                //если ложь выводим 0, если истина выводим 1
                GeneratingAssembleCode.outFile.WriteLine("cmp {0},0", variable.AlternativeName);
                string label = "comp_label_" + (++Counters.comparsions);
                GeneratingAssembleCode.outFile.WriteLine("jne {0}", label);
                GeneratingAssembleCode.outFile.WriteLine("mov eax, 0", variable.AlternativeName);
                GeneratingAssembleCode.outFile.WriteLine("jmp exit_{0}", label);
                GeneratingAssembleCode.outFile.WriteLine("{0}:", label);
                GeneratingAssembleCode.outFile.WriteLine("mov eax, 1", variable.AlternativeName);
                GeneratingAssembleCode.outFile.WriteLine("exit_{0}", label);
                GeneratingAssembleCode.outFile.WriteLine("invoke  crt_printf, ADDR Format_out, eax");
            }
        }
    }

    public partial class CreateGraphInterNode
    {
        //при создании графа занулить все диагональные элементы
        public override void GenerateAsmCode()
        {
            //делаем цикл
            GeneratingAssembleCode.outFile.WriteLine("mov ecx, {0}", graph.Value);
            //Номер цикла (для метки)
            int number = ++Counters.cycles;
            //Загружаем начало графа
            GeneratingAssembleCode.outFile.WriteLine("lea esi, {0}", graph.AlternativeName);
            //ставим метку
            GeneratingAssembleCode.outFile.WriteLine("cycle_{0}:", number);
            //Зануляем ячейку
            GeneratingAssembleCode.outFile.WriteLine("mov [esi], 0");
            //Вычисляем следующую ячейку (по диагонали)
            GeneratingAssembleCode.outFile.WriteLine("add esi, {0}", (int.Parse(graph.Value)+1)*4); //каждая ячейка 4 байтиа
            //Идём на следующую итерацию
            GeneratingAssembleCode.outFile.WriteLine("loop cycle_{0}:", number);
        }
    }

    public partial class SetGraphCell
    {
        public override void GenerateAsmCode()
        {
            //получаем адрес графа
            GeneratingAssembleCode.outFile.WriteLine("lea esi, {0}", Edge.graph.AlternativeName);
            //получаем номер ячейки (Вершины нумеровать с 0)
            GeneratingAssembleCode.outFile.WriteLine("mov eax, {0}",Edge.i.AlternativeName);
            //тут все положительные, поэтому mul
            GeneratingAssembleCode.outFile.WriteLine("mul {0}", int.Parse(Edge.graph.Value));
            GeneratingAssembleCode.outFile.WriteLine("add eax, {0}", Edge.j.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("mul 4");
            GeneratingAssembleCode.outFile.WriteLine("add esi, eax");
            //Теперь в esi адрес требуемой ячейки
            GeneratingAssembleCode.outFile.WriteLine("mov eax, {0}", InputVar.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("mov [esi], eax");
        }
    }

    public partial class GetGraphCell
    {
        public override void GenerateAsmCode()
        {
            //получаем адрес графа
            GeneratingAssembleCode.outFile.WriteLine("lea esi, {0}", Edge.graph.AlternativeName);
            //получаем номер ячейки (Вершины нумеровать с 0)
            GeneratingAssembleCode.outFile.WriteLine("mov eax, {0}", Edge.i.AlternativeName);
            //тут все положительные, поэтому mul
            GeneratingAssembleCode.outFile.WriteLine("mul {0}", int.Parse(Edge.graph.Value));
            GeneratingAssembleCode.outFile.WriteLine("add eax, {0}", Edge.j.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("mul 4");
            GeneratingAssembleCode.outFile.WriteLine("add esi, eax");
            //Теперь в esi адрес требуемой ячейки
            GeneratingAssembleCode.outFile.WriteLine("mov eax, [esi]");
            GeneratingAssembleCode.outFile.WriteLine("mov {0}, eax", OutputVar.AlternativeName);
        }
    }

    public partial class CopyGraphsInterNode
    {
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("lea esi, {0}", inGraph);
            GeneratingAssembleCode.outFile.WriteLine("lea ebx, {0}", outGraph);
            //счётчик цикла
            GeneratingAssembleCode.outFile.WriteLine("mov ecx, {0}", int.Parse(inGraph.Value)*int.Parse(inGraph.Value));
            string label = "cycle_" + (++Counters.cycles).ToString();
            GeneratingAssembleCode.outFile.WriteLine("{0}:", label);
            GeneratingAssembleCode.outFile.WriteLine("mov eax, [esi]");
            GeneratingAssembleCode.outFile.WriteLine("mov [ebx], eax");
            //получаем следующие адреса
            GeneratingAssembleCode.outFile.WriteLine("add esi, 4");
            GeneratingAssembleCode.outFile.WriteLine("add ebx, 4");
            GeneratingAssembleCode.outFile.WriteLine("loop {0}", label);
        }
    }

    public partial class FloydCall
    {
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("invoke Floyd, ADDR {0}, {1}", graph.AlternativeName, graph.Value);
        }
    }

    public static class GeneratingAssembleCode
    {
        public static StreamWriter outFile = new StreamWriter("assemble_code.asm");

        //показывает, используется ли где-нибудь алгоритм Флойда (если нет, то код для него генерироваться не будет)
        public static bool WasFloydUsed = false;

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

        //Алгоритм Флойда
        public static void MakeFloydFunction()
        {
            outFile.WriteLine(";Алгоритм Флойда");
            //параметры - указатель на граф и размерность графа
            outFile.WriteLine("Floyd proc graph_pointer: DWORD, graph_dim: DWORD");
            //тут поместить в стек какие-то регистры

            //объяыляем локальные переменные - счётчики циклов
            outFile.WriteLine("LOCAL i:DWORD");
            outFile.WriteLine("LOCAL j:DWORD");
            outFile.WriteLine("LOCAL k:DWORD");
            //вспомогательная переменная
            outFile.WriteLine("LOCAL temp_var: DWORD");
            //загрузили адрес графа
            outFile.WriteLine("mov esi, graph_pointer");

            //дальше пошли итерации
            outFile.WriteLine("mov k,0");
            //внешний цикл пошёл
            outFile.WriteLine("Floyd_cycle_1:");
            //сравнить итератор внешего цикла
            outFile.WriteLine("cmp k, ebx");
            //если дошли, то на выход
            outFile.WriteLine("je Floyd_exit_cycle_1");
            //-------------Начало среднего цикла

            //Аналогично с внуренними циклами
            outFile.WriteLine("mov i,0");
            //средний цикл
            outFile.WriteLine("Floyd_cycle_2:");
            //сравнить итератор внешего цикла
            outFile.WriteLine("cmp i, ebx");
            //если дошли, то на выход
            outFile.WriteLine("je Floyd_exit_cycle_2");

            //----------Начало самого внутреннего цикла
            outFile.WriteLine("mov j,0");
            //средний цикл
            outFile.WriteLine("Floyd_cycle_3:");
            //сравнить итератор внешего цикла
            outFile.WriteLine("cmp j, ebx");
            //если дошли, то на выход
            outFile.WriteLine("je Floyd_exit_cycle_3");

            //Собственно обработка
            //Вычисляем [i,k] ячейку
            outFile.WriteLine("mov eax, i");
            outFile.WriteLine("mul graph_dim");
            outFile.WriteLine("add eax, k");
            outFile.WriteLine("mul 4"); //теперь в eax смещение
            outFile.WriteLine("cmp [esi+eax], -1");//если равно, то ребра нет
            outFile.WriteLine("je Floyd_next");
            //поместить во временную переменную
            outFile.WriteLine("mov eax, [esi+eax]");
            outFile.WriteLine("mov temp_var, eax");
            //Аналогично считаем ячейку [k,j]
            outFile.WriteLine("mov eax, k");
            outFile.WriteLine("mul graph_dim");
            outFile.WriteLine("add eax, j");
            outFile.WriteLine("mul 4"); //теперь в eax смещение
            outFile.WriteLine("cmp [esi+eax], -1");//если равно, то ребра нет
            outFile.WriteLine("je Floyd_next");
            //Добавляем в сумму
            outFile.WriteLine("mov eax, [esi+eax]");
            outFile.WriteLine("add temp_var, eax");// теперь в temp_var сумма [i,k] и [k,j] ребра
            //Сюда дойдём если всё хорошо
            //Теперь результирующая ячейка [i,j]
            outFile.WriteLine("mov eax, i");
            outFile.WriteLine("mul graph_dim");
            outFile.WriteLine("add eax, j");
            outFile.WriteLine("mul 4"); //теперь в eax смещение
            outFile.WriteLine("cmp [esi+eax], -1");//если равно, то ребра нет, его можно добавить
            outFile.WriteLine("mov ebx, temp_var");//сразу записали сумму в ebx
            outFile.WriteLine("jne Floyd_next_check");//если не равно идём дальше
            //Если равно, то ребро нужно добавить
            outFile.WriteLine("mov [esi+eax], ebx");
            //И идём дальше
            outFile.WriteLine("jmp Floyd_next");
            //тут будем проверять длину
            outFile.WriteLine("Floyd_next_check:");
            outFile.WriteLine("cmp [esi+eax], ebx");
            //если текущее расстояние не больше, то на следующую итераци
            outFile.WriteLine("jbe Floyd_next");
            //иначе записываем
            outFile.WriteLine("mov [esi+eax], ebx");

            //---------Конец внутреннего цикла
            //Переход на следующую итерацию
            outFile.WriteLine("Floyd_next:");
            //увеличить номер итерации на 1
            outFile.WriteLine("inc j");
            //Отправляемся на проверку условия
            outFile.WriteLine("jmp Floyd_Cycle_3");
            //выход из среднего цикла
            outFile.WriteLine("Floyd_exit_cycle_3:");

            //----------Конец среднего цикла
            //увеличить номер итерации на 1
            outFile.WriteLine("inc i");
            //Отправляемся на проверку условия
            outFile.WriteLine("jmp Floyd_Cycle_2");
            //выход из среднего цикла
            outFile.WriteLine("Floyd_exit_cycle_2:");

            //-------Конец внешнего цикла
            //увеличить номер итерации на 1
            outFile.WriteLine("inc k");
            //Отправляемся на проверку условия
            outFile.WriteLine("jmp Floyd_Cycle_1");
            //выход из внешнего цикла
            outFile.WriteLine("Floyd_exit_cycle_1:");

            outFile.WriteLine("ret");
            outFile.WriteLine("Floyd endp");

        }

        public static void Generate()
        {
            //начало файла
            outFile.WriteLine(".386");
            outFile.WriteLine(".model flat, stdcall");
            outFile.WriteLine("option casemap :none");
            outFile.WriteLine("include \\masm32\\include\\windows.inc");
            outFile.WriteLine("include \\masm32\\macros\\macros.asm");
            //для ввода-вывода
            outFile.WriteLine("includelib \\masm32\\lib\\msvcrt.lib");
            outFile.WriteLine("uselib kernel32, user32, masm32, comctl32");
            if (WasFloydUsed)
                outFile.WriteLine("Floyd PROTO :DWORD, :DWORD");
            //начало сегмента данных
            outFile.WriteLine(".data");
            //буфер для чтения
            outFile.WriteLine("buf db 128 dup(?)");
            //счётчик считанных символов для буфера
            outFile.WriteLine("cRead dd?");
            //для ввода-вывода
            outFile.WriteLine("stdin DWORD?");
            outFile.WriteLine("stdout DWORD?");
            //для форматированного ввода-вывода
            outFile.WriteLine("Format_in DB \"%d\",0");
            outFile.WriteLine("Format_out DB \"%d\", 0Dh,0Ah,0");
            //объявляем все переменные
            foreach (var v in IntermediateCodeList.AllVariables)
                PutVariable(v);
            //дальше код
            outFile.WriteLine(".code");
            //если Флойд используется, то генерим код для процедуры
            if (WasFloydUsed)
                MakeFloydFunction();
            outFile.WriteLine("start:");
            foreach (var node in IntermediateCodeList.IntermediateList)
                node.GenerateAsmCode();
            //конец файла
            outFile.WriteLine("invoke ExitProcess, 0");
            outFile.WriteLine("end start");
            outFile.Close();
        }
    }
}
