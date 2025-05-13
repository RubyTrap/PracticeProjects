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
            default:
            case 3:
                return Hand.Scissors;
        }
    }
    
    static Hand GetPlayerHand()
    {
        while (true)
        {
            Console.Write("Enter ROCK, PAPER, or SCISSORS: ");
            string plrInput = Console.ReadLine() ?? "";
            switch (plrInput.ToLower().Trim())
            {
                case "rock":
                case "r":
                    return Hand.Rock;
                case "paper":
                case "p":
                    return Hand.Paper;
                case "scissors":
                case "s":
                    return Hand.Scissors;
                default:
                    Console.WriteLine("Invalid Input");
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
            string plrInput = Console.ReadLine() ?? "";
            if (plrInput == "")
            {
                continue;
            }

            switch(plrInput[0])
            {
                case 'y':
                    return true;
                case 'n':
                    Console.Write("Thanks for playing");
                    return false;
                default:
                    Console.WriteLine("Please enter valid input N/Y");
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