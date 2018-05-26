using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models.Exceptions.SemanticExceptions
{
    class RedeclaringVariableException: SemanticException
    {
        public override string Message => $"Redeclaring variable {VariableName} in line {LineNumber} ";
        public int LineNumber { get; }
        public string VariableName { get; }

        public RedeclaringVariableException(int lineNumber, string varName)
        {
            VariableName = varName;
            LineNumber = lineNumber;
        }
    }
}
