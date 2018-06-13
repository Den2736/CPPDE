using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models.Warnings
{
    class UnusedVariableWarning: WarningMessage
    {
        public override string Message => $"Warning: variable {VariableName} was declared in line {LineNumber} but never used";
        public int LineNumber { get; }
        public string VariableName { get; }

        public UnusedVariableWarning(int lineNumber, string varName)
        {
            VariableName = varName;
            LineNumber = lineNumber;
        }
    }
}
