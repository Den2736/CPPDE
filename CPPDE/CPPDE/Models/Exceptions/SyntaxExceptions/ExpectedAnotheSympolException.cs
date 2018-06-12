using CPPDE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models.Exceptions.SyntaxExceptions
{
    class ExpectedAnotherSymbolException: SyntaxException
    {
        public override string Message => $"Symbol {Symbol} expected but {Token} found in line {LineNumber}";
        public int LineNumber { get; }
        public string Token { get; }
        public string Symbol { get; }

        public ExpectedAnotherSymbolException(string symbol, int lineNumber, string token)
        {
            Token = token;
            LineNumber = lineNumber;
            Symbol = symbol;
            Program.IsSyntaxCorrect = false;
        }
    }
}
