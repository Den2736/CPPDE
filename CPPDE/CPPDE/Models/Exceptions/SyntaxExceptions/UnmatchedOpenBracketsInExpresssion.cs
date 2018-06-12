using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPPDE;

namespace C__DE.Models.Exceptions.SyntaxExceptions
{
    class UnmatchedOpenBracketsInExpresssion: SyntaxException
    {
        public override string Message => $"Unmatched bracket(s) \")\" in expression before token {Token} in line {LineNumber}";
        public int LineNumber { get; }
        public string Token { get;  }

        public UnmatchedOpenBracketsInExpresssion(int lineNumber, string token)
        {
            LineNumber = lineNumber;
            Token = token;
            Program.IsSyntaxCorrect = false;
        }
    }
}
