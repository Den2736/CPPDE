using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models.Exceptions.SemanticExceptions
{
    class InvalidTypeException: SemanticException
    {
        public override string Message => $"Invalid type \" {TypeName} \" for operation \"{Operator}\" in line {LineNumber}";
        public int LineNumber { get; }
        public string TypeName { get; }
        public string Operator { get; }

        public InvalidTypeException(int lineNumber, string type, string operation)
        {
            LineNumber = lineNumber;
            TypeName = type;
            Operator=operation;
        }
    }
}
