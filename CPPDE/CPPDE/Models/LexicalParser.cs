using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace C__DE.Models
{
    public static class LexicalParser
    {
        private static string Word { get; set; }
        private static string Operator { get; set; }
        private static readonly string[] ComplicatedOperators = { "+=", "-=", "++", "--", "&&", "||" };
        private static List<string> LineLexemes { get; set; }
        private static readonly Regex ReWord = new Regex(@"\w");
        private static bool NeedToStoreSym { get; set; }

        public static IEnumerable<string> GetLexemes(string line)
        {
            LineLexemes = new List<string>();

            foreach (var sym in line)
            {
                NeedToStoreSym = true;
                CheckWord(sym);
                CheckOperator(sym);

                if (NeedToStoreSym)
                {
                    StoreSym(sym);
                }
            }

            StoreWordIfNotNull();
            StoreOperatorIfNotNull();

            return LineLexemes;
        }

        private static void CheckWord(char sym)
        {
            if (IsPartOfAWord(sym))
            {
                StoreOperatorIfNotNull();
                Word += sym;

                NeedToStoreSym = false;
            }
        }
        private static void CheckOperator(char sym)
        {
            if (IsPartOfAnOperator(sym))
            {
                StoreWordIfNotNull();
                Operator += sym;

                NeedToStoreSym = false;
            }
        }

        private static bool IsPartOfAWord(char sym)
        {
            return ReWord.IsMatch(sym.ToString());
        }
        private static bool IsPartOfAnOperator(char sym)
        {
            return ComplicatedOperators.Any(o => o.StartsWith(sym.ToString()));
        }

        private static void StoreWordIfNotNull()
        {
            if (!string.IsNullOrEmpty(Word))
            {
                LineLexemes.Add(Word);
                Word = "";
            }
        }
        private static void StoreOperatorIfNotNull()
        {
            if (!string.IsNullOrEmpty(Operator))
            {
                LineLexemes.Add(Operator);
                Operator = "";
            }
        }
        private static void StoreSym(char sym)
        {
            StoreWordIfNotNull();
            StoreOperatorIfNotNull();

            if (!char.IsWhiteSpace(sym))
            {
                LineLexemes.Add(sym.ToString());
            }
        }
    }
}
