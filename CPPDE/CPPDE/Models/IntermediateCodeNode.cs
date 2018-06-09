using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models
{
    /*узлы промежуточного кода*/

    /// <summary>
    /// Из типо только bool и int
    /// 
    /// Variable - не обязательно переменная, может быть и константа, тогда можно обратиться к свойству Value
    /// </summary>

    public abstract partial class IntermediateCodeNode
    {

    }

    //узел бинарного оператора
    public abstract class BinaryOperatorIntermediateNode : IntermediateCodeNode
    {
        //ссылки на переменные (ы том числе временные)
        public Variable FirstOperand;
        public Variable SecondOperand;
        public Variable result; //ссылки на узлы, из них можно извлекать типы, названия и прочее
        public BinaryOperatorIntermediateNode(Variable first, Variable second, Variable res)
        {
            FirstOperand = first;
            SecondOperand = second;
        }
    }

    /// <summary>
    /// Арифметические действия (Оба операнда и результат типа int), сюда же относится 
    /// </summary>

    //сложение
    public partial class AddInterNode : BinaryOperatorIntermediateNode
    {
        public AddInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }

    //вычитание
    public partial class SubInterNode : BinaryOperatorIntermediateNode
    {
        public SubInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }

    //умножение
    public partial class MulInterNode : BinaryOperatorIntermediateNode
    {
        public MulInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }

    //целая часть от деления
    public partial class DivInterNode : BinaryOperatorIntermediateNode
    {
        public DivInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }

    //остаток от деления
    public partial class ModInterNode : BinaryOperatorIntermediateNode
    {
        public ModInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }

    /// <summary>
    /// Логические бинарные операторы
    /// </summary>

    public partial class AndInterNode : BinaryOperatorIntermediateNode
    {
        public AndInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }

    public partial class OrInterNode : BinaryOperatorIntermediateNode
    {
        public OrInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }

    /// <summary>
    /// Побитовые бинарные операции
    /// </summary>

    public partial class BitAndInterNode : BinaryOperatorIntermediateNode
    {
        public BitAndInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }

    public partial class BitOrInterNode : BinaryOperatorIntermediateNode
    {
        public BitOrInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }

    public partial class BitXorInterNode : BinaryOperatorIntermediateNode
    {
        public BitXorInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }



    //узел унарного оператора
    public abstract class UnaryOperatorIntermediateNode : IntermediateCodeNode
    {
        //ссылки на переменные (в том числе временные)
        public Variable FirstOperand;
        public Variable result;
        public UnaryOperatorIntermediateNode(Variable first, Variable res)
        {
            FirstOperand = first;
            result = res;
        }
    }

    //присваивание - присвоить переменной res значение переменной first
    public partial class AssignmentInterNode : UnaryOperatorIntermediateNode
    {
        public AssignmentInterNode(Variable first, Variable res) : base(first, res) { }
    }

    //унарный минус
    public partial class UnaryMinusInterNode : UnaryOperatorIntermediateNode
    {
        public UnaryMinusInterNode(Variable first, Variable res) : base(first, res) { }
    }

    //отрицание
    public partial class NegativeInterNode : UnaryOperatorIntermediateNode
    {
        public NegativeInterNode(Variable first, Variable res) : base(first, res) { }
    }

    //инкремент++
    public partial class IncrementInterNode : UnaryOperatorIntermediateNode
    {
        public IncrementInterNode(Variable res) : base(res, res) { }
    }

    //декремент--
    public partial class DecrementInterNode : UnaryOperatorIntermediateNode
    {
        public DecrementInterNode(Variable res) : base(res, res) { }
    }

    //узел сравнения двух операндов
    public partial class CmpNode : IntermediateCodeNode
    {
        public Variable FirstOperand;
        public Variable SecondOperand;
        public CmpNode(Variable first, Variable second)
        {
            FirstOperand = first;
            SecondOperand = second;
        }
    }

    //узел метки (то есть в это место будет осуществляться переход)
    public partial class PutLabel : IntermediateCodeNode
    {
        public string label;
        public PutLabel(string lab)
        {
            label = lab;
        }
    }

    //узел перехода по метке (отсюда будет переход)
    public partial class GoToLabel : IntermediateCodeNode
    {
        public string label;
        public string condition;// jmp, jre и так далее
        public GoToLabel(string lab, string cond)
        {
            label = lab;
            condition = cond;
        }
    }
    /* Это не надо
    //объявление переменной - хз как
    public class DeclarVar : IntermediateCodeNode
    {
        public Variable variable;
        public DeclarVar(Variable v)
        {
            variable = v;
        }
    }
    */

    //чтение переменной
    public partial class ReadVarInterNode : IntermediateCodeNode
    {
        public Variable variable;
        public ReadVarInterNode(Variable v)
        {
            variable = v;
        }
    }

    //вывод на экран значения переменной
    public partial class WriteVarInterNode : IntermediateCodeNode
    {
        public Variable variable;
        public WriteVarInterNode(Variable v)
        {
            variable = v;
        }
    }

    /// <summary>
    /// Дальше всё, что связано с графами
    /// </summary>

    //в начале сегмента можно хранить число вершин, потом число рёбер, потом саму марицу
    //Можно не хранить число вершин - оно статическое и его можно взять сразу при анализе
    //Можно не хранить число рёбер, а считать его по ходу дела


    //ребро графа (она же ячейка в матрице, в виде которой хранится граф)
    //ТУТ ГЕНЕРАЦИЯ НЕ НУЖНА, просто вспомогательная структура данных

    public class GraphCell
    {
        //строка
        public Variable i;
        //столбец
        public Variable j;
        //граф (да, это тоже переменная на этапе анализа)
        public Variable graph;

        public GraphCell(Variable gr, Variable f, Variable sec)
        {
            graph = gr;
            i = f;
            j = sec;
        }
    }

    //ячейка массива, генерация не нужна
    public class ArrayCell
    {
        //индекс
        public Variable i;
        //массив
        public Variable array;

        public ArrayCell(Variable arr, Variable ind)
        {
            i = ind;
            array = arr;
        }
    }

    public class CreateGraphInterNode: IntermediateCodeNode
    {
        //по сути тут занулить диагональные элементы (все остальные равны минус 1)
        public Variable graph;
        public CreateGraphInterNode(Variable gr)
        {
            graph = gr;
        }
    }

    //записать в ячейку графа информацию из некоторой переменной
    public class SetGraphCell : IntermediateCodeNode
    {
        public Variable InputVar;
        public GraphCell Edge;
        public SetGraphCell(Variable Input, GraphCell Ed)
        {
            InputVar = Input;
            Edge = Ed;
        }

    }

    //Аналогично, только информация записывается из ячейки в переменную
    public class GetGraphCell : IntermediateCodeNode
    {
        public Variable OutputVar;
        public GraphCell Edge;
        public GetGraphCell(Variable Output, GraphCell Ed)
        {
            OutputVar = Output;
            Edge = Ed;
        }

    }

    //копирование графов (можно постараться разбить на более мелкие узды)
    public class CopyGraphsInterNode: IntermediateCodeNode
    {
        public Variable outGraph;
        public Variable inGraph;
        public CopyGraphsInterNode(Variable Out, Variable In)
        {
            outGraph = Out;
            inGraph = In;
        }
    }

}
