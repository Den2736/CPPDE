using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models.Exceptions.SemanticExceptions
{
    class UnequalGraphsDimentionsException: SemanticException
    {
        public override string Message => $"Dimentions \"{First}\" and  \"{Second} \" graphs in line {LineNumber} must be equal";
        public int LineNumber { get; }
        public string First { get; }
        public string Second { get; }

        public UnequalGraphsDimentionsException(int lineNumber, string firstGraph, string secondGraph)
        {
            LineNumber = lineNumber;
            First = firstGraph;
            Second = secondGraph;
        }
    }
}
