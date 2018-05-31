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
using CPPDE.Models.Exceptions;

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
            public static List<string> ReservedWords = new List<string> { "if", "while", "do", "else" ,"scan", "print"};

            //приоритеты различных операций
            public static Dictionary<string, int> OperationPriorities = new Dictionary<string, int>();

            //стек узлов для имитации рекурсии
            public static Stack<BlockNode> NodesStack = new Stack<BlockNode>();

            //Назначаем приоритет операциям
            public static void GetOperationsPriorities()
            {
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
                if (LexemesIterator >= LexemsForSyntaxAnalysis.Count)
                    throw new UnexpectedEOFException();
                return LexemsForSyntaxAnalysis[LexemesIterator];
            }

            //получить конкретную лексему
            public static void GetConcreteLexeme(string lex)
            {
                if (LexemesIterator < LexemsForSyntaxAnalysis.Count)
                {
                    Lexeme cur = GetLexeme();
                    if (cur.Value == lex)
                    {
                        LexemesIterator++;
                    }
                    else
                        throw new ExpectedAnotherSymbolException(lex, cur.Line, cur.Value);
                }
                else throw new ExpectedAnotherSymbolException(lex, LexemsForSyntaxAnalysis.Last().Line, "end of file");
            }

            //обработка исключения "Неожиданный конец файла" - все операторы "складываются"
            public static void HandleEOFException()
            {
                while (NodesStack.Count!=1)
                {
                    BlockNode EmbeddedNode = NodesStack.Pop();
                    NodesStack.Peek().AddOperator(EmbeddedNode);
                }
            }

            //проверяет, могут ли две лексемы идти друг за другом (только для выражений)
            public static bool CanBeTogether(string s1, string s2)
            {
                if ((s1 == "" || s1 == "(") && (s2 == "-"))
                    return true;
                if ((s1 == "" || s1 == "(" || OperationPriorities.Keys.Contains(s1) && s1 != ")") && 
                    (char.IsLetterOrDigit(s2[0]) || s2=="!"  || s2[0]=='\'' || s2[0]=='\"' || s2=="("))
                    return true;
                if (s1.Length>0 && ((char.IsLetterOrDigit(s1[0]) || s1 == ")" || s1[0] == '\'' || s1[0] == '\"') && 
                    (OperationPriorities.Keys.Contains(s2) && s2 != "!")))
                    return true;
                return false;
            }

            //идём до следующего элемента (для выражения или присваивания)
            public static void GoNext(int brackets)
            {
                //если лексемы закончились
                while (true)
                {
                    if (LexemesIterator >= LexemsForSyntaxAnalysis.Count)
                        throw new UnexpectedEOFException();
                    if (GetLexeme().Value == ";" || GetLexeme().Value == ",")
                        return;
                    if (GetLexeme().Value == ")")
                        if (brackets == 0)
                            return;
                        else brackets--;
                    if (GetLexeme().Value == "(")
                        brackets++;
                    else if (ReservedWords.Contains(GetLexeme().Value) || Types.Contains(GetLexeme().Value))//может просто пропустили точку с запятой, и дальше идёт новый оператор
                        return;
                    if (GetLexeme().Value == "{" || GetLexeme().Value == "}")
                        return;
                    LexemesIterator++; //если ничего из этого нет, то идём дальше
                }
            }

            //разбор выражения (арифметического, логического)
            public static AtomNode ParseExpression()
            {
                //стек для временного хранения узлов. Они будут потихоньку присоединяться к более высоким узлам
                Stack<AtomNode> ResultStack = new Stack<AtomNode>();

                //стек для хранения операция
                Stack<KeyValuePair<string,int>> OperationStack = new Stack<KeyValuePair<string, int>>();

                //предыдущая и текущая лексемы
                Lexeme LastLexeme=new Lexeme(), CurrentLexeme=GetLexeme();
                LastLexeme.Value="";
                LastLexeme.Line = 0;

                //счётчик скобочек
                int brackets = 0;
                while (true)
                {
                    if (LexemesIterator >= LexemsForSyntaxAnalysis.Count)
                        throw new UnexpectedEOFException();
                    CurrentLexeme = GetLexeme();

                    if (CurrentLexeme.Value == "," || CurrentLexeme.Value == ";" || (brackets==0 && CurrentLexeme.Value==")"))//если выражение окончено
                    {
                        if (brackets>0) //плохо со скобками
                        {
                            try
                            {
                                throw new UnmatchedOpenBracketsInExpresssion(CurrentLexeme.Line, CurrentLexeme.Value);
                            }
                            catch(SyntaxException e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                        //дальше проверяем последнюю лексему выражения
                        if (LastLexeme.Value == ")" || char.IsLetterOrDigit(LastLexeme.Value[0]) || LastLexeme.Value[0] == '\'' || LastLexeme.Value[0] == '\"')
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
                                    NewNode = new BinaryOperatorNode(operation.Key, FirstOperand, SecondOperand, operation.Value);
                                }
                                ResultStack.Push(NewNode);
                            }
                            return ResultStack.Pop();
                        }
                        //если последний оператор не такой, ,бросаем исключение
                        else
                        {
                            throw new UnexpectedEndOfExpressionException(CurrentLexeme.Line, CurrentLexeme.Value);
                        }
                    }

                    //тут возможно забыли поставить точку с запятой
                    if (ReservedWords.Contains(CurrentLexeme.Value) || Types.Contains(CurrentLexeme.Value) || CurrentLexeme.Value=="{" || CurrentLexeme.Value =="}")
                    {
                        try
                        {
                            throw new ExpectedAnotherSymbolException(";", CurrentLexeme.Line, CurrentLexeme.Value);
                            //в этом случае отловим исключение сразу, и потом в программе начнёт парситься новый оператор
                        }
                        catch (SyntaxException e)
                        {
                            Console.WriteLine(e.Message);
                            //аналогично пробуем собрать то, что есть
                            //если всё хорошо
                            if (LastLexeme.Value == ")" || char.IsLetterOrDigit(LastLexeme.Value[0]) || LastLexeme.Value[0] == '\'' || LastLexeme.Value[0] == '\"')
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
                                        ResultStack.Push(NewNode);
                                    }
                                    else if (operation.Key=="(")
                                    {
                                        OperationStack.Pop();
                                    }
                                    else
                                    {
                                        var SecondOperand = ResultStack.Pop();
                                        var FirstOperand = ResultStack.Pop();
                                        NewNode = new BinaryOperatorNode(operation.Key, FirstOperand, SecondOperand, operation.Value);
                                        ResultStack.Push(NewNode);
                                    }

                                }
                                return ResultStack.Pop();
                            }
                            //если последний оператор не такой, ,бросаем исключение
                            else
                            {
                                throw new UnexpectedEndOfExpressionException(CurrentLexeme.Line, CurrentLexeme.Value);
                            }
                        }
                    }

                    //Дальше проверяем, может ли два символа идти вместе (например, два идентифкатора не могут идти подряд)
                    else if (!CanBeTogether(LastLexeme.Value, CurrentLexeme.Value))
                    {
                        //проверяем, может ли вторая лексема быть началом нового выражения
                        if (char.IsLetterOrDigit(CurrentLexeme.Value[0]) || CurrentLexeme.Value == "!" || CurrentLexeme.Value[0] == '\'' || CurrentLexeme.Value[0] == '\"' || CurrentLexeme.Value == "(")
                            //Если да, то собираем всё до первой лексемы в кучу
                        {
                            if (brackets > 0) //плохо со скобками
                            {
                                try
                                {
                                    throw new UnmatchedOpenBracketsInExpresssion(CurrentLexeme.Line, CurrentLexeme.Value);
                                }
                                catch (SyntaxException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                            //дальше проверяем последнюю лексему выражения
                            if (LastLexeme.Value == ")" || char.IsLetterOrDigit(LastLexeme.Value[0]) || LastLexeme.Value[0] == '\'' || LastLexeme.Value[0] == '\"')
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
                                        ResultStack.Push(NewNode);
                                    }
                                    else if (operation.Key == "(")
                                    {
                                        OperationStack.Pop();
                                    }
                                    else
                                    {
                                        var SecondOperand = ResultStack.Pop();
                                        var FirstOperand = ResultStack.Pop();
                                        NewNode = new BinaryOperatorNode(operation.Key, FirstOperand, SecondOperand, operation.Value);
                                        ResultStack.Push(NewNode);
                                    }
                                }
                                return ResultStack.Pop();
                            }
                            //если что-то плохо:
                            //идём до конца оператора
                            GoNext(0);
                            //и бросаем исключение
                            throw new UnexpectedTokenException(CurrentLexeme.Line, CurrentLexeme.Value);
                        }
                    }

                    else //если всё хорошо, идём дальше по выражению
                    {
                        if (char.IsDigit(CurrentLexeme.Value[0])) //То это числовая константа
                        {
                            if (CurrentLexeme.Value.Contains("."))
                            {
                                ConstantNode NewConstant = new ConstantNode("float", CurrentLexeme.Value, CurrentLexeme.Line);
                                ResultStack.Push(NewConstant);
                            }
                            else
                            {
                                ConstantNode NewConstant = new ConstantNode("int", CurrentLexeme.Value, CurrentLexeme.Line);
                                ResultStack.Push(NewConstant);
                            }
                        }

                        else if (CurrentLexeme.Value[0]=='\'') //это символ
                        {
                            ConstantNode NewConstant = new ConstantNode("char", CurrentLexeme.Value, CurrentLexeme.Line);
                            ResultStack.Push(NewConstant);
                        }

                        else if (CurrentLexeme.Value[0]=='\"') //это строка
                        {
                            ConstantNode NewConstant = new ConstantNode("string", CurrentLexeme.Value, CurrentLexeme.Line);
                            ResultStack.Push(NewConstant);
                        }

                        else if (CurrentLexeme.Value == "true" || CurrentLexeme.Value == "false")//логическая константа
                        {
                            ConstantNode NewConstant = new ConstantNode("bool", CurrentLexeme.Value, CurrentLexeme.Line);
                            ResultStack.Push(NewConstant);
                        }

                        else if (char.IsLetter(CurrentLexeme.Value[0])) //переменная
                        {
                            VariableNode NewVariable = new VariableNode(CurrentLexeme.Value, CurrentLexeme.Line);
                            ResultStack.Push(NewVariable);
                        }

                        else if (CurrentLexeme.Value == "(")
                        {
                            OperationStack.Push(new KeyValuePair<string, int>("(", CurrentLexeme.Line));
                            ++brackets;
                        }

                        else if (CurrentLexeme.Value == ")")
                        {
                            
                            while ((brackets == 0 && (OperationStack.Count != 0)) || ( (brackets != 0) && (OperationStack.Peek().Key != "(" ))) //разгрести до открывающей скобки или до конца
                                {
                                    if (OperationStack.Count == 0)//если стек пустой, то нет соответствующей скобки
                                    {
                                        return ResultStack.Pop(); //может быть например в условном операторе, поэтому скобка может быть от него
                                    }
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
                                        NewNode = new BinaryOperatorNode(operation.Key, FirstOperand,SecondOperand, operation.Value);
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

                        else if (OperationPriorities.Keys.Contains(CurrentLexeme.Value)) //если операция
                        {
                            if (CurrentLexeme.Value == "-" && (LastLexeme.Value == "" || LastLexeme.Value == "("))
                                OperationStack.Push(new KeyValuePair<string, int>("_", GetLexeme().Line));
                            else if (CurrentLexeme.Value == "!" || OperationStack.Count == 0)
                                OperationStack.Push(new KeyValuePair<string, int>(CurrentLexeme.Value, GetLexeme().Line));
                            else
                            {
                                {
                                    while (OperationStack.Count > 0 && OperationPriorities[OperationStack.Peek().Key] >= OperationPriorities[CurrentLexeme.Value])
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
                                            NewNode = new BinaryOperatorNode(operation.Key, FirstOperand, SecondOperand, operation.Value);
                                        }
                                        ResultStack.Push(NewNode);
                                    }
                                    OperationStack.Push(new KeyValuePair<string, int> (CurrentLexeme.Value, GetLexeme().Line));
                                }
                            }

                        }

                        else //если что-то другое, то ошибка (по идее ничего другого быть не должно, но на всякий случай)
                        {
                            GoNext(brackets);
                            throw new UnexpectedTokenException(CurrentLexeme.Line, CurrentLexeme.Value);
                        }

                        //переход к следующей лексеме
                        LexemesIterator++;
                        LastLexeme = CurrentLexeme;
                    }
                }
            }
            //по идее после этого указатель будет стоять на разделителе или начале следующего оператора

            //разбор оператора присваивания
            public static AssignmentOperator ParseAssignmentOperator()
            {
                Lexeme CurrentLexeme=GetLexeme(); //тут по идее тоже проблем быть не должно, потому что тогда это будет начало другого оператора
                if ((Types.Contains(CurrentLexeme.Value)) || (ReservedWords.Contains(CurrentLexeme.Value)) || !char.IsLetter(CurrentLexeme.Value[0]))
                {
                    throw new UnexpectedTokenException(CurrentLexeme.Line, CurrentLexeme.Value);
                }
                VariableNode AssignedVariable = new VariableNode(CurrentLexeme.Value, CurrentLexeme.Line);
                LexemesIterator++;
                CurrentLexeme = GetLexeme();
                if (!Operations.AssignmentOperations.Contains(CurrentLexeme.Value))
                {
                    //вообще тут исключения быть не должно, поскольку функция вызывается только когда известно, что стоит нужный символ
                    throw new UnexpectedTokenException(CurrentLexeme.Line, CurrentLexeme.Value);
                }
                LexemesIterator++;
                try
                {
                    AtomNode Expression = ParseExpression();
                    //если всё хорошо, возвращаем новый узел
                    return new AssignmentOperator(AssignedVariable, Expression, CurrentLexeme.Value, AssignedVariable.LineNumber);
                }
                catch(SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                    return null; //если что-то не так с выражением
                    //Указатель всё равно будет стоять на разделителе или начале следующего оператора, если пользователь не поставил разделитель
                }
            }

            //разбор оператора объявления переменной
            public static void ParseDeclaration()
            {
                //тут исключение не кидаем, поскольку вызов осуществляется ТОЛЬКО если была обнаружена метка типа
                string varType = GetLexeme().Value; //получили тип
                LexemesIterator++;
                if (LexemesIterator >= LexemsForSyntaxAnalysis.Count)
                    throw new UnexpectedEOFException();
                Lexeme CurrentLexeme;
                while (true)
                {
                    CurrentLexeme = GetLexeme();
                    //должен быть идентификатор
                    if (!char.IsLetter(CurrentLexeme.Value[0]) || ReservedWords.Contains(CurrentLexeme.Value) || Types.Contains(CurrentLexeme.Value))
                    {
                        int Line = CurrentLexeme.Line;
                        string Value = CurrentLexeme.Value;
                        //что делать, если пользователь написал фигню?
                        while (CurrentLexeme.Value!=";" && CurrentLexeme.Value!=",") //может, тут ещё фигурные скобки добавить?
                        {
                            LexemesIterator++;
                            GoNext(0);
                            CurrentLexeme = GetLexeme();
                        }
                        try
                        {
                            throw new UnexpectedTokenException(Line, Value);//плохо, идём до точки с запятой и кидаем исключение
                        }
                        catch(SyntaxException e)
                        {
                            Console.WriteLine(e.Message);
                            //если дошли до конца оператора
                            if (CurrentLexeme.Value == ";")
                                return;//то на выход
                            else
                                GetConcreteLexeme(","); //если конец файла, он отловится позже, в главном парсере
                        }
                    }
                    else //если всё хорошо
                    {
                        VariableNode DecVar = new VariableNode(CurrentLexeme.Value, CurrentLexeme.Line);
                        NodesStack.Peek().AddOperator(new VariableDeclarationNode(DecVar, varType, CurrentLexeme.Line));
                        LexemesIterator++;
                        if (Operations.AssignmentOperations.Contains(GetLexeme().Value))
                        {
                            LexemesIterator--;
                            try
                            {
                                AssignmentOperator oper = ParseAssignmentOperator();
                                if (oper!=null)
                                    NodesStack.Peek().AddOperator(oper);
                            //После этого указатель будет стоять на разделителе или начале следующего оператора, даже если null
                            }
                            //сюда доходить не должен, но вдруг там совсем дичь
                            catch (SyntaxException e)
                            {
                                Console.WriteLine(e.Message);
                                if (LexemesIterator == LexemsForSyntaxAnalysis.Count)
                                    throw new UnexpectedEOFException();
                                CurrentLexeme = GetLexeme();
                                int Line = CurrentLexeme.Line;
                                string Value = CurrentLexeme.Value;
                                while (CurrentLexeme.Value != ";" || CurrentLexeme.Value != ",")
                                {
                                    GoNext(0);
                                    CurrentLexeme = GetLexeme();
                                }
                            }
                        }
                        CurrentLexeme = GetLexeme();

                        //тут исключений не будет, потому что оно 100% равно, другого символа тут не будет
                        if (CurrentLexeme.Value == ",") 
                            GetConcreteLexeme(",");
                        else if (CurrentLexeme.Value == ";")
                        {
                            GetConcreteLexeme(";");
                            return;
                        }
                        else
                        {
                            try//бросим исключение и тут же выловим
                            {
                                throw new ExpectedAnotherSymbolException("';' or ','", CurrentLexeme.Line, CurrentLexeme.Value);
                            }
                            catch (SyntaxException e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            if (ReservedWords.Contains(CurrentLexeme.Value) || Types.Contains(CurrentLexeme.Value))
                                //тут тоже на случай, если забыли разделитель
                                return; //пошли на следующий оператор
                            if (char.IsDigit(CurrentLexeme.Value[0]) || CurrentLexeme.Value[0] == '\'' || CurrentLexeme.Value[0] == '\"')
                                return; //если константа, то это начало выражения (возможно)
                            //иначе просто идём на следующую итерацию
                        }
                    }
                }
            }

            //взять один оператор
            public static List<AtomNode> GetSingleOperator()//начальное действие (только для цикла for)
            {
                List<AtomNode> OperationList = new List<AtomNode>();
                Lexeme CurrentLexeme = GetLexeme();
                if (CurrentLexeme.Value == ";")//начального действия нет
                    return OperationList;

                else if (Types.Contains(CurrentLexeme.Value)) //если есть объявление типа
                {
                    string VarType = CurrentLexeme.Value;
                    LexemesIterator++;
                    CurrentLexeme = GetLexeme();
                    //должен быть идентификатор
                    if (ReservedWords.Contains(CurrentLexeme.Value)) //кто додумается написать оператор внутри условий цикла?
                    {
                        try
                        {
                            throw new UnexpectedTokenException(CurrentLexeme.Line, CurrentLexeme.Value);
                        }
                        catch (SyntaxException e)
                        {
                            Console.WriteLine(e.Message);
                            return OperationList;
                        }
                    }


                    if (!char.IsLetter(CurrentLexeme.Value[0])) //константа или выражение
                    {
                        int Line = CurrentLexeme.Line;
                        string Value = CurrentLexeme.Value;
                        try
                        {
                            //результат выражения не нужен ибо тут ничего не присваивается, ещё и объявление стоит
                            ParseExpression();
                            //даже если парсинг выражения неудачный, то остановимся на разделителе
                            return OperationList;
                        }
                        catch (SyntaxException e)
                        {
                            Console.WriteLine(e.Message);
                            return OperationList; //что получилось, то и возвращаем
                        }

                    }
                    else //если всё хорошо
                    {
                        VariableNode DecVar = new VariableNode(CurrentLexeme.Value, CurrentLexeme.Line);
                        OperationList.Add(new VariableDeclarationNode(DecVar, VarType, CurrentLexeme.Line));
                    }

                }

                //указатель стоит на первой переменной(или константе)
                LexemesIterator++;
                if (Operations.AssignmentOperations.Contains(GetLexeme().Value))
                {
                    LexemesIterator--;//вернули на переменную
                    AssignmentOperator AssignOper = ParseAssignmentOperator();
                    if (AssignOper!=null)
                        OperationList.Add(AssignOper);
                }
                else
                {
                    LexemesIterator--;
                    try
                    {
                        AtomNode Expression = ParseExpression();
                        OperationList.Add(Expression);
                    }
                    catch (SyntaxException e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }
                return OperationList;
            }

            //разбор условного оператора
            public static void ParseConditionOperator()
            {
                AtomNode ConditionExpression;
                int numLine = LexemesIterator;
                try
                {
                    GetConcreteLexeme("if");
                    GetConcreteLexeme("(");
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                }
                Lexeme CurrentLexeme = GetLexeme();
                if (ReservedWords.Contains(CurrentLexeme.Value) && CurrentLexeme.Value!="else")
                    return; //начинаем разбор нового оператора
                try
                { 
                    ConditionExpression = ParseExpression();
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                    ConditionExpression = new ConstantNode("bool", "true",GetLexeme().Line);
                }

                try
                {
                    GetConcreteLexeme(")");
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                }
                //создаём ветку if
                CurrentLexeme = GetLexeme();
                ConditionalBranchNode IfBranch = new ConditionalBranchNode(CurrentLexeme.Line);
                NodesStack.Push(IfBranch);

                if (CurrentLexeme.Value != "{")//то там один оператор
                    GetNextOperator();
                else if (CurrentLexeme.Value!="else")
                {
                    GetConcreteLexeme("{");//тут исключения не будет
                    CurrentLexeme = GetLexeme();
                    while (CurrentLexeme.Value!="}" && LexemesIterator<LexemsForSyntaxAnalysis.Count)
                    {
                        GetNextOperator(); //тут все исключения и так должны отловиться
                        CurrentLexeme = GetLexeme();
                    }
                    GetConcreteLexeme("}");//тут из исключений только EOF, он отловится позже
                }
                //если сразу идём сюда, то if-ветка - пустая
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
                try
                {
                    GetConcreteLexeme("(");
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                }
                AtomNode CycleCondition;
                try
                {
                    CycleCondition = ParseExpression();
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                    CycleCondition = new ConstantNode("bool", "false", GetLexeme().Line);
                }

                try
                {
                    GetConcreteLexeme(")");
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                }
                //создали узел
                CycleOperator NewCycle = new CycleOperator(true, CycleCondition, GetLexeme().Line);
                NodesStack.Push(NewCycle);

                Lexeme CurrentLexeme = GetLexeme();
                if (CurrentLexeme.Value != "{")//то там один оператор
                    GetNextOperator();
                else
                {
                    GetConcreteLexeme("{");
                    CurrentLexeme = GetLexeme();
                    while (CurrentLexeme.Value != "}")
                    {
                        GetNextOperator();
                        CurrentLexeme = GetLexeme();
                    }
                    GetConcreteLexeme("}");
                }
                NodesStack.Pop();
                NodesStack.Peek().AddOperator(NewCycle);
            }

            //разбор цикла с постусловием
            public static void ParsePostConditionCycle()
            {
                int numline = LexemesIterator;
                GetConcreteLexeme("do");
                try
                {
                    GetConcreteLexeme("{");

                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                }
                Lexeme CurrentLexeme = GetLexeme();

                ConditionalBranchNode Operators = new ConditionalBranchNode(CurrentLexeme.Line);
                NodesStack.Push(Operators);

                while (CurrentLexeme.Value!="}")
                {
                    GetNextOperator();
                    CurrentLexeme = GetLexeme();
                }

                GetConcreteLexeme("}");

                try
                {
                    GetConcreteLexeme("while");
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                    NodesStack.Pop();
                    CycleOperator Cycle = new CycleOperator(false, new ConstantNode("bool","false",GetLexeme().Line) , numline);
                    Cycle.ChildrenOperators = Operators.ChildrenOperators;
                    foreach (var child in Cycle.ChildrenOperators)
                        child.SetParentBlock(Cycle);
                    NodesStack.Peek().AddOperator(Cycle);
                    return;
                }
                GetConcreteLexeme("(");
                AtomNode Exp = ParseExpression();
                GetConcreteLexeme(")");

                NodesStack.Pop();
                CycleOperator NewCycle = new CycleOperator(false, Exp, numline);
                NewCycle.ChildrenOperators = Operators.ChildrenOperators;
                foreach (var child in NewCycle.ChildrenOperators)
                    child.SetParentBlock(NewCycle);
                NodesStack.Peek().AddOperator(NewCycle);
            }

            //разбор цикла for
            public static void ParseForCycle()
            {
                GetConcreteLexeme("for");
                try
                {
                    GetConcreteLexeme("(");
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                }
                List<AtomNode> BeginAction = GetSingleOperator();//тут ничего вываливаться не должно
                try
                {
                    GetConcreteLexeme(";");
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                }

                AtomNode Expression;
                try
                {
                    Expression = ParseExpression();
                }
                catch(SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                    Expression = new ConstantNode("bool", "true", GetLexeme().Line);
                }
                try
                {
                    GetConcreteLexeme(";");
                }
                catch(SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                }


                AssignmentOperator PostAction;
                try

                {
                    PostAction=ParseAssignmentOperator();
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                    PostAction = null;
                }

                try
                {
                    GetConcreteLexeme(")");
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                }

                CycleOperator NewCycle = new CycleOperator(BeginAction, Expression, PostAction, GetLexeme().Line);
                NodesStack.Push(NewCycle);

                Lexeme CurrentLexeme = GetLexeme();
                if (CurrentLexeme.Value != "{")//то там один оператор
                    GetNextOperator();
                else
                {
                    GetConcreteLexeme("{");
                    while (CurrentLexeme.Value != "}" && LexemesIterator < LexemsForSyntaxAnalysis.Count)
                    {
                        GetNextOperator();
                        CurrentLexeme = GetLexeme();
                    }
                    GetConcreteLexeme("}");
                }
                NodesStack.Pop();
                NodesStack.Peek().AddOperator(NewCycle);
            }

            //разбор оператора чтения
            public static void ParseReadOperator()
            {
                GetConcreteLexeme("scan");
                try
                {
                    GetConcreteLexeme("(");

                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                }
                Lexeme CurrentLexeme = GetLexeme();
                if (ReservedWords.Contains(CurrentLexeme.Value) || !char.IsLetter(CurrentLexeme.Value[0]) || Types.Contains(CurrentLexeme.Value))
                    throw new UnexpectedTokenException(CurrentLexeme.Line, CurrentLexeme.Value);
                VariableNode ReadVar = new VariableNode(CurrentLexeme.Value, CurrentLexeme.Line);
                NodesStack.Peek().AddOperator(new ReadOperator(ReadVar, CurrentLexeme.Line));

                LexemesIterator++;
                CurrentLexeme = GetLexeme();
                while (CurrentLexeme.Value != ")")
                {
                    try
                    {
                        GetConcreteLexeme(",");
                    }
                    catch(SyntaxException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    CurrentLexeme = GetLexeme();
                    if (ReservedWords.Contains(CurrentLexeme.Value) || !char.IsLetter(CurrentLexeme.Value[0]) || Types.Contains(CurrentLexeme.Value))
                        throw new UnexpectedTokenException(CurrentLexeme.Line, CurrentLexeme.Value);
                    ReadVar = new VariableNode(CurrentLexeme.Value, CurrentLexeme.Line);
                    NodesStack.Peek().AddOperator(new ReadOperator(ReadVar, CurrentLexeme.Line));
                    LexemesIterator++;
                    CurrentLexeme = GetLexeme();
                }
                try
                {
                    GetConcreteLexeme(")");
                }
                catch(SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                }
                try
                {
                    GetConcreteLexeme(";");
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            //разбор оператора записи
            public static void ParseWriteOperator()
            {
                GetConcreteLexeme("print");
                try
                {
                    GetConcreteLexeme("(");
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                }
                Lexeme CurrentLexeme = GetLexeme();

                AtomNode Exp;
                try
                {
                    Exp= ParseExpression();
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                    Exp = new ConstantNode("string", "", GetLexeme().Line);
                }
                NodesStack.Peek().AddOperator(new WriteOperator(Exp, CurrentLexeme.Line));
                CurrentLexeme = GetLexeme();
                while (CurrentLexeme.Value!=")")
                {
                    try
                    {
                        GetConcreteLexeme(",");
                    }
                    catch (SyntaxException e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    try
                    {
                        Exp = ParseExpression();
                    }
                    catch (SyntaxException e)
                    {
                        Console.WriteLine(e.Message);
                        Exp = new ConstantNode("string", "", GetLexeme().Line);
                    }
                    NodesStack.Peek().AddOperator(new WriteOperator(Exp, CurrentLexeme.Line));
                    LexemesIterator++;
                    CurrentLexeme = GetLexeme();
                }

                try
                {
                    GetConcreteLexeme(")");
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                }
                try
                {
                    GetConcreteLexeme(";");
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            //получение следующего оператора
            public static void GetNextOperator()
            {
                Lexeme CurrentLexeme;
                try
                {
                    CurrentLexeme = GetLexeme();
                    if (Types.Contains(CurrentLexeme.Value))
                        ParseDeclaration();
                    else if (CurrentLexeme.Value == "if")
                        ParseConditionOperator();
                    else if (CurrentLexeme.Value == "while")
                        ParsePredConditionCycleOPerator();
                    else if (CurrentLexeme.Value == "do")
                        ParsePostConditionCycle();
                    else if (CurrentLexeme.Value == "for")
                        ParseForCycle();
                    else if (CurrentLexeme.Value == "scan")
                        ParseReadOperator();
                    else if (CurrentLexeme.Value == "print")
                        ParseWriteOperator();
                    else if (Types.Contains(CurrentLexeme.Value))
                        ParseDeclaration();
                    else if (CurrentLexeme.Value==";")//пустой оператор
                    {
                        GetConcreteLexeme(";");
                    }
                    else if (LexemesIterator+1<LexemsForSyntaxAnalysis.Count && Operations.AssignmentOperations.Contains(LexemsForSyntaxAnalysis[LexemesIterator + 1].Value))
                    {
                        AssignmentOperator AssignOp = ParseAssignmentOperator();
                        NodesStack.Peek().AddOperator(AssignOp);
                        GetConcreteLexeme(";");
                    }
                    else if (char.IsLetterOrDigit(CurrentLexeme.Value[0]) || CurrentLexeme.Value[0]=='\"' || CurrentLexeme.Value[0] == '\'' || CurrentLexeme.Value=="(")
                    {
                        AtomNode Exp = ParseExpression();
                        NodesStack.Peek().AddOperator(Exp);
                        GetConcreteLexeme(";");
                    }
                    else
                    {
                        throw new UnexpectedTokenException(CurrentLexeme.Line, CurrentLexeme.Value);
                    }

                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                }
                LexemesIterator++;
                if (LexemesIterator == LexemsForSyntaxAnalysis.Count)
                    return;
            }

            public static void Parse()
            {
                GetOperationsPriorities();
                DoLexemsList();
                MainRootNode root = new MainRootNode(LexemsForSyntaxAnalysis[0].Line);
                LexemesIterator = 0;
                NodesStack.Push(root);
                try
                {
                    while (LexemesIterator < LexemsForSyntaxAnalysis.Count)
                        GetNextOperator();
                }
                catch(UnexpectedEOFException)//все остальные иключения должны отлавливаться выше по рекурсии
                {
                    HandleEOFException();
                }
                Root = root;
            }
        }
    }
}
