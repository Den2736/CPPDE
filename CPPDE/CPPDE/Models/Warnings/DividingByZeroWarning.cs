using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models.Warnings
{
    class DividingByZeroWarning:WarningMessage
    {
        public override string Message => $"Warning: dividing by zero in line {LineNumber}";
        public int LineNumber { get; }

        public DividingByZeroWarning(int lineNumber)
        {
            LineNumber = lineNumber;
        }
    }
}
