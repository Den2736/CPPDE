using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models
{
    public class Variable
    {
        //имя переменной
        public string Name { get; set; }
        //тип переменной
        public string Type { get; set; }
        //объявлена ли переменная
        public bool IsDeclared { get; set; }
        //альтернативное имя (т.к могут быть переменные с одинаковыми именами)
        public string AlternativeName { get; set; }
        //была ли переменная использована
        public bool WasUsed { get; set; }
        //было ли присвоено переменной значение
        public bool WasIdentified { get; set; }

        public override string ToString()
        {
            return $"{Type} {Name}";
        }
    }
}
