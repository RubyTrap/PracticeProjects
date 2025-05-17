/* To-Do
    tracking/counting opening and closing parenthesis
*/
using System.Text.RegularExpressions;


namespace Calculator;

public class Program
{
    public class NewLoopException : Exception
    // no idea how classes work but just copied a basic exception class for error handling
    { 
        public NewLoopException(string message) : base(message) { }
    }
    static string GetEquation()
    {
    var regexItem = new Regex(@"^[0-9()\+\-\*x\/\^\.]+$");
        Console.Write("Equation: ");
        string userInput = Console.ReadLine() ?? "";
        userInput = userInput.Replace(" ", "");
        if (!regexItem.IsMatch(userInput))
        {
            throw new NewLoopException("Invalid input: Please use only numbers (0-9), parenthesis and the operators: addition (+), subtraction (-), multiplication (* or x), and division (/)");
        }

        return userInput;
    }

    public static bool IsOperator(char input)
    {
        var regexObject = new Regex(@"^[()\+\-\*\/\^]+$");
        return regexObject.IsMatch(input.ToString());
    }
    
    public static bool IsNumberOrPeriod(char input)
    {
        var regexObject = new Regex(@"^[0-9\.]+$");
        return regexObject.IsMatch(input.ToString());
    }
    
    public static string[] Tokenize(string equation)
    {
        int i = 0;
        char lastParsed = ' ';
        List<string> output = new List<string>();
        string numberHoldingStack = "";
        char currentChar;
        while (i < equation.Length)
        {
            currentChar = equation[i];
            
            // NUMBER LOGIC
            if (Char.IsNumber(currentChar) || currentChar == '.')
            // always add numbers and periods to holding stack
            {
                numberHoldingStack += currentChar;
            }
            if ((!IsNumberOrPeriod(currentChar) && IsNumberOrPeriod(lastParsed) || (i == equation.Length - 1 )) && numberHoldingStack.Length > 0)
            // if char isn't number or period but previously parsed number is a number or period then tokenize number holding stack if it exists
            {
                if (currentChar == '.')
                // if number added to stack ends with . throw err
                {
                    // exception differentiation for debugging reasons
                    throw new NewLoopException("Invalid input: A number cannot end with a decimal point.");
                }

                if (numberHoldingStack.Length - numberHoldingStack.Replace(".", "").Length > 1)
                // if number holding stack has more than 1 period throw exception
                {
                    throw new NewLoopException("Invalid input: A number cannot contain more than one decimal point.");
                }

                if (numberHoldingStack[0] == '.')
                // if first character of number holding stack is a period then add a 0 to the start
                {
                    numberHoldingStack = "0" + numberHoldingStack;
                }
                output.Add(numberHoldingStack);
                numberHoldingStack = "";
            }
            
            // OPERATOR LOGIC
            if (currentChar == 'x')
            // parse x as *
            {
                output.Add("*");
            }
            else if (IsOperator(currentChar) && !IsOperator(lastParsed) && i != equation.Length - 1 && i != 0)
            // add operators to stack if last character wasnt an operator and not the first or last iteration
            {
                output.Add(currentChar.ToString());
            }
            else if (IsOperator(currentChar) && IsOperator(lastParsed) && i != equation.Length - 1 && i != 0)
            // if current character and last character is an operator and its not the first or last iteration
            {
                if (currentChar == '*' && lastParsed == '*')
                // if current and last char is * then parse ^
                {
                    output.RemoveAt(output.Count - 1);
                    output.Add("^");
                    currentChar = '^';
                }
                else if (currentChar == '-' && IsOperator(lastParsed))
                // if current char is - and last char is an operator then treat as unary
                {
                    numberHoldingStack += "-";
                }
                else
                {
                    throw new NewLoopException("Invalid input: Two or more operators cannot appear consecutively.");
                }
            }
            else if (IsOperator(currentChar) && (i == 0 || i == equation.Length -1))
            // if operator occurs at beginning or end throw
            {
                if (currentChar == '-' && i == 0)
                // allows standalone negative numbers
                {
                    numberHoldingStack += "-";
                }
                else
                {
                    throw new NewLoopException("Invalid input: An operator cannot appear at the beginning or end of an expression.");
                }
            }
            lastParsed = currentChar;
            i++;
        }

        return output.ToArray();
    }
    static void Main()
    {
        while (true)
        {
            try
            {
                string equation = GetEquation();
                string[] tokens = Tokenize(equation);
                // temporary debugger to print token list like python
                Console.WriteLine('\"' + String.Join("\", \"", tokens) + '\"');
            }
            catch(NewLoopException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
    }
}