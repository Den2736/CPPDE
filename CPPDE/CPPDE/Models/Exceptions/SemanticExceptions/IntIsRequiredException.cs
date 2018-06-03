using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models.Exceptions.SemanticExceptions
{
    class IntIsRequiredException: SemanticException
    {
        public override string Message => $"All operand and operators mist be \"int\" type in creating graph in line {LineNumber} ";
        public int LineNumber { get; }

        public IntIsRequiredException(int lineNumber)
        {
            LineNumber = lineNumber;
        }
    }
}
