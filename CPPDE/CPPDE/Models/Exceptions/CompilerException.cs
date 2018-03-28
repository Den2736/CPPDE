using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPPDE.Models.Exceptions
{
    public class CompilerException: Exception
    {
        public override string Message => "Compilation error.";
    }
}
