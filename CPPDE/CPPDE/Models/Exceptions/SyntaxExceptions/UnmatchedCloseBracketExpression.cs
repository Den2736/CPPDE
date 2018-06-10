using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPPDE;

namespace C__DE.Models.Exceptions.SyntaxExceptions
{
    class UnmatchedCloseBracketExpression: SyntaxException
    {
        public override string Message => $"Unmatched bracket \")\" in line {LineNumber}";
        public int LineNumber { get; }

        public UnmatchedCloseBracketExpression(int lineNumber, string bracket)
        {
            LineNumber = lineNumber;
            Program.IsSyntaxCorrect = false;
        }
    }
}
