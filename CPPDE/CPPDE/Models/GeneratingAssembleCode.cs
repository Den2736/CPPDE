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
            if (FirstOperand.Type=="int")
            {
                GeneratingAssembleCode.outFile.WriteLine("mov eax, {0}", FirstOperand.AlternativeName);
                GeneratingAssembleCode.outFile.WriteLine("cmp eax, {0}", SecondOperand.AlternativeName);
            }
            else if (FirstOperand.Type=="bool")
            {
                GeneratingAssembleCode.outFile.WriteLine("mov al, {0}", FirstOperand.AlternativeName);
                GeneratingAssembleCode.outFile.WriteLine("cmp al, {0}", SecondOperand.AlternativeName);
            }
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
                GeneratingAssembleCode.outFile.WriteLine("exit_{0}:", label);
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
            GeneratingAssembleCode.outFile.WriteLine("mov ebx, 0");
            GeneratingAssembleCode.outFile.WriteLine("mov [esi], ebx");
            //Вычисляем следующую ячейку (по диагонали)
            GeneratingAssembleCode.outFile.WriteLine("add esi, {0}", (int.Parse(graph.Value)+1)*4); //каждая ячейка 4 байтиа
            //Идём на следующую итерацию
            GeneratingAssembleCode.outFile.WriteLine("loop cycle_{0}", number);
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
            GeneratingAssembleCode.outFile.WriteLine("mov ebx, {0}", int.Parse(Edge.graph.Value));
            GeneratingAssembleCode.outFile.WriteLine("mul ebx");
            GeneratingAssembleCode.outFile.WriteLine("add eax, {0}", Edge.j.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("mov ebx,4");
            GeneratingAssembleCode.outFile.WriteLine("mul ebx");
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
            GeneratingAssembleCode.outFile.WriteLine("mov ebx, {0}", int.Parse(Edge.graph.Value));
            GeneratingAssembleCode.outFile.WriteLine("mul ebx");
            GeneratingAssembleCode.outFile.WriteLine("add eax, {0}", Edge.j.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("mov ebx, 4");
            GeneratingAssembleCode.outFile.WriteLine("mul ebx");
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
            GeneratingAssembleCode.outFile.WriteLine("lea esi, {0}", inGraph.AlternativeName);
            GeneratingAssembleCode.outFile.WriteLine("lea ebx, {0}", outGraph.AlternativeName);
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

    public partial class NumComponentsCall
    {
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("invoke num_components, ADDR {0}, {1}, ADDR {2}, ADDR {3}", graph.AlternativeName, graph.Value, outVar.AlternativeName, array.AlternativeName);
        }
    }

    public partial class CountEdgesInterNode
    {
        public override void GenerateAsmCode()
        {
            GeneratingAssembleCode.outFile.WriteLine("invoke num_edges, ADDR {0}, {1}, ADDR {2}", graph.AlternativeName, graph.Value, outVar.AlternativeName);
        }
    }

    public static class GeneratingAssembleCode
    {
        public static StreamWriter outFile = new StreamWriter("assemble_code.asm");

        //показывает, используется ли где-нибудь алгоритм Флойда (если нет, то код для него генерироваться не будет)
        public static bool WasFloydUsed = false;

        //Аналогично - для DFS
        public static bool WasDFSUsed = false;

        //Подсчёт числа компонент
        public static bool NumComponentsUsed = false;

        //Нужен ли подсчёт рёбер
        public static bool WasCountEdgesUsed = false;

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
                else outFile.WriteLine(" ?");
            }
            else if (v.Type == "bool")
            {
                outFile.Write("db"); //для логической одного байта хватит
                if (v.IsConst)
                    if (bool.Parse(v.Value))
                        outFile.WriteLine(" 127"); //true будет 127
                    else outFile.WriteLine(" 0");//false будет 0
                else outFile.WriteLine(" ?"); //если не константа, то не инициализируем
            }

            //тут графы и массивы, констант как таковых не будет
            else if (v.Type == "graph") //по сути это матрица
                //раз нормально скомпилилось, можно сразу размер указать
                outFile.WriteLine("dd {0} dup(-1)", (int.Parse(v.Value)* int.Parse(v.Value)));
            else if (v.Type=="array")
                //массивы вспомогательные, булевские, для функций с графами
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
            
            //Загрузили число вершин
            outFile.WriteLine("mov ebx, graph_dim");
            //------------Начало внешнего цикла
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
            //загрузили адрес графа
            outFile.WriteLine("mov esi, graph_pointer");
            //Вычисляем [i,k] ячейку
            outFile.WriteLine("mov eax, i");
            outFile.WriteLine("mul graph_dim");
            outFile.WriteLine("add eax, k");
            outFile.WriteLine("mov ecx, 4");
            outFile.WriteLine("mul ecx"); //теперь в eax смещение для [i,k]

            outFile.WriteLine("mov eax, [esi+eax]");//в eax содержимое ячейки [i,k]
            outFile.WriteLine("cmp eax, -1");//если равно, то ребра нет
            outFile.WriteLine("je Floyd_next");
            //поместить во временную переменную
            outFile.WriteLine("mov temp_var, eax");
            //Аналогично считаем ячейку [k,j]
            outFile.WriteLine("mov eax, k");
            outFile.WriteLine("mul graph_dim");
            outFile.WriteLine("add eax, j");
            outFile.WriteLine("mov ecx, 4");
            outFile.WriteLine("mul ecx"); //теперь в eax смещение
            
            outFile.WriteLine("mov eax, [esi+eax]");//в eax содержимое ячейки
            outFile.WriteLine("cmp eax, -1");//если равно, то ребра нет
            outFile.WriteLine("je Floyd_next");
            //Добавляем в сумму
            outFile.WriteLine("add temp_var, eax");// теперь в temp_var сумма [i,k] и [k,j] ребра
            //Сюда дойдём если всё хорошо
            //Теперь результирующая ячейка [i,j]
            outFile.WriteLine("mov eax, i");
            outFile.WriteLine("mul graph_dim");
            outFile.WriteLine("add eax, j");
            outFile.WriteLine("mov ecx, 4");
            outFile.WriteLine("mul ecx"); //теперь в eax смещение [i,j]
            outFile.WriteLine("mov ecx, temp_var");//сразу записали сумму в ecx
            outFile.WriteLine("add esi, eax");//в esi адрес ячейки [i,j]
            outFile.WriteLine("mov eax, [esi]");//в eax содержимое ячейки [i,j]
            outFile.WriteLine("cmp eax, -1");//если равно, то ребра нет, его можно добавить

            outFile.WriteLine("jne Floyd_next_check");//если не равно идём дальше
            //Если равно, то ребро нужно добавить
            
            outFile.WriteLine("mov [esi], ecx");
            //И идём дальше
            outFile.WriteLine("jmp Floyd_next");
            //тут будем проверять длину
            outFile.WriteLine("Floyd_next_check:");
            outFile.WriteLine("cmp [esi], ecx");
            //если текущее расстояние не больше, то на следующую итерацию
            outFile.WriteLine("jbe Floyd_next");
            //иначе записываем
            outFile.WriteLine("mov [esi], ecx");

            //---------Конец внутреннего цикла
            //Переход на следующую итерацию
            outFile.WriteLine("Floyd_next:");
            //увеличить номер итерации на 1
            outFile.WriteLine("inc j");
            //Отправляемся на проверку условия
            outFile.WriteLine("jmp Floyd_cycle_3");
            //выход из среднего цикла
            outFile.WriteLine("Floyd_exit_cycle_3:");

            //----------Конец среднего цикла
            //увеличить номер итерации на 1
            outFile.WriteLine("inc i");
            //Отправляемся на проверку условия
            outFile.WriteLine("jmp Floyd_cycle_2");
            //выход из среднего цикла
            outFile.WriteLine("Floyd_exit_cycle_2:");

            //-------Конец внешнего цикла
            //увеличить номер итерации на 1
            outFile.WriteLine("inc k");
            //Отправляемся на проверку условия
            outFile.WriteLine("jmp Floyd_cycle_1");
            //выход из внешнего цикла
            outFile.WriteLine("Floyd_exit_cycle_1:");

            outFile.WriteLine("ret");
            outFile.WriteLine("Floyd endp");

        }

        //поиск в глубину
        public static void MakeDFSFunction()
        {
            outFile.WriteLine("DFS proc graph_pointer: DWORD, graph_dim: DWORD, used_pointer: DWORD, curr_vertex: DWORD");
            //функция рекурсивная, поэтому сохраняем значения регистров
            outFile.WriteLine("push esi");
            outFile.WriteLine("push ebx");
            outFile.WriteLine("push ecx");
            //помечаем текущую вершину просмотренной
            outFile.WriteLine("mov esi, used_pointer");
            outFile.WriteLine("mov eax, curr_vertex");
            outFile.WriteLine("mov bl, 127");
            outFile.WriteLine("mov[esi + eax], bl");
            outFile.WriteLine(" mov ecx, graph_dim");
            //в ebx будет номер текущей просматриваемой вершины
            outFile.WriteLine("mov ebx,0");
            outFile.WriteLine("DFS_cycle:");
            outFile.WriteLine("mov esi, used_pointer");
            outFile.WriteLine("mov al, [esi + ebx]");
            //если вершина просмотрена, то идём на следующую итерацию
            outFile.WriteLine("cmp al,0");
            outFile.WriteLine("jne next_dfs");
            //смотрим, смежна ливершина с данной
            outFile.WriteLine("mov esi, graph_pointer");
            outFile.WriteLine("mov eax, curr_vertex");
            outFile.WriteLine("mul graph_dim");
            outFile.WriteLine("add eax, ebx");
            outFile.WriteLine("mov edx,4");
            outFile.WriteLine("mul edx");
            //проверяем наличие ребра
            outFile.WriteLine("mov eax, [esi + eax]");
            outFile.WriteLine("cmp eax, -1");
            outFile.WriteLine("je next_dfs");
            //если смежна и не просмотрена - рекурсивный вызов
            outFile.WriteLine("push ecx");
            //после инвоука почему-то портится ecx
            outFile.WriteLine("invoke DFS, graph_pointer, graph_dim, used_pointer, ebx");
            outFile.WriteLine("pop ecx");
            outFile.WriteLine("next_dfs:");
            outFile.WriteLine("inc ebx");
            outFile.WriteLine("loop DFS_cycle");
            //восстанавливаем значения регистров и возвращаем управление
            outFile.WriteLine("pop ecx");
            outFile.WriteLine("pop ebx");
            outFile.WriteLine("pop esi");
            outFile.WriteLine("ret");
            outFile.WriteLine("DFS endp");
        }

        //Число компонент
        public static void MakeNumComponentsFunction()
        {
            outFile.WriteLine("num_components proc graph_pointer: DWORD, graph_dim: DWORD, components: DWORD, used_pointer: DWORD");
            //обнулить компоненты
            outFile.WriteLine(" mov esi, components");
            outFile.WriteLine("mov eax, 0");
            outFile.WriteLine("mov[esi], eax");
            //номер текущей вершины
            outFile.WriteLine("mov ebx, 0");
            //в esi - указатель на будевский массив
            outFile.WriteLine("mov esi, used_pointer");
            //счётсик цикла
            outFile.WriteLine("mov ecx, graph_dim");
            outFile.WriteLine("num_components_cycle:");
            //Посещена ли вершина
            outFile.WriteLine("mov al, [esi]");
            outFile.WriteLine("cmp al,0");
            //Если нет - Вызываем DFS
            outFile.WriteLine("jne next_component");
            outFile.WriteLine("push esi");
            outFile.WriteLine("mov esi, components");
            outFile.WriteLine("mov eax, [esi]");
            outFile.WriteLine("inc eax");
            outFile.WriteLine("mov[esi], eax");
            outFile.WriteLine("pop esi");
            outFile.WriteLine("push ecx");
            outFile.WriteLine("invoke DFS, graph_pointer, graph_dim, used_pointer, ebx");
            outFile.WriteLine("pop ecx");
            //Если посещена - идём дальше
            outFile.WriteLine("next_component:");
            outFile.WriteLine("inc ebx");
            outFile.WriteLine("inc esi");
            outFile.WriteLine("loop num_components_cycle");
            outFile.WriteLine("ret");
            outFile.WriteLine("num_components endp");
        }

        //число рёбер
        public static void MakeCountEdges()
        {
            outFile.WriteLine("num_edges proc graph_pointer: DWORD, graph_dim: DWORD, num_edges_pointer: DWORD");
            //Зануляем число рёбер
            outFile.WriteLine("mov esi, num_edges_pointer");
            outFile.WriteLine("mov dword ptr[esi], 0");
            outFile.WriteLine("mov eax, graph_dim");
            outFile.WriteLine("mul eax");
            outFile.WriteLine("mov ecx, eax");
            outFile.WriteLine("mov esi, graph_pointer");
            //Просматривем все ячейки матрицы
            outFile.WriteLine("count_edges_cycle:");
            outFile.WriteLine("mov eax, [esi]");
            //Считаем всё, что не -1
            outFile.WriteLine("cmp eax, -1");
            outFile.WriteLine("je next_edge");
            outFile.WriteLine("push esi");
            outFile.WriteLine("mov esi, num_edges_pointer");
            outFile.WriteLine("inc dword ptr[esi]");
            outFile.WriteLine("pop esi");
            outFile.WriteLine("next_edge:");
            outFile.WriteLine("add esi, 4");
            outFile.WriteLine("loop count_edges_cycle");
            outFile.WriteLine("mov esi, num_edges_pointer");
            outFile.WriteLine("mov eax, [esi]");
            //Удаляем диагональные ячейки (они нули и к рёбрам не относятся)
            outFile.WriteLine("sub eax, graph_dim");
            //Делим пополам, т.к матрица симметричная
            outFile.WriteLine("mov ebx, 2");
            outFile.WriteLine("div ebx");
            outFile.WriteLine("mov[esi], eax");
            outFile.WriteLine("ret");
            outFile.WriteLine("num_edges endp");
        }

        public static void Generate()
        {
            //начало файла
            outFile.WriteLine(".386");
            outFile.WriteLine(".model flat, stdcall");
            outFile.WriteLine("option casemap :none");
            outFile.WriteLine("include \\masm32\\include\\windows.inc");
            outFile.WriteLine("include \\masm32\\include\\masm32.inc");
            outFile.WriteLine("include \\masm32\\include\\msvcrt.inc");
            outFile.WriteLine("include \\masm32\\macros\\macros.asm");
            outFile.WriteLine("includelib \\masm32\\lib\\masm32.lib");
            //для ввода-вывода
            outFile.WriteLine("includelib \\masm32\\lib\\msvcrt.lib");
            outFile.WriteLine("uselib kernel32, user32, masm32, comctl32");
            if (WasFloydUsed)
                outFile.WriteLine("Floyd PROTO :DWORD, :DWORD");
            if (WasDFSUsed)
                outFile.WriteLine("DFS PROTO :DWORD, :DWORD, :DWORD, :DWORD");
            if (NumComponentsUsed)
                outFile.WriteLine("num_components PROTO :DWORD, :DWORD, :DWORD, :DWORD");
            if (WasCountEdgesUsed)
                outFile.WriteLine("num_edges PROTO:DWORD, :DWORD, :DWORD");
            //начало сегмента данных
            outFile.WriteLine(".data");
            //буфер для чтения
            outFile.WriteLine("buf db 128 dup(?)");
            //счётчик считанных символов для буфера
            outFile.WriteLine("cRead dd ?");
            //для ввода-вывода
            outFile.WriteLine("stdin DWORD ?");
            outFile.WriteLine("stdout DWORD ?");
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
            if (WasDFSUsed)
                MakeDFSFunction();
            if (NumComponentsUsed)
                MakeNumComponentsFunction();
            if (WasCountEdgesUsed)
                MakeCountEdges();
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
