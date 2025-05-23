/* To-Do
    tracking/counting opening and closing parenthesis
*/
using System.Text.RegularExpressions;
using System.Globalization;


namespace Calculator;

public static class Program
{
    public class NewLoopException(string message) : Exception(message);

    public class ResultErrorException(string message) : Exception(message);
    public static string GetEquation()
    {
        var regexItem = new Regex(@"^[0-9()\+\-\*\/\^\.]+$");
        Console.Write("Equation: ");
        string userInput = Console.ReadLine() ?? "";
        userInput = userInput.Replace(" ", "").Replace("x", "*").Replace(" ", "").Replace("\t", "").Replace("\r", "");
        
        if (!regexItem.IsMatch(userInput))
        {
            throw new NewLoopException("Invalid input: Please use only numbers (0-9), parenthesis and the operators: addition (+), subtraction (-), multiplication (* or x), and division (/)");
        }

        userInput = NormalizeParenthesis(userInput);
        return userInput;
    }

    static string NormalizeParenthesis(string input)
    {
        return input
            .Replace("}", ")")
            .Replace("]", ")")
            .Replace("{", "(")
            .Replace("[", "(");
    }

    static bool IsOperator(string input)
    {
        var regexObject = new Regex(@"^[\+\-\*\/\^]+$");
        return regexObject.IsMatch(input);
    }
    
    static bool IsParenthesis(string input)
    {
        var regexObject = new Regex(@"^[()]+$");
        return regexObject.IsMatch(input);
    }
    
    static bool ContainsDecimal(string input)
    {
        return input.Contains('.');
    }
    
    static bool IsNumber(string input)
    {
        var regexObject = new Regex(@"^-?[0-9]*\.?[0-9]+$");
        return regexObject.IsMatch(input);
    }
    
    static bool IsDoubleAsterisk(string equation, int i)
    {
        return equation[i] == '*' &&
               i + 1 < equation.Length && equation[i + 1] == '*' &&
               (i == 0 || equation[i - 1] != '*') && // ensure it doesn't follow another '*'
               (i + 2 >= equation.Length || equation[i + 2] != '*'); // ensure it’s not followed by another '*'
    }



    
    public static string[] Tokenize(string equation)
    {
        // doing manual checking as it's easier than rewriting the logic for an edge case
        // (translation: don’t give a shit)
        if (equation.Contains("()"))
        {
            throw new NewLoopException("Invalid Input: Parenthesis is Empty");
        }
        
        if (equation.Contains("---"))
        {
            throw new NewLoopException("Invalid Input: Two operators cannot appear consecutively");
        }
        
        List<string> output = new List<string>();
        string numberHoldingStack = "";
        char lastChar = ' ';
        int balance = 0;
        for (int i = 0; i < equation.Length; i++)
        {
            char currentChar = equation[i];
            
            if (currentChar == '(') balance++;
            if (currentChar == ')')
            {
                balance--;
                if (balance < 0)
                {
                    throw new NewLoopException("Invalid Input: Misordered closing parenthesis.");
                }
            }

            if (IsNumber(currentChar.ToString()))
            {
                numberHoldingStack += currentChar;
            }

            if (currentChar == '.')
            {
                if (!ContainsDecimal(numberHoldingStack))
                {
                    numberHoldingStack += currentChar;
                }
                else
                {
                    throw new NewLoopException("Invalid Input: multiple decimal points.");
                }
            }
            
            if (IsOperator(currentChar.ToString()) || IsParenthesis(currentChar.ToString()) || i == equation.Length - 1)
            {
                if (numberHoldingStack.Length > 0)
                {
                    if (numberHoldingStack == ".")
                        throw new NewLoopException("Invalid Input: lone decimal point.");
                    if (numberHoldingStack[0] == '.')
                        numberHoldingStack = "0" + numberHoldingStack;
                    else if (numberHoldingStack[^1] == '.')
                        numberHoldingStack = numberHoldingStack.Replace(".", "");

                    output.Add(numberHoldingStack);
                    numberHoldingStack = "";
                }
            }

            if (IsOperator(currentChar.ToString()))
            {
                if ((i == 0 && currentChar != '-')|| i == equation.Length - 1)
                {
                    throw new NewLoopException("Invalid Input: Operators cannot be on first or last character");
                }
                else if (IsDoubleAsterisk(equation, i))
                {
                    output.Add("^");
                    i++; // Skip the second '*'
                }
                else if (currentChar == '-' && ((IsOperator(lastChar.ToString()) || IsParenthesis(lastChar.ToString())) || i == 0))
                {
                    numberHoldingStack += currentChar;
                }
                else if (!IsOperator(lastChar.ToString()) && lastChar != '(')
                {
                    output.Add(currentChar.ToString());
                }
                else
                {
                    throw new NewLoopException("Invalid Input: Two operators cannot appear consecutively");
                }
            }

            if (IsParenthesis(currentChar.ToString()))
            {
                output.Add(currentChar.ToString());
            }

            lastChar = currentChar;
        }

        if (balance != 0)
        {
            throw new NewLoopException("Invalid Input: Unbalanced parentheses.");
        }
        
        return output.ToArray();
    }
    
