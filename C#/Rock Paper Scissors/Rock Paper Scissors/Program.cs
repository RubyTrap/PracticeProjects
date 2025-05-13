using System.Runtime.InteropServices;

namespace Rock_Paper_Scissors;

class Program
{
    static string GetAiHand()
    {
        Random rnd = new Random();
        int rndInt = rnd.Next(1, 4);
        switch (rndInt)
        {
            case 1:
                return "rock";
            case 2:
                return "paper";
            case 3:
                return "scissors";
            default:
                throw new Exception("Computer never assigned variable hand");
        }
    }
    
    static string GetPlayerHand()
    {
        while (true)
        {
            Console.Write("Enter ROCK, PAPER, or SCISSORS: ");
            string plrInput = Console.ReadLine()!;
            switch (plrInput.ToLower().Trim())
            {
                case "rock":
                case "r":
                    return "rock";
                case "paper":
                case "p":
                    return "paper";
                case "scissors":
                case "s":
                    return "scissors";
                default:
                    Console.WriteLine("Invalid Input");
                    break;
            }
        }
    }

    static void CheckWinState((string player, string computer) hands)
    {
        if (hands.player == hands.computer)
        {
            Console.WriteLine("It's a draw");
        }
        else if (hands.player == "rock" && hands.computer == "scissors" ||
                 hands.player == "paper" && hands.computer == "rock" ||
                 hands.player == "scissors" && hands.computer == "paper")
        {
            Console.WriteLine("You win");
        }
        else
        {
            Console.WriteLine("You lose");
        }
        
    }

    static Boolean InputPlayAgain()
    {
        while (true)
        {
            Console.Write("Would you like to play again (Y/N): ");
            string? plrInput = Console.ReadLine();
            if (plrInput == null)
            {
                throw new Exception("InputPlayAgain plrInput variable returned null");
            }
            switch (plrInput.ToLower().Trim())
            {
                case "y":
                case"yes":
                    return true;
                case "n":
                case "no":
                    Console.Write("thanks for playing");
                    return false;
            }

            Console.WriteLine("Please enter valid input n/y or no/yes");
        }
    }
    
    static void Main()
    {
        do
        {
            var aiHand = GetAiHand();
            var playerHand = GetPlayerHand();
            Console.WriteLine("Player: " + playerHand.ToUpper());
            Console.WriteLine("Computer: " + aiHand.ToUpper());
            CheckWinState((playerHand, aiHand));
        } while (InputPlayAgain());
    }
}