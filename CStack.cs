using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolkFreeCalc2
{
    /// <summary>
    /// Essentially a RPN-calculator with four registers X, Y, Z, T. Like the HP <br/>
    /// RPN calculators.Numeric values are entered in the entry string by<br/>
    /// adding digits and one comma. For test purposes the method<br/>
    /// RollSetX can be used instead. Operations can be performed on the<br/>
    /// calculator preferrably by using one of the methods:
    /// <list type="number">
    ///     <item><b>BinOp</b> - merges X and Y into X via an operation and rolls
    ///           down the stack</item>
    ///     <item><b>UnOp</b> - operates X and puts the result in X with
    ///           overwrite</item>
    ///     <item><b>Nilop</b> - adds a known constant on the stack and rolls up
    ///           the stack</item>
    /// </list>
    /// </summary>
    public class CStack
    {
        /// <summary>
        /// The conventional X, Y, Z and T registers of a HP calculator, where<br/>
        /// X is the first pushed to when pressing Enter(), and T the last one.
        /// </summary>
        public double X, Y, Z, T;
        /// <summary>
        /// The text entry where strings are composed intended to be numbers
        /// to be pushed onto the stack.
        /// </summary>
        public string entry;
        /// <summary>
        /// Creates a new stack and inits X, Y, Z, T and the text entry.
        /// </summary>
        public CStack()
        {
            X = Y = Z = T = 0;
            entry = "";
        }
        /// <summary>
        /// Construct a string to write out in the stack view
        /// </summary>
        /// <returns>a string containing the values T, Z, Y, X with newlines 
        /// between them</returns>
        public string StackString()
        {
            return $"{T}\n{Z}\n{Y}\n{X}\n{entry}";
        }
        /// <summary>
        /// Sets X with overwrite.
        /// </summary>
        /// <param name="newX">the new value to put in X</param>
        public void SetX(double newX)
        {
            X = newX;
        }
        /// <summary>
        /// Adds a digit to the entry string.
        /// </summary>
        /// <param name="digit">the candidate digit to add at the end of the</param>
        /// <remarks>If the string digit does not contain a parseable<br/>
        /// integer, nothing is added to the entry</remarks>
        public void EntryAddNum(string digit)
        {
            int val;
            if (int.TryParse(digit, out val))
            {
                entry = entry + val;
            }
        }
        /// <summary>
        /// Adds a comma to the entry string.
        /// </summary>
        /// <remarks>If the entry string already contains a comma,<br/>
        /// nothing is added to the entry.</remarks>
        public void EntryAddComma()
        {
            if (entry.IndexOf(",") == -1)
                entry = entry + ",";
        }
        /// <summary>
        /// Changes the sign of the entry string.
        /// </summary>
        /// <remarks>If the first char is already a '−' it is exchanged for a '+'.<br/>
        /// If it is a '+' it is changed to a '−', otherwise a '−' is just<br/>
        /// added first.</remarks>
        public void EntryChangeSign()
        {
            char[] cval = entry.ToCharArray();
            if (cval.Length > 0)
            {
                switch (cval[0])
                {
                    case '+': cval[0] = '-'; entry = new string(cval); break;
                    case '-': cval[0] = '+'; entry = new string(cval); break;
                    default: entry = '-' + entry; break;
                }
            }
            else
            {
                entry = '-' + entry;
            }
        }
        /// <summary>
        /// Converts the entry to a double and puts it into X.
        /// </summary>
        /// <remarks>The entry is cleared after a successful operation.</remarks>
        public void Enter()
        {
            if (entry != "")
            {
                RollSetX(double.Parse(entry));
                entry = "";
            }
        }
        /// <summary>
        /// Drops the value of X, and rolls down. X gets the value of Y.<br/>
        /// Y gets the value of Z and Z gets the value of T, while T keeps<br/>
        /// the same value as before.</summary>
        public void Drop()
        {
            X = Y; Y = Z; Z = T;
        }
        /// <summary>
        /// Replaces the value of X, and rolls down. Y gets the<br/>
        /// value of Z. Z gets the value of T, while T keeps same<br/>
        /// value as before.
        /// </summary>
        /// <param name="newX">the replacement value for X</param>
        /// <remarks>this is used when applying binary operations consuming X<br/>
        /// and Y and putting the result in X, while rolling down the
        /// stack.</remarks>
        public void DropSetX(double newX)
        {
            X = newX; Y = Z; Z = T;
        }
        /// <summary>
        /// Evaluates a binary operation.
        /// </summary>
        /// <param name="op">the binary operation retrieved from the<br/>
        /// GUI buttons</param>
        /// <remarks>The stack is automatically rolled down.</remarks>
        public void BinOp(string op)
        {
            switch (op)
            {
                case "+": DropSetX(Y + X); break;
                case "−": DropSetX(Y - X); break;
                case "×": DropSetX(Y * X); break;
                case "÷": DropSetX(Y / X); break;
                case "yˣ": DropSetX(Math.Pow(Y, X)); break;
                case "ˣ√y": DropSetX(Math.Pow(Y, 1.0 / X)); break;
            }
        }
        /// <summary>
        /// Evaluates a unary operation.
        /// </summary>
        /// <param name="op">the unary operation retrieved from the<br/>
        /// GUI buttons</param>
        /// <remarks>The stack is not moved, X is replaced by the result<br/>
        /// of the operation. Operations available:<br/>
        /// x², √x, log x, ln x, 10ˣ, eˣ, sin, cos, tan, sin⁻¹, cos⁻¹, tan⁻¹
        /// </remarks>
        public void Unop(string op)
        {
            switch (op)
            {
                // Powers & Logarithms:
                case "x²": SetX(X * X); break;
                case "√x": SetX(Math.Sqrt(X)); break;
                case "log x": SetX(Math.Log10(X)); break;
                case "ln x": SetX(Math.Log(X)); break;
                case "10ˣ": SetX(Math.Pow(10, X)); break;
                case "eˣ": SetX(Math.Exp(X)); break;

                // Trigonometry:
                case "sin": SetX(Math.Sin(X)); break;
                case "cos": /* NYI: Cosine */ break;
                case "tan": /* NYI: Tangent */ break;
                case "sin⁻¹": /* NYI: Arc Sine */ break;
                case "cos⁻¹": /* NYI: Arc Cosine */ break;
                case "tan⁻¹": /* NYI: Arc Tangent */ break;
            }
        }
        /// <summary>
        /// Evaluates a nilary operation, that is: insert a constant.
        /// </summary>
        /// <param name="op">the nilary operation retrieved from the<br/>
        /// GUI buttons</param>
        /// <remarks>The stack is rolled up. Operations available:<br/>
        /// π, e
        /// </remarks>
        /// <example></example>
        public void Nilop(string op)
        {
            switch (op)
            {
                case "π": RollSetX(Math.PI); break;
                case "e": RollSetX(Math.E); break;
            }
        }
        /// <summary>
        /// Rolls the stack up.
        /// </summary>
        public void Roll()
        {
            double tmp = T;
            T = Z; Z = Y; Y = X; X = tmp;
        }
        /// <summary>
        /// Rolls the stack up and sets X to a new value
        /// </summary>
		/// <param name="newX">the new value to put into X</param>
        /// <remarks>
        /// T is dropped.
        /// </remarks>
        public void RollSetX(double newX)
        {
            T = Z; Z = Y; Y = X; X = newX;
        }
        /// <summary>
        /// Sets the variables A, B or C to the content of X depending on the<br/>
        /// value of parameter op, which is expected to have one of the<br/>
        /// values "STO A", "STO B" or "STO C".
        /// </summary>
        /// <param name="op"></param>
        public void Setvar(string op)
        {
            // NYI!
        }
        /// <summary>
        /// Pushes the content of the var A, B or C onto the stack depending on the<br/>
        /// value of parameter op, which is expected to have one of the values "RCL A",<br/>
        /// "RCL B" or "RCL C".
        /// </summary>
        /// <param name="op"></param>
        public void GetVar(string op)
        {
            // NYI!
        }
    }
}