    static bool IsLeftAssociative(string op) {
        return op != "^";
    }
    
    static int GetPriority(string op)
    {
        return op switch
        {
            "-" => 1,
            "+" => 1,
            "/" => 2,
            "*" => 2,
            "^" => 3,
            _ => throw new ArgumentException($"Invalid operator: {op}")
        };
    }

    public static string[] InfixToPostfix(string[] infix)
    {
        List<string> output = new List<string>();
        List<string> holdingStack = new List<string>();
        for (int i = 0; i < infix.Length; i++)
        {
            string topOfStackToken = (holdingStack.Count > 0) ? holdingStack[^1] : "";
            string currentToken = infix[i];
            if (IsNumber(currentToken))
            {
                output.Add(currentToken);
            }

            else if (currentToken == "(")
            {
                holdingStack.Add(currentToken);
            }
            else if (IsOperator(currentToken)) {
                while (IsOperator(topOfStackToken) && 
                       (GetPriority(topOfStackToken) > GetPriority(currentToken) ||
                        (GetPriority(topOfStackToken) == GetPriority(currentToken) && IsLeftAssociative(currentToken)))) {
                    output.Add(topOfStackToken);
                    holdingStack.RemoveAt(holdingStack.Count - 1);
                    topOfStackToken = (holdingStack.Count > 0) ? holdingStack[^1] : "";
                }
                holdingStack.Add(currentToken);
            }
            else if (currentToken == ")") {
                while (topOfStackToken != "(") {
                    output.Add(topOfStackToken);
                    holdingStack.RemoveAt(holdingStack.Count - 1);
                    topOfStackToken = (holdingStack.Count > 0) ? holdingStack[^1] : "";
                }
                holdingStack.RemoveAt(holdingStack.Count - 1);
            }
        }

        foreach (string token in holdingStack.AsEnumerable().Reverse()){
                output.Add(token);
        }
        return output.ToArray();
    }

    public static double EvaluatePostfix(string[] postfix)
    {
        List<string> output = new List<string>();

        for (int i = 0; i < postfix.Length; i++)
        {
            string currentToken = postfix[i];
            if (IsNumber(currentToken))
            {
                output.Add(currentToken);
            }
            else if (IsOperator(currentToken))
            {
                double left = double.Parse(output[^2], CultureInfo.InvariantCulture);
                double right = double.Parse(output[^1], CultureInfo.InvariantCulture);

                if (currentToken == "/" && right == 0)
                {
                    throw new ResultErrorException("Error: Cannot Divide By Zero");
                }
                
                double result = currentToken switch
                {
                    "+" => left + right,
                    "-" => left - right,
                    "*" => left * right,
                    "/" => left / right,
                    "^" => Math.Pow(left, right),
                    _   => throw new InvalidOperationException($"Unsupported operator: {currentToken}")
                };
                

                string evaluation = result.ToString(CultureInfo.InvariantCulture);

                output.RemoveRange(output.Count - 2, 2);
                output.Add(evaluation);
            }
        }

        return double.Parse(output[0]);
    }
    
    static void Main()
    {
        while (true)
        {
            try
            {
                string equation = GetEquation();
                string[] tokens = Tokenize(equation);
                string[] postfix = InfixToPostfix(tokens);
                double result = EvaluatePostfix(postfix);

                // temporary debugger to print token list like python
                Console.WriteLine("Tokens: " + '\"' + String.Join("\", \"", tokens) + '\"');
                Console.WriteLine("Postfix: " + '\"' + String.Join("\", \"", postfix) + '\"');
                Console.WriteLine("Result: " + result);
            }
            catch (NewLoopException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ResultErrorException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
    }
}