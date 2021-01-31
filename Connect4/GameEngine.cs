using System;
using System.Collections.Generic;
using System.Text;

namespace Connect4
{
	class GameEngine
	{
        public enum GameMode: sbyte { Classic, Infinity }
		public static byte NumberOfPlayers { get; set; }
		public static GameMode ChooseGameMode(Player player1, Player player2)
		{
            GameMode gameMode = GameMode.Classic;
            do
            {
                Console.WriteLine("Choose a gamemode!");
                Program.Print("1 - Classic Mode", player1.Colour, null, true);
                Program.Print("2 - Infinity Mode\n", player2.Colour, null, true);
                try
                {
                    gameMode = (GameMode)Program.GetKey() - 1;
                    Console.Clear();
                }
                catch { Program.Catch(false); }
                if (gameMode == GameMode.Classic) Console.WriteLine("You have chosen Classic Mode!\n");
                else if (gameMode == GameMode.Infinity) Console.WriteLine("You have chosen Infinity Mode!\n");
                else Program.Print("Please try again.", ConsoleColor.Red);
            } while (gameMode != GameMode.Classic && gameMode != GameMode.Infinity); 
                return gameMode;
		}

        public static byte ChooseNumberOfPlayers(Player player1, Player player2)
        {  
            // Giving the players names based on possibility they may be A.I.
            // TODO: Code the computer player's moves.
            player2.Name = "Computer A.I.";
            byte numberOfPlayers = 2;
            do
            {
                Console.Write("How many players will be playing: ");
                Program.Print("1", player1.Colour);
                Console.Write(" or ");
                Program.Print("2", player2.Colour);
                Console.WriteLine("?");
                Console.WriteLine("Please enter your answer as a whole number.");

                try { numberOfPlayers = Program.GetKey(); }
                catch { Program.Print("PLEASE ENTER YOUR ANSWER AS AN INTEGER. THIS MEANS A NUMBER WITH NO DECIMAL POINTS.", ConsoleColor.DarkRed); }

                switch (numberOfPlayers)
				{
                    case 0:
                        string answer = null;
						while (true)
                        {
                            Console.WriteLine("\nThis will result in the computer versing itself while you watch. Are you sure?");
                            Console.WriteLine("Please type \"yes\" or \"no\". :)");
                            try { answer = Console.ReadLine().ToLower().Replace(" ", ""); }
                            catch
                            {
                                Console.Clear();
                                Console.WriteLine("Please type \"yes\" or \"no\". :)\n");
                            }
                            if (answer.StartsWith("y")) // So people can be funny and write something dumb like "yaaa, boii" not that it matters or anything.
                            {
                                player1.Name = "Computer A.I. 1";
                                player2.Name = "Computer A.I. 2";
                                Console.Write("Okay, enjoy ");
                            }
                            else if (answer.StartsWith("n"))
                            {
                                numberOfPlayers = 99;
                                Console.Clear();
                                Console.WriteLine("Please choose again.");
                            }
                            else
                            {
                                Program.Print("Sorry, I don't understand, please try again.", ConsoleColor.Red);
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey();
                                Console.Clear();
                            }
                        }
                    case 1:
                    case 2:
                        Console.Clear();
                        Console.WriteLine("Enter the name for Player 1...");
                        player1.Name = Program.Read<string>(player1.Colour);
                        Console.Write($"\nPlayer 1's name is set to ");
                        Program.Print(player1.Name, player1.Colour);
                        Console.WriteLine("!\n");
                        if (numberOfPlayers == 2)
                        {
                            Console.Clear();
                            Console.WriteLine("Enter the name for Player 2...");
                            player2.Name = Program.Read<string>(player2.Colour);
                            Console.Write("\nPlayer 2's name is set to ");
                            Program.Print(player2.Name, player2.Colour);
                            Console.WriteLine("!\n");
                        }
                        break;
                    default:
                        numberOfPlayers = 99;
                        Console.Clear();
						Console.WriteLine("Sorry, I don't understand, please make another attempt.\nI can only accept numerical answers.\n");
                        break;
				}
            } while (numberOfPlayers < 0 || numberOfPlayers > 2);
            NumberOfPlayers = (byte)(numberOfPlayers < 0 || numberOfPlayers > 2 ? 2 : numberOfPlayers); // Guaranteeing that there are no conversion issues that could cause an exception.
            return NumberOfPlayers;
		}
	}
}
