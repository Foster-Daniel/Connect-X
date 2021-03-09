using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConnectX
{
    class GameEngine
    {
        public enum Difficulty : byte { Easy, Medium, Hard, Master }
        public static byte NumberOfPlayers { get; set; }
		public Difficulty difficulty { get; set; }
		public static Gameboard.GameMode ChooseGameMode(Player player1, Player player2)
		{
            do
            {
                Console.WriteLine("Choose a gamemode!");
                Program.Print("1 - Classic Mode", player1.Colour, null, true);
                Program.Print("2 - Infinity Mode\n", player2.Colour, null, true);
                try
                {
                    Gameboard.Mode = (Gameboard.GameMode)Program.GetKey() - 1;
                    Console.Clear();
                }
                catch { Program.Catch(false); }
                if (Gameboard.Mode == Gameboard.GameMode.Classic) Console.WriteLine("You have chosen Classic Mode!\n");
                else if (Gameboard.Mode == Gameboard.GameMode.Infinity) Console.WriteLine("You have chosen Infinity Mode!\n");
                else Program.Print("Please try again.", ConsoleColor.Red);
            } while (Gameboard.Mode != Gameboard.GameMode.Classic && Gameboard.Mode != Gameboard.GameMode.Infinity); 
            return Gameboard.Mode;
		}

        public static byte ChooseNumberOfPlayers(Player player1, Player player2)
        {  
            // Giving the players names based on possibility they may be A.I.
            // TODO: Code the computer player's moves.
            player2.Name = "Computer A.I.";
            byte numberOfPlayers = 2;
            do
            {
                // Asking the user how many players there will be in the game?
                Console.Write("How many players will be playing: ");
                Program.Print("1", player1.Colour);
                Console.Write(" or ");
                Program.Print("2", player2.Colour);
                Console.WriteLine("?");
                Console.WriteLine("Please enter your answer as a whole number.");

                try { numberOfPlayers = Program.GetKey(limit: 2); }
                catch { Program.Print("PLEASE ENTER YOUR ANSWER AS AN INTEGER. THIS MEANS A NUMBER WITH NO DECIMAL POINTS.", ConsoleColor.DarkRed); }

                if (numberOfPlayers == 0)
                {
                    byte answer = 0;
                    while (true)
                    {
                        Console.WriteLine("\nThis will result in the computer versing itself while you watch. Are you sure?");
                        Console.WriteLine("[1] - Yes\n[2] - No");
                        try { answer = Program.GetKey(limit: 2); }
                        catch
                        {
                            Console.Clear();
                            Console.WriteLine("Please choose from the following...\n[1] - Yes\n[2] - No");
                        }
                        if (answer == 1)
                        {
                            player1.Name = "Computer A.I. 1";
                            player2.Name = "Computer A.I. 2";
                            Console.Write("Okay, enjoy...");
                        }
                        else if (answer == 2)
                        {
                            numberOfPlayers = 99; // This will force the outer loop to continue. 99 was arbitrarily chosen.
                            Console.Clear();
                            Console.WriteLine("Please choose again.");
                        }
                        else // This shouldn't run but just incase it is here to catch anything unexpected.
                        {
                            Program.Print("Sorry, I don't understand, please try again.", ConsoleColor.Red);
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                            Console.Clear();
                        }
                    }
                }
                else if(numberOfPlayers == 1 || numberOfPlayers == 2)
                {
                    player1.IsRobot = false;
                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine("Enter the name for Player 1...");
                        player1.Name = Program.Read(player1.Colour).Trim();
                        if (player1.Name.Length < 1 || (player1.Name.ToLower().Contains("computer")
                            || player1.Name.ToLower().Contains("a.i") && numberOfPlayers == 1))
                        {
                            Console.Clear();
                            if (player1.Name.Length < 1) Console.WriteLine("You must give Player 1 a name.");
                            else Console.WriteLine("That name is reserved... Please choose something else.");
							Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                            continue;
                        }
                        Console.Write($"\nPlayer 1's name is set to ");
                        Program.Print(player1.Name, player1.Colour);
                        Console.WriteLine("!\n");
                        break;
                    }
                    if (numberOfPlayers == 2)
                    {
                        player2.IsRobot = false;
                        while (true)
                        {
                            Console.Clear();
                            Console.Write($"\nPlayer 1's name is set to ");
                            Program.Print(player1.Name, player1.Colour);
                            Console.WriteLine("!\n");
                            Console.WriteLine("Enter the name for Player 2...");
                            player2.Name = Program.Read(player2.Colour).Trim();
                            if (player2.Name.Length < 1 || player2.Name.ToLower() == player1.Name.ToLower())
                            {
                                Console.Clear();
                                if (player2.Name.Length < 0) Console.WriteLine("You musy give Player 2 a name.");
                                if (player2.Name.ToLower().Trim() == player1.Name.ToLower().Trim()) Console.WriteLine("The player names cannot be the same.");
                                continue;
                            }
                            Console.Write("\nPlayer 2's name is set to ");
                            Program.Print(player2.Name, player2.Colour);
                            Console.WriteLine("!\n");
                            break;
                        }
                    }
                }
                else // This shouldn't run but just incase it is here to catch anything unexpected.
                {
                    numberOfPlayers = 99; // Resets the outer loop.
                    Console.Clear();
				    Console.WriteLine("Sorry, I don't understand, please make another attempt.\nI can only accept numerical answers.\n");
				}
            } while (numberOfPlayers < 0 || numberOfPlayers > 2);
            NumberOfPlayers = (byte)(numberOfPlayers < 0 || numberOfPlayers > 2 ? 2 : numberOfPlayers); // Guaranteeing that there are no conversion issues that could cause an exception.
            return NumberOfPlayers;
		}
        public static void VictoryMessage(Player winningPlayer, Player losingPlayer, DateTime startingTime)
		{
            Gameboard.PrintGameBoard(true);
            Console.Write("\nCongratulations to ");
            Program.Print(winningPlayer.Name, winningPlayer.Colour, null, true);
            Console.Write("Commiseration to ");
            Program.Print(losingPlayer.Name, losingPlayer.Colour, null, true);

            Program.Print($"\n{winningPlayer.Name}", winningPlayer.Colour);
            Console.Write($" made {winningPlayer.Moves} during the game.\n");

            Program.Print($"\n{losingPlayer.Name}", losingPlayer.Colour);
            Console.Write($" made {losingPlayer.Moves} during the game.\n");

            Console.WriteLine($"A total of {winningPlayer.Moves + losingPlayer.Moves} where made between both players.\n");

            Console.WriteLine($"The game took {DateTime.Now - startingTime}.\n");
            Console.WriteLine("Would you like to play again?");
            Console.WriteLine("1 - Yes\n2 - No\n3 - Show Game Replay");
        }
	}
}
