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

    public abstract class IntermediateCodeNode
    {
        //реализовать для каждого типа узлов, 
        //public abstract void GenerateAssembleCode();
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
    public class AddInterNode: BinaryOperatorIntermediateNode
    {
        public AddInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }

    //вычитание
    public class SubInterNode: BinaryOperatorIntermediateNode
    {
        public SubInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }

    //умножение
    public class MulInterNode: BinaryOperatorIntermediateNode
    {
        public MulInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }

    //целая часть от деления
    public class DivInterNode : BinaryOperatorIntermediateNode
    {
        public DivInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }

    //остаток от деления
    public class ModInterNode : BinaryOperatorIntermediateNode
    {
        public ModInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }

    /// <summary>
    /// Логические бинарные операторы
    /// </summary>

    public class AndInterNode : BinaryOperatorIntermediateNode
    {
        public AndInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }

    public class OrInterNode : BinaryOperatorIntermediateNode
    {
        public OrInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }

    /// <summary>
    /// Побитовые бинарные операции
    /// </summary>
    
    public class BitAndInterNode: BinaryOperatorIntermediateNode
    {
        public BitAndInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }

    public class BitOrInterNode : BinaryOperatorIntermediateNode
    {
        public BitOrInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }

    public class BitXorInterNode: BinaryOperatorIntermediateNode
    {
        public BitXorInterNode(Variable first, Variable second, Variable res) : base(first, second, res) { }
    }



    //узел унарного оператора
    public class UnaryOperatorIntermediateNode : IntermediateCodeNode
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
    public class AssignmentInterNode: UnaryOperatorIntermediateNode
    {
        public AssignmentInterNode(Variable first, Variable res) : base(first, res) { }
    }

    //унарный минус
    public class UnaryMinusInterNode: UnaryOperatorIntermediateNode
    {
        public UnaryMinusInterNode(Variable first, Variable res) : base(first, res) { }
    }

    //отрицание
    public class NegativeInterNode: UnaryOperatorIntermediateNode
    {
        public NegativeInterNode(Variable first, Variable res) : base(first, res) { }
    }

    //инкремент++
    public class IncrementInterNode: UnaryOperatorIntermediateNode
    {
        public IncrementInterNode( Variable res) : base(res, res) { }
    }

    //декремент--
    public class DecrementInterNode : UnaryOperatorIntermediateNode
    {
        public DecrementInterNode(Variable res) : base(res, res) { }
    }

    //узел сравнения двух операндов
    public class CmpNode: IntermediateCodeNode
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
    public class PutLabel: IntermediateCodeNode
    {
        public string label;
        public PutLabel(string lab)
        {
            label = lab;
        }
    }

    //узел перехода по метке (отсюда будет переход)
    public class GoToLabel: IntermediateCodeNode
    {
        public string label;
        public string condition;// jmp, jre и так далее
        public GoToLabel(string lab, string cond)
        {
            label = lab;
            condition = cond;
        }
    }

    //объявление переменной - хз как
    public class DeclarVar: IntermediateCodeNode
    {
        public Variable variable;
        public DeclarVar (Variable v)
        {
            variable = v;
        }
    }

    //чтение переменной
    public class ReadVarInterNode: IntermediateCodeNode
    {
        public Variable variable;
        public ReadVarInterNode(Variable v)
        {
            variable = v;
        }
    }

    //вывод на экран значения переменной
    public class WriteVarInterNode : IntermediateCodeNode
    {
        public Variable variable;
        public WriteVarInterNode(Variable v)
        {
            variable = v;
        }
    }
}
