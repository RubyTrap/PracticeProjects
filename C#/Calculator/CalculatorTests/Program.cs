using Xunit;
using Calculator;

namespace CalculatorTests
{

    public class CalculatorTests
    {
        public static readonly Func<string, string[]> Tokenize = Calculator.Program.Tokenize;

        [Fact]
        public void SimpleAddition()
        {
            Assert.Equal(new[] { "2", "+", "2" }, Tokenize("2+2"));
        }

        [Fact]
        public void MultiDigitNumbers()
        {
            Assert.Equal(new[] { "12", "+", "345" }, Tokenize("12+345"));
        }

        [Fact]
        public void DecimalNumbers()
        {
            Assert.Equal(new[] { "3.14", "+", "2.0" }, Tokenize("3.14+2.0"));
        }

        [Fact]
        public void Parentheses()
        {
            Assert.Equal(new[] { "(", "1", "+", "2", ")", "*", "3" }, Tokenize("(1+2)*3"));
        }

        [Fact]
        public void XAsMultiplication()
        {
            Assert.Equal(new[] { "2", "*", "3" }, Tokenize("2x3"));
        }
        
        [Fact]
        public void DoubleAsteriskExponentiation()
        {
            Assert.Equal(new[] { "2", "^", "3" }, Tokenize("2**3"));
        }
        
        [Fact]
        public void MultipleOperatorsThrows()
        {
            Assert.Throws<Calculator.Program.NewLoopException>(() => Tokenize("5++6"));
            Assert.Throws<Calculator.Program.NewLoopException>(() => Tokenize("5+*6"));
        }
        
        [Fact]
        public void TripleAsteriskThrows()
        {
            Assert.Throws<Calculator.Program.NewLoopException>(() => Tokenize("5***8"));
        }
        
        [Fact]
        public void NumberEndingWithDecimal_Throws()
        {
            Assert.Throws<Calculator.Program.NewLoopException>(() => Tokenize("5."));
        }
        
        [Fact]
        public void NumberStartingWithDecimal()
        {
            Assert.Equal(new[] {"0.5"} ,Tokenize(".5"));
        }

        [Fact]
        public void OperatorAtStart()
        {
            Assert.Throws<Calculator.Program.NewLoopException>(() => Tokenize("+4"));
            Assert.Throws<Calculator.Program.NewLoopException>(() => Tokenize("*4"));
        }

        [Fact]
        public void OperatorAtEnd()
        {
            Assert.Throws<Calculator.Program.NewLoopException>(() => Tokenize("4+"));
            Assert.Throws<Calculator.Program.NewLoopException>(() => Tokenize("4*"));
        }

        [Fact]
        public void OnlyNegativeNumber()
        {
            Assert.Equal(new[] {"-3"}, Tokenize("-3"));
        }

        [Fact]
        public void NegativeOperation()
        {
            Assert.Equal(new[] {"4", "*", "-3"}, Tokenize("4*-3"));
        }

        [Fact]
        public void ComplexExpression()
        {
            Assert.Equal(
                new[] { "(", "(", "12.5", "+", "3", ")", "*", "2", ")", "-", "(", "4", "/", "2", "^", "2", ")" },
                Tokenize("((12.5+3)*2)-(4/2^2)")
            );
        }

        [Fact]
        public void EmptyInput()
        {
            Assert.Equal(Array.Empty<string>(), Tokenize(""));
        }

        [Fact]
        public void OnlyOperatorInput()
        {
            Assert.Equal(new[] { "+" }, Tokenize("+"));
        }
        
        [Fact]
        public void MultipleDecimals_Throws()
        {
            Assert.Throws<Calculator.Program.NewLoopException>(() => Tokenize("3.1.4"));
        }
    }
}