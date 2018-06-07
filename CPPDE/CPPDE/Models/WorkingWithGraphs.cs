﻿using C__DE.Models.Exceptions.SemanticExceptions;
using C__DE.Models.Warnings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__DE.Models
{
    public partial class AtomNode
    {
        public abstract string CanCreateGraph(); //проверить, можно ли компилятор вычислить значение как константу
    }

    public partial class ConstantNode
    {
        public override string CanCreateGraph()
        {
            if (MainVariable.Type == "int")
                return MainVariable.Value;
            else return "";
        }
    }

    public partial class VariableNode
    {
        public override string CanCreateGraph()
        {
            return "";
        }
    }

    public partial class BinaryOperatorNode
    {
        public override string CanCreateGraph()
        {
            if (FirstOperand.MainVariable.Type != "int" || FirstOperand.CanCreateGraph() == "")
                return "";
            if (IsUnary)
            {
                if (Value == "-")
                {
                    MainVariable.Value = (-int.Parse(FirstOperand.MainVariable.Value)).ToString();
                }
                else return "";
            }
            else
            {
                if (SecondOperand.MainVariable.Type != "int" || SecondOperand.CanCreateGraph() == "")
                    return "";
                else
                    switch (Value)
                    {
                        case ("+"):
                            {
                                MainVariable.Value = (int.Parse(FirstOperand.MainVariable.Value)+ 
                                    int.Parse(SecondOperand.MainVariable.Value)).ToString();
                                break;
                            }
                        case ("-"):
                            {
                                MainVariable.Value = (int.Parse(FirstOperand.MainVariable.Value) -
                                    int.Parse(SecondOperand.MainVariable.Value)).ToString();
                                break;
                            }
                        case ("*"):
                            {
                                {
                                    MainVariable.Value = (int.Parse(FirstOperand.MainVariable.Value) *
                                        int.Parse(SecondOperand.MainVariable.Value)).ToString();
                                    break;
                                }
                            }
                        case ("/"):
                            {
                                if (int.Parse(SecondOperand.MainVariable.Value) == 0)
                                    throw new DividingByZeroWarning(LineNumber);

                                MainVariable.Value = (int.Parse(FirstOperand.MainVariable.Value) /
                                    int.Parse(SecondOperand.MainVariable.Value)).ToString();
                                break;
                            }
                        case ("%"):
                            {
                                if (int.Parse(SecondOperand.MainVariable.Value) == 0)
                                    throw new DividingByZeroWarning(LineNumber);

                                MainVariable.Value = (int.Parse(FirstOperand.MainVariable.Value) %
                                    int.Parse(SecondOperand.MainVariable.Value)).ToString();
                                break;
                            }
                        case ("&"):
                            {
                                MainVariable.Value = (int.Parse(FirstOperand.MainVariable.Value) &
                                    int.Parse(SecondOperand.MainVariable.Value)).ToString();
                                break;
                            }
                        case ("|"):
                            {
                                MainVariable.Value = (int.Parse(FirstOperand.MainVariable.Value) |
                                    int.Parse(SecondOperand.MainVariable.Value)).ToString();
                                break;
                            }
                        case ("^"):
                            {
                                MainVariable.Value = (int.Parse(FirstOperand.MainVariable.Value) ^
                                    int.Parse(SecondOperand.MainVariable.Value)).ToString();
                                break;
                            }
                        default:
                            {
                                throw new IntIsRequiredException(LineNumber);
                            }
                    }  
            }
            return MainVariable.Value;
        }
    }

    public partial class VariableDeclarationNode
    {
        public override string CanCreateGraph()
        {
            return "";
        }
    }

    public partial class AssignmentOperator
    {
        public override string CanCreateGraph()
        {
            return "";
        }
    }

    public partial class ReadOperator
    {
        public override string CanCreateGraph()
        {
            return "";
        }
    }

    public partial class WriteOperator
    {
        public override string CanCreateGraph()
        {
            return "";
        }
    }

    //Создание графа - создание нового или копирование существующего
    //Синтаксис: CreateGrapg(graph, int)
    public class CreatingGraphNode: AtomNode
    {
        public int NumVertecies; //число вершин в графе
        public int numGraph; //номер данного графа в программе


        public AtomNode InputVar;
        public CreatingGraphNode(VariableNode newGraph, AtomNode vertecies, int numLine)
        {
            MainVariable = newGraph.MainVariable;
            MainVariable.Type = "graph";
            InputVar = vertecies;
            LineNumber = numLine;
        }

        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
            InputVar.SetParentBlock(Parent);
        }

        public override string CanCreateGraph()
        {
            return "";
        }

        public override bool SemanticAnalysis()
        {
            //По сути, создавая граф, мы объявляем новую переменную тип graph
            numGraph = ++Counters.graphs;
            MainVariable.AlternativeName = "graph_" + (++Counters.graphs).ToString();
            MainVariable.IsDeclared = true;
            MainVariable.WasIdentified = true; //переменная объявлена и идентифицирована
            MainVariable.WasNewValueUsed = false;
            MainVariable.WasUsed = false;
            try
            {
                Variable possibleVariable = parentBlock.BlockVariables.FirstOrDefault(var => var.Name == MainVariable.Name);
                if (possibleVariable != null)
                {
                    IsSemanticCorrect = false;
                    possibleVariable.Type = "graph"; //меняем тип
                    throw new RedeclaringVariableException(LineNumber, MainVariable.Name);
                }
                parentBlock.BlockVariables.Add(MainVariable); //помещаем в список переменных данного блока
                IsSemanticCorrect = true;
            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false; //плохо
            }

            try
            {
                IsSemanticCorrect &= InputVar.SemanticAnalysis();
            }
            catch(SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }

            if (InputVar.MainVariable.Type != "graph")
            {
                try
                {
                    string num = InputVar.CanCreateGraph();
                    if (num == "")
                    {
                        IsSemanticCorrect = false;
                        throw new RequredConstantExceptoion(LineNumber);
                    }
                    else
                    {
                        NumVertecies = int.Parse(num);
                        if (NumVertecies <= 0)
                            throw new PositiveIntegerIsRequiredException(LineNumber);
                        MainVariable.Value = num;
                    }
                }
                catch (SemanticException e)
                {
                    Console.WriteLine(e.Message);
                    IsSemanticCorrect = false;
                }
                catch (DividingByZeroWarning w)
                {
                    Console.WriteLine(w.Message);
                    IsSemanticCorrect = false;
                }
            }
            return IsSemanticCorrect;
        }

        public override void GenerateIntermediateCode()
        {
            IntermediateCodeList.addVar(MainVariable);
        }
    }

    //Добавление ребра (сделать так, чтобы ребро нельзя было добавить петлю, кратных рёбер НЕ будет)
    public class SetEdgeNode: AtomNode
    {
        //MainVariable - сам граф
        public VariableNode graph;
        public AtomNode first; //первая вершина
        public AtomNode second; //вторяа вершина
        public AtomNode Res; //результирующая переменная

        public SetEdgeNode(VariableNode gr, AtomNode firstVert, AtomNode secondVert, int Line, AtomNode Result)
        {
            graph=gr;
            first = firstVert;
            second = secondVert;
            LineNumber = Line;
            Res = Result;
        }

        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
            graph.SetParentBlock(Parent);
            first.SetParentBlock(Parent);
            second.SetParentBlock(Parent);
            Res.SetParentBlock(Parent);
        }

        public override bool SemanticAnalysis()
        {
            try
            {
                IsSemanticCorrect = graph.SemanticAnalysis();
                graph.MainVariable.WasUsed = true;
                graph.MainVariable.WasNewValueUsed=true;
                if (graph.MainVariable.Type!="graph")
                    throw new WrongOperandTypeException(LineNumber, "SetEdge", 1, "graph");

            }
            catch(SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }

            //проверяем первую вершину
            try
            {
                IsSemanticCorrect &= first.SemanticAnalysis();
                first.MainVariable.WasUsed = true;
                first.MainVariable.WasNewValueUsed = true;
                if (first.MainVariable.Type != "int")
                    throw new WrongOperandTypeException(LineNumber, "SetEdge", 2, "int");
                if (!first.MainVariable.WasIdentified)
                    throw new UnidentifiedVariableException(LineNumber, first.MainVariable.Name);
            }
            catch(SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }

            //аналогично вторую
            try
            {
                IsSemanticCorrect &= second.SemanticAnalysis();
                second.MainVariable.WasUsed = true;
                second.MainVariable.WasNewValueUsed = true;
                if (second.MainVariable.Type != "int")
                    throw new WrongOperandTypeException(LineNumber, "SetEdge", 3, "int");
                if (!second.MainVariable.WasIdentified)
                    throw new UnidentifiedVariableException(LineNumber, second.MainVariable.Name);
            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }

            //теперь результирующая
            {
                try
                {
                    IsSemanticCorrect &= Res.SemanticAnalysis();
                    Res.MainVariable.WasUsed = true;
                    Res.MainVariable.WasNewValueUsed = false;
                    if (Res.TypeOfNode != NodeType.Variable)
                        throw new WrongOperandTypeException(LineNumber, "SetEdge", 4, "int variable");
                }
                catch (SemanticException e)
                {
                    Console.WriteLine(e.Message);
                    IsSemanticCorrect = false;
                }
                 //возвращаемся
                return IsSemanticCorrect;
            }
        }
    }
}