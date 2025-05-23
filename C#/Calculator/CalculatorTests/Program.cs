using Xunit;
using Calculator;
using System.IO;

namespace CalculatorTests
{
    public class GetEquationTests
    {
        public static readonly Func<string> GetEquation = Program.GetEquation;

        [Fact]
        public void AcceptsValidEquation()
        {
            Console.SetIn(new StringReader("3+4*2"));
            Assert.Equal("3+4*2", GetEquation());
        }

        [Fact]
        public void ReplacesXWithMultiplication()
        {
            Console.SetIn(new StringReader("3x2"));
            Assert.Equal("3*2", GetEquation());
        }

        [Fact]
        public void StripsSpaces()
        {
            Console.SetIn(new StringReader(" 3 + 2 "));
            Assert.Equal("3+2", GetEquation());
        }

        [Fact]
        public void ThrowsOnInvalidCharacters()
        {
            Console.SetIn(new StringReader("2+2a"));
            Assert.Throws<Program.NewLoopException>(() => GetEquation());
        }

        [Fact]
        public void ThrowsOnLettersOnly()
        {
            Console.SetIn(new StringReader("abc"));
            Assert.Throws<Program.NewLoopException>(() => GetEquation());
        }

        [Fact]
        public void AllowsParenthesesAndOperators()
        {
            Console.SetIn(new StringReader("(3+4)*2"));
            Assert.Equal("(3+4)*2", GetEquation());
        }

        [Fact]
        public void AllowsDecimals()
        {
            Console.SetIn(new StringReader("3.14+2.0"));
            Assert.Equal("3.14+2.0", GetEquation());
        }
        
        [Fact]
        public void ThrowsOnControlCharacters()
        {
            Console.SetIn(new StringReader("3+4\u0007"));  // Bell char in input
            Assert.Throws<Program.NewLoopException>(() => GetEquation());
        }

        [Fact]
        public void ThrowsOnSpecialSymbols()
        {
            Console.SetIn(new StringReader("3+4$2"));
            Assert.Throws<Program.NewLoopException>(() => GetEquation());
        }

        [Fact]
        public void ThrowsOnOnlyWhitespaceInput()
        {
            Console.SetIn(new StringReader("    "));
            Assert.Throws<Program.NewLoopException>(() => GetEquation());
        }

        [Fact]
        public void ThrowsOnEmptyInput()
        {
            Console.SetIn(new StringReader(""));
            Assert.Throws<Program.NewLoopException>(() => GetEquation());
        }

        [Fact]
        public void ThrowsOnMixedInvalidInput()
        {
            Console.SetIn(new StringReader("3 + 4 * a$"));
            Assert.Throws<Program.NewLoopException>(() => GetEquation());
        }

        [Fact]
        public void AcceptsLongValidInput()
        {
            string longInput = new string('1', 1000) + "+" + new string('2', 1000);
            Console.SetIn(new StringReader(longInput));
            Assert.Equal(longInput, GetEquation());
        }

        [Fact]
        public void StripsTabsAndNewlines()
        {
            Console.SetIn(new StringReader("3\t+4"));
            Assert.Equal("3+4", GetEquation());
        }

        [Fact]
        public void ThrowsOnEmojiInput()
        {
            Console.SetIn(new StringReader("3+4😊"));
            Assert.Throws<Program.NewLoopException>(() => GetEquation());
        }
    }

    public class TokenizeTests
    {
        public static readonly Func<string, string[]> Tokenize = Program.Tokenize;

        [Fact]
        public void SimpleAddition() => Assert.Equal(new[] { "2", "+", "2" }, Tokenize("2+2"));

        [Fact]
        public void MultiDigitNumbers() => Assert.Equal(new[] { "12", "+", "345" }, Tokenize("12+345"));

        [Fact]
        public void DecimalNumbers() => Assert.Equal(new[] { "3.14", "+", "2.0" }, Tokenize("3.14+2.0"));

        [Fact]
        public void Parentheses() => Assert.Equal(new[] { "(", "1", "+", "2", ")", "*", "3" }, Tokenize("(1+2)*3"));

        [Fact]
        public void DoubleAsteriskExponentiation() => Assert.Equal(new[] { "2", "^", "3" }, Tokenize("2**3"));

        [Fact]
        public void MultipleOperatorsThrows()
        {
            Assert.Throws<Program.NewLoopException>(() => Tokenize("5++6"));
            Assert.Throws<Program.NewLoopException>(() => Tokenize("5+*6"));
        }

        [Fact]
        public void TripleAsteriskThrows() => Assert.Throws<Program.NewLoopException>(() => Tokenize("5***8"));

        [Fact]
        public void NumberEndingWithDecimal() => Assert.Equal(new[] { "5" }, Tokenize("5."));

        [Fact]
        public void NumberStartingWithDecimal() => Assert.Equal(new[] { "0.5" }, Tokenize(".5"));

        [Fact]
        public void OperatorAtStart()
        {
            Assert.Throws<Program.NewLoopException>(() => Tokenize("+4"));
            Assert.Throws<Program.NewLoopException>(() => Tokenize("*4"));
        }

