using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models.Warnings
{
    class UnusedValueWarning:WarningMessage
    {
        public override string Message => $"Variable {VariableName} was assigned in line {LineNumber} but this walue was never used";
        public int LineNumber { get; }
        public string VariableName { get; }

        public UnusedValueWarning(int lineNumber, string varName)
        {
            VariableName = varName;
            LineNumber = lineNumber;
        }
    }
}
