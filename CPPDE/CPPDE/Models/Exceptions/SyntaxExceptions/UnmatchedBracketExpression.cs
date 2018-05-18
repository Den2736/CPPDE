using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models.Exceptions.SyntaxExceptions
{
    class UnmatchedBracketExpression: SyntaxException
    {
        public override string Message => $"Unmatched bracket {Bracket} in line {LineNumber}";
        public int LineNumber { get; }
        public string Bracket { get; }

        public UnmatchedBracketExpression(int lineNumber, string bracket)
        {
            Bracket = bracket;
            LineNumber = lineNumber;
        }
    }
}
