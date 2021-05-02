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
                CX.Print("1 - Classic Mode", player1.Colour, null, endLine: true);
                CX.Print("2 - Infinity Mode\n", player2.Colour, null, endLine: true);
                try
                {
                    Gameboard.Mode = (Gameboard.GameMode)CX.GetKey() - 1;
                    Console.Clear();
                }
                catch { CX.Catch(false); }
                if (Gameboard.Mode == Gameboard.GameMode.Classic) Console.WriteLine("You have chosen Classic Mode!\n");
                else if (Gameboard.Mode == Gameboard.GameMode.Infinity) Console.WriteLine("You have chosen Infinity Mode!\n");
                else CX.Print("Please try again.", ConsoleColor.Red);
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
                CX.Print("1", player1.Colour);
                Console.Write(" or ");
                CX.Print("2", player2.Colour);
                Console.WriteLine("?");
                Console.WriteLine("Please enter your answer as a whole number.");

                try { numberOfPlayers = CX.GetKey(limit: 2); }
                catch { CX.Print("PLEASE ENTER YOUR ANSWER AS AN INTEGER. THIS MEANS A NUMBER WITH NO DECIMAL POINTS.", ConsoleColor.DarkRed); }

                if (numberOfPlayers == 0)
                {
                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine("This will result in the computer versing itself while you watch. Are you sure?");
                        Console.WriteLine("1 - Yes\n2 - No");
                        byte answer = CX.GetKey(limit: 2);
                        if (answer == 1)
                        {
                            player1.Name = "Computer A.I. 1";
                            player2.Name = "Computer A.I. 2";
                            Console.Write("Okay, enjoy...");
                            break;
                        }
                        else if (answer == 2)
                        {
                            numberOfPlayers = 99; // This will force the outer loop to continue. 99 was arbitrarily chosen.
                            Console.Clear();
                            Console.WriteLine("Please choose again.");
                            break;
                        }
                        else // This shouldn't run but just incase it is here to catch anything unexpected.
                        {
                            CX.Print("Sorry, I don't understand, please try again.", ConsoleColor.Red);
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
                        player1.Name = CX.Read(player1.Colour).Trim();
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
                        CX.Print(player1.Name, player1.Colour);
                        Console.WriteLine("!\n");
                        break;
                    }
                    if (numberOfPlayers == 2)
                    {
                        player2.IsRobot = false;
                        while (true)
                        {
                            Console.Clear();
                            Console.Write($"Player 1's name is set to ");
                            CX.Print(player1.Name, player1.Colour);
                            Console.WriteLine("!\n");
                            Console.WriteLine("Enter the name for Player 2...");
                            player2.Name = CX.Read(player2.Colour).Trim();
                            if (player2.Name.Length < 1 || player2.Name.ToLower() == player1.Name.ToLower())
                            {
                                Console.Clear();
                                if (player2.Name.Length < 0) Console.WriteLine("You must give Player 2 a name.");
                                if (player2.Name.ToLower().Trim() == player1.Name.ToLower().Trim()) Console.WriteLine("The player names cannot be the same.");
                                continue;
                            }
                            Console.Write("\nPlayer 2's name is set to ");
                            CX.Print(player2.Name, player2.Colour);
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
        public static void ModifyGame(Player player1, Player player2)
		{
            Console.Clear();
            if (NumberOfPlayers < 2)
            {
                Console.WriteLine("Would you like to modify the game board or change the A.I. difficulty?");
                Console.WriteLine("Please select from the options below...");
                Console.WriteLine("1 - Modify Board and Game Parameters");
                Console.WriteLine("2 - Change A.I. Difficulty");
                Console.WriteLine("0 - Return without changing anything");
                byte answer = CX.GetKey(limit: 2);
                if (answer == 0) return;
                if (answer == 1) Gameboard.CustomiseGame(player1, player2);
                if (answer == 2)
                {
                    if (player1.IsRobot && player2.IsRobot)
                    {
                        Console.WriteLine("Who would you like to change the difficulty of?");
                        Console.Write("1 - ");
                        CX.Print(player1.Name, player1.Colour, null, true);
                        Console.Write("2 - ");
                        CX.Print(player2.Name, player2.Colour, null, true);
                        Console.WriteLine("0 - I changed my mind and don't want to change any difficulties.");
                        byte secondAnswer = CX.GetKey(limit: 2);
                        if (secondAnswer == 0) return;
                        if (secondAnswer == 0) player1.ChangeDifficulty();
                        if (secondAnswer == 0) player2.ChangeDifficulty();
                    }
                    else player2.ChangeDifficulty();
                }
            }
            else Gameboard.CustomiseGame(player1, player2);
		}
        public static void VictoryMessage(Player winningPlayer, Player losingPlayer, DateTime startingTime)
		{
            Gameboard.PrintGameBoard(true);
            Console.Write("\nCongratulations to ");
            CX.Print(winningPlayer.Name, winningPlayer.Colour, null, true);
            Console.Write("Commiseration to ");
            CX.Print(losingPlayer.Name, losingPlayer.Colour, null, true);

            CX.Print($"\n{winningPlayer.Name}", winningPlayer.Colour);
            Console.Write($" made {winningPlayer.Moves} during the game.\n");

            CX.Print($"\n{losingPlayer.Name}", losingPlayer.Colour);
            Console.Write($" made {losingPlayer.Moves} during the game.\n");

            Console.WriteLine($"A total of {winningPlayer.Moves + losingPlayer.Moves} where made between both players.\n");

            Console.WriteLine($"The game took {DateTime.Now - startingTime}.\n");
            Console.WriteLine("Would you like to play again?");
            Console.WriteLine("1 - Yes\n2 - No\n0 - Show Game Replay");
        }
	}
}
