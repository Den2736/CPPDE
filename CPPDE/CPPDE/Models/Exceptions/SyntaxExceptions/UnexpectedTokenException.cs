using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPPDE;

namespace C__DE.Models.Exceptions.SyntaxExceptions
{
    class UnexpectedTokenException:SyntaxException
    {
        public override string Message => $"Unexpected token {Token} in line {LineNumber}";
        public int LineNumber { get; }
        public string Token { get; }

        public UnexpectedTokenException(int lineNumber, string token)
        {
            Token = token;
            LineNumber = lineNumber;
            Program.IsSyntaxCorrect = false;
        }
    }
}
