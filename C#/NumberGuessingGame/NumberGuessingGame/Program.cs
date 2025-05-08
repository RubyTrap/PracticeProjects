namespace NumberGuessingGame;

class Program
{
    static void Main(string[] args)
    {
        bool gameLoop = true;
        Random rnd = new Random();
        const int min = 1;
        const int max = 100;
        int rndNum = rnd.Next(min, max+1);
        int guesses = 1;
        while (gameLoop)
        {
            Console.WriteLine("Guess a number between " + min + " and " + max);
            Console.Write("Guess: ");
            bool success = int.TryParse(Console.ReadLine(), out int guess);
            if (!success)
            {
               Console.WriteLine("Enter a valid number"); 
            }
            else
            {
                if (guess < rndNum)
                {
                    Console.WriteLine(guess + " is too low");
                    guesses++;
                }
                else if (guess > rndNum)
                {
                    Console.WriteLine(guess + " is too high");
                    guesses++;
                }
                else
                {
                    Console.WriteLine("You win");
                    Console.WriteLine("Number of guesses: " + guesses);
                    Console.Write("Would you like to play again? (Y/N): ");
                    while (true)
                    {    
                        string? again = Console.ReadLine();
                        if (again == null)
                        {
                            Console.Write("Please enter Y or N: ");
                        }
                        else
                        {
                            string lower = again.Trim().ToLower();
                            if (lower != "y" && lower != "n")
                            {
                                Console.Write("Please enter Y or N: ");
                            }
                            else if (lower == "y")
                            {
                                rndNum = rnd.Next(min, max + 1);
                                guesses = 1;
                                break;
                            }
                            else
                            {
                                Console.Write("Thanks for playing");
                                gameLoop = false;
                                break;
                            }
                        }
                        
                    }
                }
            }
        }
    }
}