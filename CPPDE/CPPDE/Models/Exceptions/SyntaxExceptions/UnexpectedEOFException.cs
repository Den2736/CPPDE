using CPPDE.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPPDE;

namespace C__DE.Models.Exceptions.SyntaxExceptions
{
    class UnexpectedEOFException:CompilerException
    {
        public override string Message => "Unexpected end of file";
        
        public UnexpectedEOFException()
        {
            Program.IsSyntaxCorrect = false;
        }
    }
}