        [Fact]
        public void OperatorAtEnd()
        {
            Assert.Throws<Program.NewLoopException>(() => Tokenize("4+"));
            Assert.Throws<Program.NewLoopException>(() => Tokenize("4*"));
        }

        [Fact]
        public void OnlyNegativeNumber() => Assert.Equal(new[] { "-3" }, Tokenize("-3"));

        [Fact]
        public void NegativeOperation() => Assert.Equal(new[] { "4", "*", "-3" }, Tokenize("4*-3"));

        [Fact]
        public void ComplexExpression()
        {
            Assert.Equal(
                new[] { "(", "(", "12.5", "+", "3", ")", "*", "2", ")", "-", "(", "4", "/", "2", "^", "2", ")" },
                Tokenize("((12.5+3)*2)-(4/2^2)")
            );
        }

        [Fact]
        public void EmptyInput() => Assert.Equal(Array.Empty<string>(), Tokenize(""));

        [Fact]
        public void OnlyOperatorInput() => Assert.Throws<Program.NewLoopException>(() => Tokenize("+"));

        [Fact]
        public void MultipleDecimals_Throws() => Assert.Throws<Program.NewLoopException>(() => Tokenize("3.1.4"));
        
        [Fact]
        public void ThrowsOnUnbalancedParentheses()
        {
            Assert.Throws<Program.NewLoopException>(() => Tokenize("(3+2"));
            Assert.Throws<Program.NewLoopException>(() => Tokenize("3+2)"));
            Assert.Throws<Program.NewLoopException>(() => Tokenize(")3+2("));
        }

        [Fact]
        public void ThrowsOnConsecutiveOperators()
        {
            Assert.Throws<Program.NewLoopException>(() => Tokenize("5++6"));
            Assert.Throws<Program.NewLoopException>(() => Tokenize("5//6"));
            Assert.Throws<Program.NewLoopException>(() => Tokenize("5---6"));
        }

        [Fact]
        public void ThrowsOnMultipleDecimalPointsInNumber()
        {
            Assert.Throws<Program.NewLoopException>(() => Tokenize("3.14.15+2"));
        }

        [Fact]
        public void AllowsLeadingZeroInDecimal()
        {
            Assert.Equal(new[] { "0.5", "+", "2" }, Tokenize("0.5+2"));
        }

        [Fact]
        public void AllowsTrailingDecimalPoint()
        {
            var tokens = Tokenize("3.+2");
            Assert.Equal(new[] { "3", "+", "2" }, tokens);
        }


        [Fact]
        public void ThrowsOnEmptyParentheses()
        {
            Assert.Throws<Program.NewLoopException>(() => Tokenize("3+()2"));
        }

        [Fact]
        public void ThrowsOnOperatorAtStartOrEnd()
        {
            Assert.Throws<Program.NewLoopException>(() => Tokenize("+3+2"));
            Assert.Throws<Program.NewLoopException>(() => Tokenize("3+2-"));
        }

        [Fact]
        public void HandlesWhitespaceInInput()
        {
            Assert.Equal(new[] { "3", "+", "4" }, Tokenize(" 3 + 4 "));
        }

        [Fact]
        public void ThrowsOnInvalidExponentSyntax()
        {
            Assert.Throws<Program.NewLoopException>(() => Tokenize("2***3"));
            Assert.Throws<Program.NewLoopException>(() => Tokenize("2****3"));
        }

        [Fact]
        public void HandlesNegativeNumbersCorrectly()
        {
            Assert.Equal(new[] { "-3", "+", "5" }, Tokenize("-3+5"));
            Assert.Equal(new[] { "4", "*", "-2" }, Tokenize("4*-2"));
        }

        [Fact]
        public void AllowsMinusNegativeNumber()
        {
            Assert.Equal(new[] { "4", "-", "-2" }, Tokenize("4--2"));
        }

        
        [Fact]
        public void AllowsBalancedParentheses()
        {
            Assert.Equal(
                new[] { "(", "3", "+", "4", ")", "*", "(", "2", "-", "1", ")" },
                Tokenize("(3+4)*(2-1)")
            );
        }

        [Fact]
        public void ThrowsOnUnbalancedParentheses_MissingClosing()
        {
            Assert.Throws<Program.NewLoopException>(() => Tokenize("(3+4*2"));
        }

        [Fact]
        public void ThrowsOnUnbalancedParentheses_MissingOpening()
        {
            Assert.Throws<Program.NewLoopException>(() => Tokenize("3+4)*2"));
        }

        [Fact]
        public void ThrowsOnUnbalancedParentheses_ExtraClosing()
        {
            Assert.Throws<Program.NewLoopException>(() => Tokenize("3+(4*2))"));
        }

        [Fact]
        public void ThrowsOnUnbalancedParentheses_EmptyParentheses()
        {
            Assert.Throws<Program.NewLoopException>(() => Tokenize("3+()4"));
        }

    }

