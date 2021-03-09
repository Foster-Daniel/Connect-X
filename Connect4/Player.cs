using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectX
{
    public class Player
    {
        public enum Difficulty : byte { Easy, Medium, Hard, Master };
        public string Name {get; set;}
        public ConsoleColor? Colour {get; set;}
        public bool Victory {get; set;}
        public int Moves {get; set;}
        public char Marker { get; set; } = 'O';
        public bool IsRobot { get; set; } = true;
        public Difficulty PlayerDifficulty { get; set; } = Difficulty.Medium;

		public Player(string name, ConsoleColor colour)
        {
            Colour = colour;
            Name = name;
            Moves = 0;
            Marker = 'O';
        }

        public void MakeMove(byte move)
		{
            move--;
            for(short i = 0; i < Gameboard.Row.Count - 1; i++)
                if (Gameboard.Row[i][move].ClaimedBy == null)
				{
                    Gameboard.Row[i][move].ClaimCounter(this);
                    Gameboard.History.Add((Gameboard.Row[i][move], this));

                    // In 'Infinity' mode, when a counter is placed on the highest row, a new row is added to the top of the board.
                    // This code checks to see if a counter has been placed on the highest row and if so it adds a new row to the top of the board.
                    if (Gameboard.Mode == Gameboard.GameMode.Infinity && i == Gameboard.Row.Count - 1) Gameboard.Row.Add(new Counter[Gameboard.BoardWidth]);
                    break;
				}
            Moves++;
		}


        public void ComputerMove(Player otherPlayer)
        {
            // Vairables to be used throughout this method.
            Player chosenPlayer = this;
            Random random = new Random();

            // Grab all the possible moves that can be made by the AI.
            Counter[] possibleMoves = new Counter[Gameboard.BoardWidth];
            for (int j = 0; j < Gameboard.BoardWidth; j++)
                for (int i = 0; i < Gameboard.Row.Count; i++)
                    if (Gameboard.Row[i][j].ClaimedBy == null)
                    {
                        possibleMoves[j] = Gameboard.Row[i][j];
                        break;
                    }

            foreach (Counter move in possibleMoves)
                Gameboard.CheckCounter(move);

            possibleMoves.OrderByDescending(m => m.GetValue());

            if (PlayerDifficulty == Difficulty.Easy) MakeMove(possibleMoves[random.Next(Gameboard.BoardWidth % 2 == 0 ? Gameboard.BoardWidth / 2 : (Gameboard.BoardWidth + 1) / 2)].Coordinates.Item2);
            else if (PlayerDifficulty == Difficulty.Medium) MakeMove(possibleMoves[random.Next(Gameboard.BoardWidth % 2 == 0 ? Gameboard.BoardWidth / 3 : (Gameboard.BoardWidth + 1) / 3)].Coordinates.Item2);
            else if (PlayerDifficulty == Difficulty.Hard) MakeMove(possibleMoves[random.Next(Gameboard.BoardWidth % 2 == 0 ? Gameboard.BoardWidth / 4 : (Gameboard.BoardWidth + 1) / 4)].Coordinates.Item2);
            else if (PlayerDifficulty == Difficulty.Master) MakeMove((byte)(possibleMoves[0].Coordinates.Item2 - 1));
        }


        public static void ChangeName(Player player1, Player player2)
        {
            while (true)
            {
                Player chosenPlayer = player1, otherPlayer = player2;
                Console.Clear();
                Console.WriteLine("What player would you like to change the name of?");
                Program.Print($"1 - {player1.Name}\n", player1.Colour, null, true);
                Program.Print($"2 - {player2.Name}\n", player2.Colour, null, true);
                Console.WriteLine("Please enter your choice...");
                Console.WriteLine("Enter \"-1\" to exit without changing any player names...");
                try
                {
                    sbyte playerNameChangeChoice = sbyte.Parse(Console.ReadLine());
                    if (playerNameChangeChoice == 1 || playerNameChangeChoice == 2)
                    {
                        chosenPlayer = player2;
                        otherPlayer = player1;
                    }
                    else if (playerNameChangeChoice == -1) return;
                    else Program.Print("That wasn't a valid option, please try again...\n", ConsoleColor.Red, null, true);
                    while (true)
                    {
                        sbyte answer = 0;
                        Console.Clear();
                        Console.Write("Please enter the new name for ");

                        Program.Print(chosenPlayer.Name, chosenPlayer.Colour);
                        Console.WriteLine(".");
                        chosenPlayer.Name = Program.Read(chosenPlayer.Colour);
                        Console.WriteLine();
                        while (true)
                        {
                            Console.Write("Would you also like to rename ");
                            Program.Print(otherPlayer.Name, otherPlayer.Colour);

                            Console.Write("?\n1 - Yes | 2 - No | 0 - Rename ");
                            Program.Print(chosenPlayer.Name, chosenPlayer.Colour);

                            answer = sbyte.Parse(Console.ReadLine());
                            if (answer == 0 || answer == 1) break;
                            else if (answer == 2) return;
                            else
                            {
                                answer = -2;
                                Console.WriteLine("That wasn't a valid option.\nPress any key to try again...");
                                Console.ReadKey();
                                Console.Clear();
                            }
                        }
                        if (answer == 0) continue;
                        else if (answer == 1) break;
                    }
                }
                catch { Program.Catch(true); }
                finally { Console.Clear(); }
            }
        }


        public static void ColourChanger(Player player1, Player player2)
        {
            while (true)
            {
                // Declaring the player objects that determine which player has been selected.
                Player chosenPlayer = player1, otherPlayer = player2;

                sbyte playerColourChoice = 0; // Declaring the variable for user input and giving it a default that triggers the question .
                Console.Clear();

                // Choosing the player to change the colour of.
                while (playerColourChoice != 1 && playerColourChoice != 2)
                {
                    Console.WriteLine("What player would you like to change the colour of?");
                    Program.Print($"1 - {player1.Name}", player1.Colour, null, true);
                    Program.Print($"2 - {player2.Name}", player2.Colour, null, true);
                    Console.WriteLine("Please enter your choice...");
                    Console.WriteLine("Enter \"-1\" to exit without changing any player colours...");
                    try
                    {
                        playerColourChoice = sbyte.Parse(Console.ReadLine());
                        if (playerColourChoice == 2)
                        {
                            // Swapping the initialised variables values around should the user choose so.
                            chosenPlayer = player2;
                            otherPlayer = player1;
                        }
                        else if (playerColourChoice == -1)
                        {
                            Console.Clear();
                            return;
                        }
                        else Console.WriteLine("That wasn't a valid option, please try again...\n");
                    }
                    catch { Program.Catch(false); }
                    finally { Console.Clear(); }
                }

                // Selecting a colour to set for the chosen user.
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("Please select a colour");

                    // We get the colours from the Print method.
                    ConsoleColor?[] colourChoices = new ConsoleColor?[12];
                    colourChoices[0] = Program.Print("1 - Dark Red", ConsoleColor.DarkRed, null, true);
                    colourChoices[1] = Program.Print("2 - Red", ConsoleColor.Red, null, true);
                    colourChoices[2] = Program.Print("3 - Dark Yellow", ConsoleColor.DarkYellow, null, true);
                    colourChoices[3] = Program.Print("4 - Yellow", ConsoleColor.Yellow, null, true);
                    colourChoices[4] = Program.Print("5 - Green", ConsoleColor.Green, null, true);
                    colourChoices[5] = Program.Print("6 - Dark Green", ConsoleColor.DarkGreen, null, true);
                    colourChoices[6] = Program.Print("7 - Dark Blue", ConsoleColor.DarkBlue, null, true);
                    colourChoices[7] = Program.Print("8 - Blue", ConsoleColor.Blue, null, true);
                    colourChoices[8] = Program.Print("9 - Dark Cyan", ConsoleColor.DarkCyan, null, true);
                    colourChoices[9] = Program.Print("10 - Cyan", ConsoleColor.Cyan, null, true);
                    colourChoices[10] = Program.Print("11 - Pink", ConsoleColor.Magenta, null, true);
                    colourChoices[11] = Program.Print("12 - Dark Pink", ConsoleColor.DarkMagenta, null, true);
                    Console.WriteLine("Chose the wrong player? Enter \"0\" to go back...");
                    Console.WriteLine("Decided you like the colours as they are? Enter \"-1\" to go back");

                    try
                    {
                        sbyte colour = sbyte.Parse(Console.ReadLine());
                        Console.Clear();

                        // Sorting out non-colour changing related options.
                        if (colour == -1) return;
                        else if (colour == 0) break;

                        // colour will always be +1 higher than the array because the array starts at '0'.
                        // Dealing with errors that would otherwise cause the player reassign the same colour or assign the same colour to different players.
                        if (chosenPlayer.Colour == colourChoices[colour - 1]) { Console.WriteLine($"{player1.Name} already has this colour selected.\n"); Console.ReadKey(); continue; }
                        else if (otherPlayer.Colour == colourChoices[colour - 1]) { Console.WriteLine($"{player2.Name} already has this colour selected, please choose another colour.\nPress any key to try again..."); Console.ReadKey(); continue; }
                        else
                        {
                            byte answer = 0;
                            while (true)
                            {
                                // Assigning the colour to the chosen player.
                                chosenPlayer.Colour = colourChoices[colour - 1];

                                // Offering the player the option to change the other players colour.
                                Console.Write("Would you like to change the colour of ");
                                Program.Print(otherPlayer.Name, otherPlayer.Colour);
                                Console.WriteLine("?");
                                Console.Write("1 - Yes | 2 - No | 0 - Recolour ");
                                Program.Print(chosenPlayer.Name, chosenPlayer.Colour, null, true);

                                // Processing user input.
                                answer = byte.Parse(Console.ReadLine());
                                if (answer == 0 || answer == 1) break;
                                else if (answer == 2) return;
                                else if (answer != 0 && answer != 1)
                                {
                                    // Error handling.
                                    Console.Clear();
                                    Console.WriteLine("That wasn't a valid option.\nPress any key to try again...");
                                    Console.ReadKey();
                                    Console.Clear();
                                }
                            }
                            if (answer == 0) continue;
                            if (answer == 1) break;
                        }
                    }
                    catch { Program.Catch(null); }
                    Console.Clear();
                }
            }
        }
    }
}
