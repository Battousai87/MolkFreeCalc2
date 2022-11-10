using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolkFreeCalc2
{
    /// <summary>
    /// <b>CLASS:</b> CStack
    /// 
    /// Is essentially a RPN-calculator with four registers X, Y, Z, T
    /// like the HP RPN calculators. Numeric values are entered in the entry
    /// string by adding digits and one comma. For test purposes the method
    /// RollSetX can be used instead. Operations can be performed on the
    /// calculator preferrably by using one of the methods
    /// <list type="number">
    ///   <item><b>BinOp</b> - merges X and Y into X via an operation and rolls
    ///         down the stack</item>
    ///   <item><b>Unop</b> - operates X and puts the result in X with
    ///         overwrite</item>
    ///   <item><b>Nilop</b> - adds a known constant on the stack and rolls up
    ///         the stack</item>
    /// </list>
    /// </summary>
    public class CStack
    {
        /// <summary>
        /// The variable slots in the stack
        /// </summary>
        public double X, Y, Z, T;
        /// <summary>
        /// The entry field for adding up to a string that is converted
        /// to a double and pushed into X when key [Enter] is pressed
        /// </summary>
        public string entry;
        /// <summary>
        /// <b>Constructor:</b> CStack, no parameters, create a new stack and
        /// init X, Y, Z, T to zero and clears the text entry
        /// </summary>
        public CStack()
        {
            X = Y = Z = T = 0;
            entry = "";
        }
        /// <summary>
        /// <b>StackString</b> construct a string to write out in a stack view
        /// </summary>
        /// <returns>the string containing the values T, Z, Y, X with newlines 
        /// between them</returns>
        public string StackString()
        {
            return $"{T}\n{Z}\n{Y}\n{X}\n{entry}";
        }
        /// <summary>
        /// Setter for X
        /// </summary>
        /// <param name="newX"></param>
        public void SetX(double newX)
        {
            X = newX;
        }
        /// <summary>
        /// EntryAddNum: Add a digit to the entry field
        /// </summary>
        /// <param name="digit">string containing one digit "0" ... "9"</param>
        /// <remarks><b>NB:</b>if the string digit does not contain a parseable 
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
        /// EntryAddComma: adds a comma to the entry string
        /// </summary>
        /// <remarks>
        /// <b>NB:</b> if the entry string already contains a comma, nothing is 
        /// added to the entry
        /// </remarks>
        public void EntryAddComma()
        {
            if (entry.IndexOf(",") == -1)
                entry = entry + ",";
        }
        /// <summary>
        /// EntryChangeSign: changes the sign of the entry string
        /// </summary>
        /// <remarks>
        /// <b>NB:</b> if the first char is already a '-' it is exchanged for a '+',
		/// if it is a '+' it is changed to a '-', otherwise a '-' is just added
        /// first
        /// </remarks>
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
        /// Enter: converts the entry to a double and puts it into X
        /// </summary>
        /// <remarks>
        /// <b>NB:</b> the entry is cleared after a successful operation
        /// </remarks>
        public void Enter()
        {
            if (entry != "")
            {
                RollSetX(double.Parse(entry));
                entry = "";
            }
        }
        /// <summary>
        /// Drop: drops the value of X, and rolls down
        /// </summary>
        /// <remarks>
        /// <b>NB:</b> Z gets the value of T
        /// </remarks>
        public void Drop()
        {
            X = Y; Y = Z; Z = T;
        }
        /// <summary>
        /// DropSetX: replaces the value of X, and rolls down
        /// </summary>
		/// <param name="newX">the new value to assign to X</param>
        /// <remarks>
        /// <b>NB:</b> this is used when applying binary operations consuming
		/// X and Y and putting the result in X, while rolling down the stack
        /// </remarks>
        public void DropSetX(double newX)
        {
            X = newX; Y = Z; Z = T;
        }
        /// <summary>
        /// BinOp: evaluates a binary operation
        /// </summary>
		/// <param name="op">the binary operation retrieved from the GUI buttons</param>
        /// <remarks>
        /// <b>NB:</b> the stack is rolled down
        /// </remarks>
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
        /// Unop: evaluates a unary operation
        /// </summary>
		/// <param name="op">the unary operation retrieved from the GUI buttons</param>
        /// <remarks>
        /// <b>NB:</b> the stack is not moved, X is replaced by the result of the operation
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
        /// Nilop: evaluates a "nilary operation" (insertion of a constant)
        /// </summary>
		/// <param name="op">the nilary operation (name of the constant)
		/// retrieved from the GUI buttons</param>
        /// <remarks>
        /// <b>NB:</b> the stack is rolled up, X is preserved in Y that is
		/// preserved in Z that is preserved in T, T is erased
        /// </remarks>
        public void Nilop(string op)
        {
            switch (op)
            {
                case "π": RollSetX(Math.PI); break;
                case "e": RollSetX(Math.E); break;
            }
        }
        /// <summary>
        /// Roll: rolls the stack up
        /// </summary>
        public void Roll()
        {
            double tmp = T;
            T = Z; Z = Y; Y = X; X = tmp;
        }
        /// <summary>
        /// Roll: rolls the stack up and puts a new value in X
        /// </summary>
		/// <param name="newX">the new value to put into X</param>
        /// <remarks>
        /// <b>NB:</b> T is dropped
        /// </remarks>
        public void RollSetX(double newX)
        {
            T = Z; Z = Y; Y = X; X = newX;
        }
        public void Setvar(string op)
        {
            /// \todo NYI!
        }
        public void GetVar(string op)
        {
            /// \todo NYI!
        }
    }
}
