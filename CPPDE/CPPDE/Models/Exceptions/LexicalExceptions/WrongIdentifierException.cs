using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPPDE.Models.Exceptions.LexicalExceptions
{
    class WrongIdentifierException: LexicalException
    {
        public override string Message => $"Wrong identifier in line {LineNumber}:{Environment.NewLine} " +
            $"{Line}";
        public int LineNumber { get; }
        public string Line { get; }

        public WrongIdentifierException(int lineNumber, string line)
        {
            Line = line;
            LineNumber = lineNumber;
        }
    }
}
