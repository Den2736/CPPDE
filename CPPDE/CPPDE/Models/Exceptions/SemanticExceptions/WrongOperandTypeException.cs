using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models.Exceptions.SemanticExceptions
{
    class WrongOperandTypeException: SemanticException
    {
        public override string Message => $"Operand {numOperand} in function {func} must be {type_} in line {LineNumber} ";
        public int LineNumber { get; }
        public int numOperand { get; }
        public string func { get; }
        public string type_ { get; }

        public WrongOperandTypeException(int lineNumber, string f, int operand, string t)
        {
            LineNumber = lineNumber;
            func = f;
            numOperand = operand;
            type_ = t;
        }
    }
}
