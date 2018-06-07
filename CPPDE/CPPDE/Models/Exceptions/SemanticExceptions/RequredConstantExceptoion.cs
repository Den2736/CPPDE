using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models.Exceptions.SemanticExceptions
{
    class RequredConstantExceptoion:SemanticException
    {
        public override string Message => $"Number of vertexes of graph must be constant in line {LineNumber} ";
        public int LineNumber { get; }

        public RequredConstantExceptoion(int lineNumber)
        {
            LineNumber = lineNumber;
        }
    }
}
