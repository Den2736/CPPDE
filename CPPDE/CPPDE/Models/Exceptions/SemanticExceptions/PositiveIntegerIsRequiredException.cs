using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models.Exceptions.SemanticExceptions
{
    class PositiveIntegerIsRequiredException: SemanticException
    {
        public override string Message => $"Number of vertexes of graph must be positive integer in line {LineNumber} ";
        public int LineNumber { get; }

        public PositiveIntegerIsRequiredException(int lineNumber)
        {
            LineNumber = lineNumber;
        }
    }
}
