using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPPDE.Models.Exceptions.LexicalExceptions;
using C__DE.Models;
using static CPPDE.Program;
using C__DE.Models.Exceptions.SyntaxExceptions;

namespace CPPDE
{
    partial class Program
    {
        //приоритеты различных операций
        public static Dictionary<string, int> OperationPriorities = new Dictionary<string, int>();

        //Одна лексема
        public class Lexeme
        {
            public int Line;
            public string Value;
        }

       


        //----------------------------------------------------------------
        public static class SyntaxAnalyzer
        {
            //Список всех лексем по порядку
            public static List<Lexeme> LexemsForSyntaxAnalysis = new List<Lexeme>();

            //итератор по списку лексем
            public static int LexemesIterator = 0;

            //список служебных слов
            public static List<string> ReservedWords = new List<string> { "if", "while", "do", "else" };

            //Назначаем приоритет операциям
            public static void GetOperationsPriorities()
            {
                Dictionary<string, int> OperationPriorities = new Dictionary<string, int>();
                OperationPriorities.Add("(", 0);
                OperationPriorities.Add(")", 0);

                OperationPriorities.Add("||", 1);
                OperationPriorities.Add("&&", 2);
                OperationPriorities.Add("!", 3);

                OperationPriorities.Add(">", 4);
                OperationPriorities.Add("<", 4);
                OperationPriorities.Add("==", 4);
                OperationPriorities.Add(">=", 4);
                OperationPriorities.Add("<=", 4);
                OperationPriorities.Add("!=", 4);

                OperationPriorities.Add("+", 5);
                OperationPriorities.Add("-", 5);
                OperationPriorities.Add("*", 6);
                OperationPriorities.Add("/", 6);
                OperationPriorities.Add("%", 6);
            }

            //сформировать нормальный список лексем
            public static void DoLexemsList()
            {
                foreach (int line in Lexemes.Keys)
                    foreach (var lexem in Lexemes[line])
                    {
                        var elem = new Lexeme();
                        elem.Line = line;
                        elem.Value = lexem;
                        LexemsForSyntaxAnalysis.Add(elem);
                    }
            }

            //получаем лексему
            public static Lexeme GetLexeme()
            {
                return LexemsForSyntaxAnalysis[LexemesIterator];
            }

            public static bool CanBeTogether(string s1, string s2)
            {
                if ((s1 == "" || s1 == "(") && (s2 == "-"))
                    return true;
                if ((s1 == "" || s1 == "(" || OperationPriorities.Keys.Contains(s1) && s1 != ")") && 
                    (char.IsLetterOrDigit(s2[0]) || s2=="!" || s2==")" || s2[0]=='\'' || s2[0]=='\"'))
                    return false;
                if ((char.IsLetterOrDigit(s1[0]) || s1 == ")" || s1[0] == '\'' || s1[0] == '\"') && 
                    (OperationPriorities.Keys.Contains(s2) && s2 != "!"))
                    return true;
                return false;
            }

            public static AtomNode ParseExpression()
            {
                //стек для временного хранения узлов. Они будут потихоньку присоединяться к более высоким узлам
                Stack<AtomNode> OperationsStack = new Stack<AtomNode>();
                string LastLexeme="", CurrentLexeme;
                AtomNode mainNode;
                while (true)
                {
                    CurrentLexeme = GetLexeme().Value;
                    if (ReservedWords.Contains(CurrentLexeme) || !(CanBeTogether(LastLexeme, CurrentLexeme)))
                        throw new UnexpectedTokenException(GetLexeme().Line, GetLexeme().Value);


                    else //если всё хорошо
                    {
                        if (char.IsDigit(CurrentLexeme[0])) //То это числовая константа
                        {
                            if (CurrentLexeme.Contains("."))
                            {
                                ConstantNode NewConstant = new ConstantNode("float", CurrentLexeme, GetLexeme().Line);
                                OperationsStack.Push(NewConstant);
                            }
                            else
                            {
                                ConstantNode NewConstant = new ConstantNode("int", CurrentLexeme, GetLexeme().Line);
                                OperationsStack.Push(NewConstant);
                            }
                        }

                        else if (CurrentLexeme[0]=='\'') //это символ
                        {
                            ConstantNode NewConstant = new ConstantNode("char", CurrentLexeme, GetLexeme().Line);
                            OperationsStack.Push(NewConstant);
                        }

                        else if (CurrentLexeme[0]=='\"') //это строка
                        {
                            ConstantNode NewConstant = new ConstantNode("string", CurrentLexeme, GetLexeme().Line);
                            OperationsStack.Push(NewConstant);
                        }

                        else if (CurrentLexeme=="true" || CurrentLexeme=="false")//логическая константа
                        {
                            ConstantNode NewConstant = new ConstantNode("bool", CurrentLexeme, GetLexeme().Line);
                            OperationsStack.Push(NewConstant);
                        }

                        else if (char.IsLetter(CurrentLexeme[0])) //переменная
                        {
                            VariableNode NewVariable = new VariableNode(CurrentLexeme, GetLexeme().Line);
                            OperationsStack.Push(NewVariable);
                        }
                    }
                }
            }

            public static void Parse()
            {
                throw new NotImplementedException();
            }
        }
    }
}
