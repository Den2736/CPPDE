using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPPDE.Models.Exceptions.LexicalExceptions
{
    public class LexicalException: CompilerException
    {
        public override string Message => "Lexical error.";
    }
}
