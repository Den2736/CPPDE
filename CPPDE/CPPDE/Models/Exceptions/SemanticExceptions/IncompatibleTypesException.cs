using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models.Exceptions.SemanticExceptions
{
    class IncompatibleTypesException: SemanticException
    {
        public override string Message => $"Uncompetible types in line {LineNumber}: {FirstType}, {SecondType} ";
        public int LineNumber { get; }
        public string FirstType { get; }
        public string SecondType { get;  }

        public IncompatibleTypesException(int lineNumber, string firstType, string secondType)
        {
            LineNumber = lineNumber;
            FirstType = firstType;
            SecondType = secondType;
        }
    }
}
