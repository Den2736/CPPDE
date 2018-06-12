using C__DE.Models.Exceptions.SemanticExceptions;
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
        //число вершин
        public AtomNode InputVar;
        //Граф
        public VariableNode graph;
        public CreatingGraphNode(VariableNode newGraph, AtomNode vertecies, int numLine)
        {
            graph = newGraph;
            InputVar = vertecies;
            LineNumber = numLine;
        }

        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
            InputVar.SetParentBlock(Parent);
            graph.SetParentBlock(Parent);
        }

        public override string CanCreateGraph()
        {
            return "";
        }

        public override bool SemanticAnalysis()
        {
            //По сути, создавая граф, мы объявляем новую переменную тип graph
            MainVariable = new Variable();
            numGraph = ++Counters.graphs;
            MainVariable.AlternativeName = "graph_" + (numGraph).ToString();
            MainVariable.IsDeclared = true;
            MainVariable.WasIdentified = true; //переменная объявлена и идентифицирована
            MainVariable.WasNewValueUsed = false;
            MainVariable.WasUsed = false;
            MainVariable.Name = graph.Value;
            MainVariable.DeclaredLine = LineNumber;
            MainVariable.Type = "graph";
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
                graph.SemanticAnalysis();
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
            IntermediateCodeList.push(new CreateGraphInterNode(MainVariable));
        }
    }

    //Добавление ребра (сделать так, чтобы ребро нельзя было добавить петлю, кратных рёбер НЕ будет)
    public class SetEdgeNode: AtomNode
    {
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
                    if (Res.MainVariable.Type!="int")
                        throw new WrongOperandTypeException(LineNumber, "SetEdge", 4, "int");
                    if (!Res.MainVariable.WasIdentified)
                        throw new UnidentifiedVariableException(LineNumber, Res.MainVariable.Name);
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

        public override string CanCreateGraph()
        {
            return "";
        }

        public override void GenerateIntermediateCode()
        {
            //Нельзя добавить петлю
            IntermediateCodeList.push(new CmpNode(first.MainVariable, second.MainVariable));
            string label = "comp_" + (++Counters.comparsions).ToString();
            IntermediateCodeList.push(new GoToLabel(label, "je"));
            //граф будет неориентированный, поэтому добавляем в обе стороны
            IntermediateCodeList.push(new SetGraphCell(Res.MainVariable, new GraphCell(graph.MainVariable, first.MainVariable, second.MainVariable)));
            IntermediateCodeList.push(new SetGraphCell(Res.MainVariable, new GraphCell(graph.MainVariable, second.MainVariable, first.MainVariable)));
            IntermediateCodeList.push(new PutLabel(label));
        }
    }

    public class GetEdgeNode: AtomNode
    {
            public VariableNode graph;
            public AtomNode first; //первая вершина
            public AtomNode second; //вторяа вершина
            public AtomNode Res; //результирующая переменная

            public GetEdgeNode(VariableNode gr, AtomNode firstVert, AtomNode secondVert, int Line, AtomNode Result)
            {
                graph = gr;
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
                    graph.MainVariable.WasNewValueUsed = true;
                    if (graph.MainVariable.Type != "graph")
                        throw new WrongOperandTypeException(LineNumber, "GetEdge", 1, "graph");

                }
                catch (SemanticException e)
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
                        throw new WrongOperandTypeException(LineNumber, "GetEdge", 2, "int");
                    if (!first.MainVariable.WasIdentified)
                        throw new UnidentifiedVariableException(LineNumber, first.MainVariable.Name);
                }
                catch (SemanticException e)
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
                        throw new WrongOperandTypeException(LineNumber, "GetEdge", 3, "int");
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
                        Res.MainVariable.WasIdentified = true;
                        if (Res.TypeOfNode != NodeType.Variable)
                            throw new WrongOperandTypeException(LineNumber, "GetEdge", 4, "int variable");
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

            public override string CanCreateGraph()
            {
                return "";
            }

            public override void GenerateIntermediateCode()
            {
                IntermediateCodeList.push(new GetGraphCell(Res.MainVariable, new GraphCell(graph.MainVariable, first.MainVariable, second.MainVariable)));
            }
        }

    //Сначала КУДА копируем, потом ОТКУДА
    public class CopyGraph: AtomNode
    {
        public VariableNode outGraph;
        public AtomNode inGraph;

        public CopyGraph(VariableNode First, AtomNode Second, int Line)
        {
            outGraph = First;
            inGraph = Second;
            LineNumber = Line;
        }

        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
            outGraph.SetParentBlock(Parent);
            inGraph.SetParentBlock(Parent);
        }

        public override string CanCreateGraph()
        {
            return "";
        }

        public override bool SemanticAnalysis()
        {
            try
            {
                IsSemanticCorrect = outGraph.SemanticAnalysis();
                outGraph.MainVariable.WasUsed = true;
                outGraph.MainVariable.WasNewValueUsed = false;
                outGraph.MainVariable.WasAssignedNewValue = LineNumber;
                if (outGraph.MainVariable.Type != "graph")
                    throw new WrongOperandTypeException(LineNumber, "CopyGraph", 1, "graph");
            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }

            try
            {
                IsSemanticCorrect &= inGraph.SemanticAnalysis();
                inGraph.MainVariable.WasUsed = true;
                inGraph.MainVariable.WasNewValueUsed = true;
                if (inGraph.MainVariable.Type != "graph" || (inGraph.TypeOfNode != NodeType.Variable))
                    throw new WrongOperandTypeException(LineNumber, "CopyGraph", 2, "graph");
                if (inGraph.IsSemanticCorrect && outGraph.IsSemanticCorrect) //ещё надо сравнить размерности
                    if (int.Parse(inGraph.MainVariable.Value) != int.Parse(outGraph.MainVariable.Value)) //если не равны
                        throw new UnequalGraphsDimentionsException(LineNumber, outGraph.MainVariable.Name, inGraph.MainVariable.Name);
                
            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }
            return IsSemanticCorrect;
        }

        public override void GenerateIntermediateCode()
        {
            IntermediateCodeList.push(new CopyGraphsInterNode(outGraph.MainVariable, inGraph.MainVariable));
        }
    }

    public class FloydNode: AtomNode
    {
        public VariableNode outGraph;
        public AtomNode inGraph;

        public FloydNode(VariableNode First, AtomNode Second, int Line)
        {
            outGraph = First;
            inGraph = Second;
            LineNumber = Line;
        }

        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
            outGraph.SetParentBlock(Parent);
            inGraph.SetParentBlock(Parent);
        }

        public override string CanCreateGraph()
        {
            return "";
        }

        public override bool SemanticAnalysis()
        {
            try
            {
                IsSemanticCorrect = outGraph.SemanticAnalysis();
                outGraph.MainVariable.WasUsed = true;
                outGraph.MainVariable.WasNewValueUsed = false;
                outGraph.MainVariable.WasAssignedNewValue = LineNumber;
                if (outGraph.MainVariable.Type != "graph")
                    throw new WrongOperandTypeException(LineNumber, "Floyd", 1, "graph");
            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }

            try
            {
                IsSemanticCorrect &= inGraph.SemanticAnalysis();
                inGraph.MainVariable.WasUsed = true;
                inGraph.MainVariable.WasNewValueUsed = true;
                if (inGraph.MainVariable.Type != "graph" || (inGraph.TypeOfNode != NodeType.Variable))
                    throw new WrongOperandTypeException(LineNumber, "Floyd", 2, "graph");
                if (IsSemanticCorrect) //ещё надо сравнить размерности
                    if (int.Parse(inGraph.MainVariable.Value) != int.Parse(outGraph.MainVariable.Value)) //если не равны
                        throw new UnequalGraphsDimentionsException(LineNumber, outGraph.MainVariable.Name, inGraph.MainVariable.Name);

            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }
            return IsSemanticCorrect;
        }

        public override void GenerateIntermediateCode()
        {
            IntermediateCodeList.push(new CopyGraphsInterNode(outGraph.MainVariable, inGraph.MainVariable));
            IntermediateCodeList.push(new FloydCall(outGraph.MainVariable));
            GeneratingAssembleCode.WasFloydUsed = true;
        }
    }

    public class NumComponentsNode: AtomNode
    {
        public VariableNode graph;
        public AtomNode outVariable;
        public NumComponentsNode(VariableNode gr, AtomNode outVar, int Line)
        {
            graph = gr;
            outVariable = outVar;
            LineNumber = Line;
        }

        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
            outVariable.SetParentBlock(Parent);
            graph.SetParentBlock(Parent);
        }

        public override string CanCreateGraph()
        {
            return "";
        }

        public override bool SemanticAnalysis()
        {
            try
            {
                IsSemanticCorrect = graph.SemanticAnalysis();
                graph.MainVariable.WasUsed = true;
                graph.MainVariable.WasNewValueUsed = true;
                if (graph.MainVariable.Type != "graph")
                    throw new WrongOperandTypeException(LineNumber, "NumComponents", 1, "graph");

            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }

            try
            {
                IsSemanticCorrect &= outVariable.SemanticAnalysis();
                outVariable.MainVariable.WasUsed = true;
                outVariable.MainVariable.WasNewValueUsed = false;
                outVariable.MainVariable.WasIdentified = true;
                outVariable.MainVariable.WasAssignedNewValue = LineNumber;
                if (outVariable.TypeOfNode != NodeType.Variable || (outVariable.MainVariable.Type != "int"))
                    throw new WrongOperandTypeException(LineNumber, "NumComponents", 2, "int variable");
            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }
            //возвращаемся
            return IsSemanticCorrect;
        }

        public override void GenerateIntermediateCode()
        {
            Variable arr = new Variable();
            arr.AlternativeName = "array_" + (++Counters.arrays).ToString();
            arr.Type = "array";
            arr.Value = graph.MainVariable.Value;
            //Создали массив для вычислений
            IntermediateCodeList.addVar(arr);
            IntermediateCodeList.push(new NumComponentsCall(graph.MainVariable, arr, outVariable.MainVariable));
            GeneratingAssembleCode.WasDFSUsed = true;
            GeneratingAssembleCode.NumComponentsUsed = true;
        }
    }

    public class CountEdgesNode: AtomNode
    {
        public VariableNode graph;
        public AtomNode outVariable;
        public CountEdgesNode(VariableNode gr, AtomNode outVar, int Line)
        {
            graph = gr;
            outVariable = outVar;
            LineNumber = Line;
        }

        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
            outVariable.SetParentBlock(Parent);
            graph.SetParentBlock(Parent);
        }

        public override string CanCreateGraph()
        {
            return "";
        }

        public override bool SemanticAnalysis()
        {
            try
            {
                IsSemanticCorrect = graph.SemanticAnalysis();
                graph.MainVariable.WasUsed = true;
                graph.MainVariable.WasNewValueUsed = true;
                if (graph.MainVariable.Type != "graph")
                    throw new WrongOperandTypeException(LineNumber, "NumComponents", 1, "graph");

            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }

            try
            {
                IsSemanticCorrect &= outVariable.SemanticAnalysis();
                outVariable.MainVariable.WasUsed = true;
                outVariable.MainVariable.WasNewValueUsed = false;
                outVariable.MainVariable.WasIdentified = true;
                outVariable.MainVariable.WasAssignedNewValue = LineNumber;
                if (outVariable.TypeOfNode != NodeType.Variable || (outVariable.MainVariable.Type != "int"))
                    throw new WrongOperandTypeException(LineNumber, "NumComponents", 2, "int variable");
            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }
            //возвращаемся
            return IsSemanticCorrect;
        }

        public override void GenerateIntermediateCode()
        {
            IntermediateCodeList.push(new CountEdgesInterNode(graph.MainVariable, outVariable.MainVariable));
            GeneratingAssembleCode.WasCountEdgesUsed = true;
        }
    }

    public class IsTreeNode: AtomNode
    {
        public VariableNode graph;
        public AtomNode outVariable;
        public IsTreeNode(VariableNode gr, AtomNode outVar, int Line)
        {
            graph = gr;
            outVariable = outVar;
            LineNumber = Line;
        }

        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
            outVariable.SetParentBlock(Parent);
            graph.SetParentBlock(Parent);
        }

        public override string CanCreateGraph()
        {
            return "";
        }

        public override bool SemanticAnalysis()
        {
            try
            {
                IsSemanticCorrect = graph.SemanticAnalysis();
                graph.MainVariable.WasUsed = true;
                graph.MainVariable.WasNewValueUsed = true;
                if (graph.MainVariable.Type != "graph")
                    throw new WrongOperandTypeException(LineNumber, "IsTree", 1, "graph");

            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }

            try
            {
                IsSemanticCorrect &= outVariable.SemanticAnalysis();
                outVariable.MainVariable.WasUsed = true;
                outVariable.MainVariable.WasNewValueUsed = false;
                outVariable.MainVariable.WasIdentified = true;
                outVariable.MainVariable.WasAssignedNewValue = LineNumber;
                if ( (outVariable.TypeOfNode != NodeType.Variable) || (outVariable.MainVariable.Type!="bool"))
                    throw new WrongOperandTypeException(LineNumber, "IsTree", 2, "bool variable");
            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }
            //возвращаемся
            return IsSemanticCorrect;
        }

        public override void GenerateIntermediateCode()
        {
            Variable temp = new Variable();
            temp.Type = "int";
            temp.AlternativeName = "temp_" + (++Counters.temps);
            IntermediateCodeList.addVar(temp);

            Variable tr = new Variable();
            tr.IsConst = true;
            tr.AlternativeName = "const_" + (++Counters.consts).ToString();
            tr.Value = "true";
            tr.Type = "bool";
            IntermediateCodeList.addVar(tr);

            Variable fl = new Variable();
            fl.IsConst = true;
            fl.AlternativeName = "const_" + (++Counters.consts).ToString();
            fl.Value = "false";
            fl.Type = "bool";
            IntermediateCodeList.addVar(fl);

            Variable Edges = new Variable();
            Edges.IsConst = true;
            Edges.AlternativeName = "const_" + (++Counters.consts).ToString();
            Edges.Value = (int.Parse(graph.MainVariable.Value) - 1).ToString();
            Edges.Type = "int";
            IntermediateCodeList.addVar(Edges);

            Variable temp_arr = new Variable();
            temp_arr.Value = graph.MainVariable.Value;
            temp_arr.Type = "array";
            temp_arr.AlternativeName = "array_" + (++Counters.arrays).ToString();
            IntermediateCodeList.addVar(temp_arr);

            Variable comps = new Variable();
            comps.IsConst = true;
            comps.AlternativeName = "const_" + (++Counters.consts).ToString();
            comps.Value = "1";
            comps.Type = "int";
            IntermediateCodeList.addVar(comps);

            //Считаем рёбра
            IntermediateCodeList.push(new CountEdgesInterNode(graph.MainVariable, temp));
            //српавниваем
            IntermediateCodeList.push(new CmpNode(temp, Edges));
            string label = "not_tree_" + (++Counters.comparsions).ToString();
            //Если не совпадает - плохо
            IntermediateCodeList.push(new GoToLabel(label, "jne"));
            //Если пока совпадает, проверим на связность
            IntermediateCodeList.push(new NumComponentsCall(graph.MainVariable, temp_arr, temp));
            
            IntermediateCodeList.push(new CmpNode(temp, comps));
            //Если не совпадает - плохо
            IntermediateCodeList.push(new GoToLabel(label, "jne"));
            IntermediateCodeList.push(new AssignmentInterNode(tr, outVariable.MainVariable));
            IntermediateCodeList.push(new GoToLabel(label+"_exit", "jmp"));
            IntermediateCodeList.push(new PutLabel(label));
            IntermediateCodeList.push(new AssignmentInterNode(fl, outVariable.MainVariable));
            IntermediateCodeList.push(new PutLabel(label + "_exit"));
            GeneratingAssembleCode.WasCountEdgesUsed = true;
            GeneratingAssembleCode.NumComponentsUsed = true;
            GeneratingAssembleCode.WasDFSUsed = true;
        }
    }

    public class IsFullNode: AtomNode
    {
        public VariableNode graph;
        public AtomNode outVariable;
        public IsFullNode(VariableNode gr, AtomNode outVar, int Line)
        {
            graph = gr;
            outVariable = outVar;
            LineNumber = Line;
        }

        public override void SetParentBlock(BlockNode Parent)
        {
            parentBlock = Parent;
            outVariable.SetParentBlock(Parent);
            graph.SetParentBlock(Parent);
        }

        public override string CanCreateGraph()
        {
            return "";
        }

        public override bool SemanticAnalysis()
        {
            try
            {
                IsSemanticCorrect = graph.SemanticAnalysis();
                graph.MainVariable.WasUsed = true;
                graph.MainVariable.WasNewValueUsed = true;
                if (graph.MainVariable.Type != "graph")
                    throw new WrongOperandTypeException(LineNumber, "IsFull", 1, "graph");

            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }

            try
            {
                IsSemanticCorrect &= outVariable.SemanticAnalysis();
                outVariable.MainVariable.WasUsed = true;
                outVariable.MainVariable.WasNewValueUsed = false;
                outVariable.MainVariable.WasIdentified = true;
                outVariable.MainVariable.WasAssignedNewValue = LineNumber;
                if ((outVariable.TypeOfNode != NodeType.Variable) || (outVariable.MainVariable.Type != "bool"))
                    throw new WrongOperandTypeException(LineNumber, "IsFull", 2, "bool variable");
            }
            catch (SemanticException e)
            {
                Console.WriteLine(e.Message);
                IsSemanticCorrect = false;
            }
            //возвращаемся
            return IsSemanticCorrect;
        }

        public override void GenerateIntermediateCode()
        {
            Variable temp = new Variable();
            temp.Type = "int";
            temp.AlternativeName = "temp_" + (++Counters.temps);
            IntermediateCodeList.addVar(temp);

            Variable tr = new Variable();
            tr.IsConst = true;
            tr.AlternativeName = "const_" + (++Counters.consts).ToString();
            tr.Value = "true";
            tr.Type = "bool";
            IntermediateCodeList.addVar(tr);

            Variable fl = new Variable();
            fl.IsConst = true;
            fl.AlternativeName = "const_" + (++Counters.consts).ToString();
            fl.Value = "false";
            fl.Type = "bool";
            IntermediateCodeList.addVar(fl);

            Variable Edges = new Variable();
            Edges.IsConst = true;
            Edges.AlternativeName = "const_" + (++Counters.consts).ToString();
            Edges.Value = ((int.Parse(graph.MainVariable.Value) * (int.Parse(graph.MainVariable.Value) - 1)) / 2).ToString();
            Edges.Type = "int";
            IntermediateCodeList.addVar(Edges);

            //Считаем рёбра
            IntermediateCodeList.push(new CountEdgesInterNode(graph.MainVariable, temp));
            //српавниваем
            IntermediateCodeList.push(new CmpNode(temp, Edges));
            string label = "not_full_" + (++Counters.comparsions).ToString();
            //Если не совпадает - плохо
            IntermediateCodeList.push(new GoToLabel(label, "jne"));
            //Если совпадает, то всё хорошо
            IntermediateCodeList.push(new AssignmentInterNode(tr, outVariable.MainVariable));
            IntermediateCodeList.push(new GoToLabel(label + "_exit", "jmp"));
            IntermediateCodeList.push(new PutLabel(label));
            IntermediateCodeList.push(new AssignmentInterNode(fl, outVariable.MainVariable));
            IntermediateCodeList.push(new PutLabel(label + "_exit"));
            GeneratingAssembleCode.WasCountEdgesUsed = true;
        }
    }
}
