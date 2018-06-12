using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPPDE;

namespace C__DE.Models.Exceptions.SyntaxExceptions
{
    class UnexpectedEndOfExpressionException: SyntaxException
    {
        public override string Message => $"Unexpected end of expression in line {LineNumber} by token '{Token}'";
        public int LineNumber { get; }
        public string Token { get; }

        public UnexpectedEndOfExpressionException(int lineNumber, string token)
        {
            Token=token;
            LineNumber = lineNumber;
            Program.IsSyntaxCorrect = false;
        }
    }
}
