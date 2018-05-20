using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models.Exceptions.SyntaxExceptions
{
    class UnexpectedEOFException: SyntaxException
    {
        public override string Message => "Unexpected end of file";
    }
}
