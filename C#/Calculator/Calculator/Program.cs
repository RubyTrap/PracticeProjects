using System.Text.RegularExpressions;

namespace Calculator;

class Program
{
    static string GetEquation()
    {
        var regexItem = new Regex(@"^[0-9()\+\-\*x\/\^\.]*$");
        while (true)
        {
            Console.Write("Equation: ");
            string userInput = Console.ReadLine() ?? "";
            userInput = userInput.Replace(" ", "");
            if (!regexItem.IsMatch(userInput))
            {
                Console.WriteLine("Invalid input. Please use only numbers (0-9), parenthesis and the operators: addition (+), subtraction (-), multiplication (* or x), and division (/)");
                continue;
            }

            return userInput;
        }
    }

    static string Tokenize(string equation)
    {
        int i = 0;
        char lastParsed = ' ';
        List<string> output = new List<string>();
        string numberHoldingStack = "";
        while (i < equation.Length)
        {
            if (Char.IsNumber(equation[i]) || equation[i] == '.' )
            {
                numberHoldingStack += equation[i];
            }
            else if (Char.IsNumber(lastParsed))
            // if char isn't number or period but previously parsed number is a number then tokenize number holding stack
            {
                output.Add(numberHoldingStack);
                numberHoldingStack = "";
            }
            lastParsed = equation[i];
            i++;
        }

        return "";
    }
    static void Main()
    {
        string equation = GetEquation();
        Tokenize(equation);
    }
}