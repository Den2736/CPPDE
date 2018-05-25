using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models.Exceptions.SemanticExceptions
{
    class UnidentifiedVariableException: SemanticException
    {
        public override string Message => $"Unidentified variable {VariableName} is used in line {LineNumber} ";
        public int LineNumber { get; }
        public string VariableName { get; }

        public UnidentifiedVariableException(int lineNumber, string varName)
        {
            VariableName = varName;
            LineNumber = lineNumber;
        }
    }
}