    public class InfixToPostfixTests
    {
        public static readonly Func<string[], string[]> InfixToPostfix = Program.InfixToPostfix;
        public static readonly Func<string, string[]> Tokenize = Program.Tokenize;
        
        [Fact]
        public void SimpleAddition()
        {
            string[] result = InfixToPostfix(Tokenize("5+2"));
            Assert.Equal("52+", string.Join("", result));
        }

        [Fact]
        public void AdditionAndMultiplication()
        {
            string[] result = InfixToPostfix(Tokenize("5+2*3"));
            Assert.Equal("523*+", string.Join("", result));
        }

        [Fact]
        public void ParenthesesAffectPrecedence()
        {
            string[] result = InfixToPostfix(Tokenize("(5+2)*3"));
            Assert.Equal("52+3*", string.Join("", result));
        }

        [Fact]
        public void RightAssociativeExponent()
        {
            string[] result = InfixToPostfix(Tokenize("2^3^4"));
            Assert.Equal("234^^", string.Join("", result));
        }
        
        [Fact]
        public void HandlesNegativeNumbers()
        {
            string[] result = InfixToPostfix(Tokenize("-5+3"));
            Assert.Equal(new[] { "-5", "3", "+" }, result);
        }

        [Fact]
        public void HandlesMultipleDigitNumbers()
        {
            string[] result = InfixToPostfix(Tokenize("12+34"));
            Assert.Equal(new[] { "12", "34", "+" }, result);
        }

        [Fact]
        public void HandlesWhitespace()
        {
            string[] result = InfixToPostfix(Tokenize(" 7 +   8 "));
            Assert.Equal(new[] { "7", "8", "+" }, result);
        }

        [Fact]
        public void EmptyInputReturnsEmpty()
        {
            string[] result = InfixToPostfix(Tokenize(""));
            Assert.Empty(result);
        }

        [Fact]
        public void EmptyParenthesisThrows()
        {
            Assert.Throws<Program.NewLoopException>(() => InfixToPostfix(Tokenize("((()))")));
        }

        [Fact]
        public void ComplexExpression()
        {
            string[] result = InfixToPostfix(Tokenize("3+12*(2-4)^2+(-6)"));
            Assert.Equal(new[] { "3", "12", "2", "4", "-", "2", "^", "*", "+", "-6", "+" }, result);
        }
    }

    public class EvaluatePostfixTests
    {
        public static readonly Func<string[], double> EvaluatePostfix = Program.EvaluatePostfix;
        public static readonly Func<string[], string[]> InfixToPostfix = Program.InfixToPostfix;
        public static readonly Func<string, string[]> Tokenize = Program.Tokenize;
        
        [Fact]
        public void HandlesSimpleAddition()
        {
            double expected = 7;
            double result = EvaluatePostfix(InfixToPostfix(Tokenize("3+4")));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void HandlesSimpleSubtraction()
        {
            double expected = -1;
            double result = EvaluatePostfix(InfixToPostfix(Tokenize("3-4")));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void HandlesSimpleMultiplication()
        {
            double expected = 12;
            double result = EvaluatePostfix(InfixToPostfix(Tokenize("3*4")));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void HandlesSimpleDivision()
        {
            double expected = 3;
            double result = EvaluatePostfix(InfixToPostfix(Tokenize("12/4")));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void HandlesMultipleOperators()
        {
            double expected = 14;
            double result = EvaluatePostfix(InfixToPostfix(Tokenize("2+3*4")));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void HandlesParentheses()
        {
            double expected = 20;
            double result = EvaluatePostfix(InfixToPostfix(Tokenize("(2+3)*4")));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void HandlesDecimalNumbers()
        {
            double expected = 7.5;
            double result = EvaluatePostfix(InfixToPostfix(Tokenize("3.5+4")));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void HandlesNegativeNumbers()
        {
            double expected = -1;
            double result = EvaluatePostfix(InfixToPostfix(Tokenize("-3+2")));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void HandlesComplexExpression()
        {
            double expected = 7;
            double result = EvaluatePostfix(InfixToPostfix(Tokenize("3+4*(2-1)")));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void HandlesExponentiation()
        {
            double expected = 81;
            double result = EvaluatePostfix(InfixToPostfix(Tokenize("3^4")));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void HandlesMultipleDigitNumbers()
        {
            double expected = 46;
            double result = EvaluatePostfix(InfixToPostfix(Tokenize("12+34")));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void HandlesDivisionByZero()
        {
            // Depending on implementation, might throw or return Infinity. 
            // This test assumes throwing an exception.
            Assert.Throws<Program.ResultErrorException>(() =>
            {
                EvaluatePostfix(InfixToPostfix(Tokenize("10/0")));
            });
        }

        [Fact]
        public void HandlesWhitespaceInExpression()
        {
            double expected = 7;
            double result = EvaluatePostfix(InfixToPostfix(Tokenize(" 3 + 4 ")));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void HandlesSingleNumber()
        {
            double expected = 42;
            double result = EvaluatePostfix(InfixToPostfix(Tokenize("42")));
            Assert.Equal(expected, result);
        }
    }
}
