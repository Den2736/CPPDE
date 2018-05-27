using CPPDE.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models.Warnings
{
    class WarningMessage: CompilerException
    {
            public override string Message => "Warning message.";
    }
}
