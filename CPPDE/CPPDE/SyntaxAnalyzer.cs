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

            //приоритеты различных операций
            public static Dictionary<string, int> OperationPriorities = new Dictionary<string, int>();

            //стек узлов для имитации рекурсии
            public static Stack<BlockNode> NodesStack = new Stack<BlockNode>();

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
                OperationPriorities.Add("_", 5);//Это будет унарный минус
                OperationPriorities.Add("*", 6);
                OperationPriorities.Add("/", 6);
                OperationPriorities.Add("%", 6);

                OperationPriorities.Add("&", 7);
                OperationPriorities.Add("|", 7);
                OperationPriorities.Add("^", 7);
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

            //получаем текущую лексему
            public static Lexeme GetLexeme()
            {
                return LexemsForSyntaxAnalysis[LexemesIterator];
            }

            //получить конкретную лексему
            public static void GetConcreteLexeme(string lex)
            {
                Lexeme cur = GetLexeme();
                if (cur.Value == lex)
                {
                    LexemesIterator++;
                }
                else
                    throw new ExpectedAnotherSymbolException(lex, cur.Line, cur.Value);
            }


            //проверяет, могут ли две лексемы идти друг за другом (только для выражений)
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

            //разбор выражения (арифметического, логического)
            public static AtomNode ParseExpression()
            {
                //стек для временного хранения узлов. Они будут потихоньку присоединяться к более высоким узлам
                Stack<AtomNode> ResultStack = new Stack<AtomNode>();

                //стек для хранения операция
                Stack<KeyValuePair<string,int>> OperationStack = new Stack<KeyValuePair<string, int>>();

                //предыдущая и текущая лексемы
                string LastLexeme="", CurrentLexeme;

                //счётчик скобочек
                int brackets = 0;
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
                                ResultStack.Push(NewConstant);
                            }
                            else
                            {
                                ConstantNode NewConstant = new ConstantNode("int", CurrentLexeme, GetLexeme().Line);
                                ResultStack.Push(NewConstant);
                            }
                        }

                        else if (CurrentLexeme[0]=='\'') //это символ
                        {
                            ConstantNode NewConstant = new ConstantNode("char", CurrentLexeme, GetLexeme().Line);
                            ResultStack.Push(NewConstant);
                        }

                        else if (CurrentLexeme[0]=='\"') //это строка
                        {
                            ConstantNode NewConstant = new ConstantNode("string", CurrentLexeme, GetLexeme().Line);
                            ResultStack.Push(NewConstant);
                        }

                        else if (CurrentLexeme=="true" || CurrentLexeme=="false")//логическая константа
                        {
                            ConstantNode NewConstant = new ConstantNode("bool", CurrentLexeme, GetLexeme().Line);
                            ResultStack.Push(NewConstant);
                        }

                        else if (char.IsLetter(CurrentLexeme[0])) //переменная
                        {
                            VariableNode NewVariable = new VariableNode(CurrentLexeme, GetLexeme().Line);
                            ResultStack.Push(NewVariable);
                        }

                        else if (CurrentLexeme=="(")
                        {
                            OperationStack.Push(new KeyValuePair<string, int>("(",GetLexeme().Line));
                            ++brackets;
                        }

                        else if (CurrentLexeme==")")
                        {
                            
                            while ((OperationStack.Peek().Key != "(" && brackets!=0) || //разгрести до открывающей скобки
                                    ((OperationStack.Count!=0) && brackets==0)) //или до конца
                                {
                                    if (OperationStack.Count == 0)//если стек пустой, то нет соответствующей скобки
                                        throw new UnmatchedBracketExpression(GetLexeme().Line, ")");
                                    //извлекаем верхнюю операцию
                                    var operation = OperationStack.Pop();

                                    //новый узел для операции
                                    BinaryOperatorNode NewNode;

                                    if (operation.Key=="!" || operation.Key=="_")//если унарная
                                    {
                                        var Operand = ResultStack.Pop();
                                        if (operation.Key == "_")
                                            NewNode = new BinaryOperatorNode("-", Operand, operation.Value);
                                        else
                                            NewNode = new BinaryOperatorNode("!", Operand, operation.Value);
                                    }
                                    else
                                    {
                                        var SecondOperand = ResultStack.Pop();
                                        var FirstOperand = ResultStack.Pop();
                                        NewNode = new BinaryOperatorNode(operation.Key, FirstOperand, operation.Value);
                                    }
                                    ResultStack.Push(NewNode);
                                }
                            if (OperationStack.Count != 0) //если там была скобочка, то уменьшаем счётчик и идём дальше
                            {
                                --brackets;
                                OperationStack.Pop();
                            }
                            else
                                return ResultStack.Pop(); //иначе всё
                        }

                        else if (OperationPriorities.Keys.Contains(CurrentLexeme)) //если операция
                        {
                            if (CurrentLexeme == "-" && (LastLexeme == "" || LastLexeme == "("))
                                OperationStack.Push(new KeyValuePair<string, int>("_", GetLexeme().Line));
                            else if (CurrentLexeme == "!" || OperationStack.Count == 0)
                                OperationStack.Push(new KeyValuePair<string, int>(CurrentLexeme, GetLexeme().Line));
                            else
                            {
                                {
                                    while (OperationStack.Count > 0 && OperationPriorities[OperationStack.Peek().Key] >= OperationPriorities[CurrentLexeme])
                                    {
                                        var operation = OperationStack.Pop();

                                        //новый узел для операции
                                        BinaryOperatorNode NewNode;

                                        if (operation.Key == "!" || operation.Key == "_")//если унарная
                                        {
                                            var Operand = ResultStack.Pop();
                                            if (operation.Key == "_")
                                                NewNode = new BinaryOperatorNode("-", Operand, operation.Value);
                                            else
                                                NewNode = new BinaryOperatorNode("!", Operand, operation.Value);
                                        }
                                        else
                                        {
                                            var SecondOperand = ResultStack.Pop();
                                            var FirstOperand = ResultStack.Pop();
                                            NewNode = new BinaryOperatorNode(operation.Key, FirstOperand, operation.Value);
                                        }
                                        ResultStack.Push(NewNode);
                                    }
                                    OperationStack.Push(new KeyValuePair<string, int> (CurrentLexeme, GetLexeme().Line));
                                }
                            }

                        }

                        else if (CurrentLexeme=="," || CurrentLexeme==";")//если выражение оконченор
                        {
                            //если всё хорошо
                            if (LastLexeme == ")" || char.IsLetterOrDigit(LastLexeme[0]) || LastLexeme[0] == '\'' || LastLexeme[0] == '\"')
                            {
                                while (OperationStack.Count > 0)
                                {
                                    //извлекаем верхнюю операцию
                                    var operation = OperationStack.Pop();

                                    //новый узел для операции
                                    BinaryOperatorNode NewNode;

                                    if (operation.Key == "!" || operation.Key == "_")//если унарная
                                    {
                                        var Operand = ResultStack.Pop();
                                        if (operation.Key == "_")
                                            NewNode = new BinaryOperatorNode("-", Operand, operation.Value);
                                        else
                                            NewNode = new BinaryOperatorNode("!", Operand, operation.Value);
                                    }
                                    else
                                    {
                                        var SecondOperand = ResultStack.Pop();
                                        var FirstOperand = ResultStack.Pop();
                                        NewNode = new BinaryOperatorNode(operation.Key, FirstOperand, operation.Value);
                                    }
                                    ResultStack.Push(NewNode);
                                }
                                return ResultStack.Pop();
                            }
                            else
                                throw new UnexpectedTokenException(GetLexeme().Line, CurrentLexeme);
                        }

                        else //если что-то другое, то ошибка
                            throw new UnexpectedTokenException(GetLexeme().Line, CurrentLexeme);

                        //переход к следующей лексеме
                        LexemesIterator++;
                        LastLexeme = CurrentLexeme;
                    }
                }
            }

            //разбор оператора присваивания
            public static void ParseAssignmentOperator()
            {
                Lexeme CurrentLexeme=GetLexeme();
                if ((Types.Contains(CurrentLexeme.Value)) || (ReservedWords.Contains(CurrentLexeme.Value)) || !char.IsLetter(CurrentLexeme.Value[0]))
                    throw new UnexpectedTokenException(CurrentLexeme.Line, CurrentLexeme.Value);
                VariableNode AssignedVariable = new VariableNode(CurrentLexeme.Value, CurrentLexeme.Line);
                LexemesIterator++;
                CurrentLexeme = GetLexeme();
                if (!Operations.AssignmentOperations.Contains(CurrentLexeme.Value))
                    throw new UnexpectedTokenException(CurrentLexeme.Line, CurrentLexeme.Value);
                LexemesIterator++;
                try
                {
                    AtomNode Expression = ParseExpression();
                    NodesStack.Peek().AddOperator(new AssignmentOperator(AssignedVariable,Expression,CurrentLexeme.Value, AssignedVariable.LineNumber));
                    return;
                }
                catch(SyntaxException)
                {
                    while (!(GetLexeme().Value == ";" || GetLexeme().Value == ","  || LexemesIterator < LexemsForSyntaxAnalysis.Count))
                        ++LexemesIterator;
                    //идём до точки с запятой и возвращаем null
                    return;
                }
            }

            //разбор оператора объявления переменной
            public static void ParseDeclaration()
            {
                string varType = GetLexeme().Value; //получили тип
                LexemesIterator++;
                Lexeme CurrentLexeme;
                while (true)
                {
                    CurrentLexeme = GetLexeme();
                    //должен быть идентификатор
                    if (!char.IsLetter(CurrentLexeme.Value[0]) || ReservedWords.Contains(CurrentLexeme.Value) || Types.Contains(CurrentLexeme.Value))
                        throw new UnexpectedTokenException(CurrentLexeme.Line, CurrentLexeme.Value);
                    else //если всё хорошо
                    {
                        VariableNode DecVar = new VariableNode(CurrentLexeme.Value, CurrentLexeme.Line);
                        NodesStack.Peek().AddOperator(new VariableDeclarationNode(DecVar, varType, CurrentLexeme.Line));
                        LexemesIterator++;
                        if (GetLexeme().Value == "=")
                        {
                            LexemesIterator--;
                            try
                            {
                                ParseAssignmentOperator();
                            }
                            catch (SyntaxException)
                            {
                                if (LexemesIterator == LexemsForSyntaxAnalysis.Count)
                                    throw new UnexpectedEOFException();
                            }
                        }
                        CurrentLexeme = GetLexeme();
                        if (CurrentLexeme.Value == ",")
                            GetConcreteLexeme(",");
                        else if (CurrentLexeme.Value == ";")
                        {
                            GetConcreteLexeme(";");
                            return;
                        }
                        else throw new UnexpectedTokenException(CurrentLexeme.Line, CurrentLexeme.Value);
                    }
                }
            }

            //разбор условного оператора
            public static void ParseConditionOperator()
            {
                int numLine = LexemesIterator;
                GetConcreteLexeme("if");
                GetConcreteLexeme("(");
                AtomNode ConditionExpression = ParseExpression();
                GetConcreteLexeme(")");
                Lexeme CurrentLexeme = GetLexeme();
                //создаём ветку if
                ConditionalBranchNode IfBranch = new ConditionalBranchNode(CurrentLexeme.Line);
                NodesStack.Push(IfBranch);

                if (CurrentLexeme.Value != "{")//то там один оператор
                    GetNextOperator();
                else
                {
                    while (CurrentLexeme.Value!="}" && LexemesIterator<LexemsForSyntaxAnalysis.Count)
                    {
                        GetNextOperator();
                        CurrentLexeme = GetLexeme();
                    }
                    GetConcreteLexeme("}");
                }
                CurrentLexeme = GetLexeme();
                if (CurrentLexeme.Value == "else")//то есть вторая ветка
                {
                    ConditionalBranchNode ElseBranch = new ConditionalBranchNode(CurrentLexeme.Line);
                    NodesStack.Push(ElseBranch); //ещё одна ветка
                    LexemesIterator++;
                    CurrentLexeme = GetLexeme();
                    if (CurrentLexeme.Value != "{")//то там один оператор
                        GetNextOperator();
                    else
                    {
                        while (CurrentLexeme.Value != "}" && LexemesIterator < LexemsForSyntaxAnalysis.Count)
                        {
                            GetNextOperator();
                            CurrentLexeme = GetLexeme();
                        }
                        GetConcreteLexeme("}");
                    }
                    NodesStack.Pop();
                    NodesStack.Pop();
                    NodesStack.Peek().AddOperator(new ConditionalOperatorNode(ConditionExpression, numLine, IfBranch, ElseBranch));
                    return;
                }
                //если ветки else нет, то создаём с одной веткой
                NodesStack.Peek().AddOperator(new ConditionalOperatorNode(ConditionExpression, numLine, IfBranch));
                NodesStack.Pop();
            }

            //разбор цикла с предусловием
            public static void ParsePredConditionCycleOPerator()
            {
                GetConcreteLexeme("while");
                GetConcreteLexeme("(");
                AtomNode CycleCondition = ParseExpression();
                GetConcreteLexeme(")");

                //создали узел
                CycleOperator NewCycle = new CycleOperator(true, CycleCondition, GetLexeme().Line);
                NodesStack.Push(NewCycle);

                Lexeme CurrentLexeme = GetLexeme();
                if (CurrentLexeme.Value != "{")//то там один оператор
                    GetNextOperator();
                else
                {
                    while (CurrentLexeme.Value != "}" && LexemesIterator < LexemsForSyntaxAnalysis.Count)
                        GetNextOperator();
                    GetConcreteLexeme("}");
                }
                NodesStack.Pop();
                NodesStack.Peek().AddOperator(NewCycle);
            }

            public static void ParsePostConditionCycle()
            {
                int numline = LexemesIterator;
                GetConcreteLexeme("do");
                GetConcreteLexeme("{");
                Lexeme CurrentLexeme = GetLexeme();

                ConditionalBranchNode Operators = new ConditionalBranchNode(CurrentLexeme.Line);
                NodesStack.Push(Operators);

                while (CurrentLexeme.Value!="}")
                {
                    GetNextOperator();
                    CurrentLexeme = GetLexeme();
                }
                GetConcreteLexeme("}");

                GetConcreteLexeme("while");
                GetConcreteLexeme("(");
                AtomNode Exp = ParseExpression();
                GetConcreteLexeme(")");

                NodesStack.Pop();
                CycleOperator NewCycle = new CycleOperator(false, Exp, numline);
                NewCycle.ChildrenOperators = Operators.ChildrenOperators;
                NodesStack.Peek().AddOperator(NewCycle);
            }

            public static void GetNextOperator()
            {
                Lexeme CurrentLexeme = GetLexeme();
                try
                {
                    if (Types.Contains(CurrentLexeme.Value))
                        ParseDeclaration();
                    else if (CurrentLexeme.Value == "if")
                        ParseConditionOperator();
                }
                catch (SyntaxException)
                {

                }
            }

            public static void Parse()
            {
                MainRootNode root = new MainRootNode(LexemsForSyntaxAnalysis[0].Line);
                LexemesIterator = 0;
                NodesStack.Push(root);
            }
        }
    }
}
