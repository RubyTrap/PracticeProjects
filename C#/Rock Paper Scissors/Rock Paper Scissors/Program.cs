using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Rock_Paper_Scissors;

enum Hand
{
    Rock, 
    Paper, 
    Scissors
}

class Program
{
    static Hand GetAiHand()
    {
        int rndInt = Random.Shared.Next(1, 4);
        switch (rndInt)
        {
            case 1: 
                return Hand.Rock;
            case 2:
                return Hand.Paper;
            case 3:
                return Hand.Scissors;
            default:
                throw new UnreachableException();
        }
    }
    
    static Hand GetPlayerHand()
    {
        while (true)
        {
            Console.Write("Enter ROCK, PAPER, or SCISSORS: ");
            string playerInput = Console.ReadLine() ?? "";
            if (playerInput == "")
            {
                Console.WriteLine("Please enter a valid input rock paper or scissors");
                continue;
            }
            switch (playerInput.ToLower().Trim()[0])
            {
                case 'r':
                    return Hand.Rock;
                case 'p':
                    return Hand.Paper;
                case 's':
                    return Hand.Scissors;
                default:
                    Console.WriteLine("Please enter a valid input rock paper or scissors");
                    break;
            }
        }
    }

    static void CheckWinState(Hand player, Hand computer)
    {
        if (player == computer)
        {
            Console.WriteLine("It's a draw");
        }
        else if (player == Hand.Rock && computer == Hand.Scissors ||
                 player == Hand.Paper && computer == Hand.Rock ||
                 player == Hand.Scissors && computer == Hand.Paper)
        {
            Console.WriteLine("You win");
        }
        else
        {
            Console.WriteLine("You lose");
        }
        
    }

    static bool InputPlayAgain()
    {
        while (true)
        {
            Console.Write("Would you like to play again (Y/N): ");
            string playerInput = Console.ReadLine() ?? "";
            if (playerInput == "")
            {
                continue;
            }

            switch(playerInput.Trim().ToLower()[0])
            {
                case 'y':
                    return true;
                case 'n':
                    Console.WriteLine("Please enter valid input Y/N");
                    Console.Write("Thanks for playing");
                    return false;
                default:
                    Console.WriteLine("Please enter valid input Y/N");
                    break;
            }
        }
    }
    
    static void Main()
    {
        do
        {
            var aiHand = GetAiHand();
            var playerHand = GetPlayerHand();
            Console.WriteLine("Player: " + playerHand.ToString().ToUpper());
            Console.WriteLine("Computer: " + aiHand.ToString().ToUpper());
            CheckWinState(playerHand, aiHand);
        } while (InputPlayAgain());
    }
}