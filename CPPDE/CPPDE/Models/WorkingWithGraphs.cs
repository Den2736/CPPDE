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

        public void CheckTypesGraph()
        {
            if (RightPart.CanCreateGraph()=="")
                throw new RequredConstantExceptoion(LineNumber);
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
}
