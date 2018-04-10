using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models
{
    public class Variable
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsDeclared { get; set; }
        public string AlternativeName { get; set; }
        public bool IsUsed { get; set; }

        public override string ToString()
        {
            return $"{Type} {Name}";
        }
    }
}
