using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

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

        public bool MakeMove(byte move)
		{
            move--;
            for(short i = 0; i < Gameboard.Rows.Count; i++)
                if (Gameboard.Rows[i][move].ClaimedBy == null || (Gameboard.Mode == Gameboard.GameMode.Infinity && i == Gameboard.Rows.Count - 1))
				{
                    // In 'Infinity' mode, when a counter is placed on the highest row, a new row is added to the top of the board.
                    // This code checks to see if a counter has been placed on the highest row and if so it adds a new row to the top of the board.
                    if (Gameboard.Mode == Gameboard.GameMode.Infinity && i == Gameboard.Rows.Count - 1 && Gameboard.Rows[i][move].ClaimedBy != null) Gameboard.RowDropper();
                    Gameboard.Rows[i][move].ClaimCounter(this);
                    Gameboard.History.Add((Gameboard.Rows[i][move], this));
                    Moves++;
                    return true;
				}
            return false;
		}


        public void ComputerMove(Player opponent)
        {
            // Variables to be used throughout this method.
            Random random = new Random();

            // Grab all the possible moves that can be made by the AI.
            Counter[] possibleMoves = new Counter[Gameboard.BoardWidth];
            for (int j = 0; j < Gameboard.BoardWidth; j++)
                for (int i = 0; i < Gameboard.Rows.Count; i++)
                    if (Gameboard.Rows[i][j].ClaimedBy == null) // Go up the rows to make sure claimed counters are ignored.
                    {
                        possibleMoves[j] = Gameboard.Rows[i][j];
                        break;
                    }
            bool urgentMove = false;
            foreach (Counter move in possibleMoves)
            {
                if (move != null)
                {
                    move.EvaluateValue(this, opponent);
                    if (move.GetValue() > 49) urgentMove = true;
                }
            }

            Counter[] sortedMoves = possibleMoves.Where(m => m != null && m.ClaimedBy == null).OrderByDescending(m => m.GetValue()).ToArray();
            if(urgentMove || PlayerDifficulty == Difficulty.Master) MakeMove((byte)(sortedMoves[0].Coordinates.Item2 + 1));
            else
			{
                if (PlayerDifficulty == Difficulty.Easy)
                    MakeMove((byte)(sortedMoves[random.Next(sortedMoves.Length % 2 == 0 ? (sortedMoves.Length / 2) + 1 : (sortedMoves.Length + 1) / 2)].Coordinates.Item2 + 1));
                else if (PlayerDifficulty == Difficulty.Medium)
                    MakeMove((byte)(sortedMoves[random.Next(sortedMoves.Length % 2 == 0 ? (sortedMoves.Length / 3) + 1 : (sortedMoves.Length + 1) / 3)].Coordinates.Item2 + 1));
                else if (PlayerDifficulty == Difficulty.Hard)
                    MakeMove((byte)(sortedMoves[random.Next(sortedMoves.Length % 2 == 0 ? (sortedMoves.Length / 4) + 1 : (sortedMoves.Length + 1) / 4)].Coordinates.Item2 + 1));
			}
        }

        public void ChangeDifficulty()
		{
            Console.Clear();
            Console.Write("What dificulty would you like to change ");
            CX.Print(Name, Colour);
			Console.WriteLine(" to?");
			Console.Write("Your current difficulty is: ");
			if (PlayerDifficulty == Difficulty.Easy) Console.WriteLine("EASY");
			if (PlayerDifficulty == Difficulty.Medium) Console.WriteLine("NORMAL");
			if (PlayerDifficulty == Difficulty.Hard) Console.WriteLine("HARD");
            if (PlayerDifficulty == Difficulty.Master) Console.WriteLine("MASTER");

			Console.WriteLine("\nPlease select from the following options by entering the associated number.");
            Console.WriteLine("1 - Easy Difficulty");
			Console.WriteLine("2 - Normal Difficulty");
			Console.WriteLine("3 - Hard Difficulty");
			Console.WriteLine("4 - Master Difficulty");
			Console.WriteLine("0 - Return without changing difficulty.");

            byte answer = CX.GetKey(limit: 4);
			switch (answer)
			{
                case 0:
                    return;
                case 1:
                    PlayerDifficulty = Difficulty.Easy;
                        break;
                case 2:
                    PlayerDifficulty = Difficulty.Medium;
                        break;
                case 3:
                    PlayerDifficulty = Difficulty.Hard;
                        break;
                case 4:
                    PlayerDifficulty = Difficulty.Master;
                        break;
			}
			Console.Write("Difficulty for ");
            CX.Print(Name, Colour);
			Console.WriteLine(" changed to ");
            if (PlayerDifficulty == Difficulty.Easy) Console.WriteLine("EASY!");
            if (PlayerDifficulty == Difficulty.Medium) Console.WriteLine("NORMAL!");
            if (PlayerDifficulty == Difficulty.Hard) Console.WriteLine("HARD!");
            if (PlayerDifficulty == Difficulty.Master) Console.WriteLine("MASTER!");

			Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
        }

        private string ChangeName(Player opponent)
		{
            if (GameEngine.NumberOfPlayers == 0) return "Sorry, you cannot change computer player names.";
            while (true)
			{
                Console.Write("Your current name is: ");
                CX.Print(Name, Colour);
                Console.WriteLine(".\nYour opponent is named: ");
                CX.Print(opponent.Name, opponent.Colour);

                Console.WriteLine("Please choose a name.");
                string newName;
                try { newName = CX.Read(); }
				catch (Exception e)
				{
					Console.WriteLine(e);
                    continue;
				}
				if (newName == "") Console.WriteLine("You have not entered a name, please do that.");
                else if (opponent.IsRobot)
				{
                    if (newName.ToLower().Contains("computer") ||
                        newName.ToLower().Contains("a.i."))
                        Console.WriteLine("Sorry, this name is unavailable. Please try something that isn't related to being a computer player.");
					else if (Name == opponent.Name) Console.WriteLine("This name is reserved for the computer player.");
                    else
                    {
                        Name = newName;
                        return Name;
                    }
				}
                else if (Name == opponent.Name) Console.WriteLine("This name is taken by the other player.");
                else return Name;
			}
		}

        public static void ChangeNameChoice(Player player1, Player player2)
        {
            Player chosenPlayer = player1, otherPlayer = player2;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("What player would you like to change the name of?");
                CX.Print($"1 - {player1.Name}\n", player1.Colour, null, true);
                CX.Print($"2 - {player2.Name}\n", player2.Colour, null, true);
                Console.WriteLine("Please enter your choice...");
                Console.WriteLine("Enter \"0\" to exit without changing any player names...");
                try
                {
                    byte playerNameChangeChoice = CX.GetKey(limit: 2);
                    if (playerNameChangeChoice == 2)
                    {
                        chosenPlayer = player2;
                        otherPlayer = player1;
                    }
                    else if (playerNameChangeChoice == 0) return;
                    else CX.Print("That wasn't a valid option, please try again...\n", ConsoleColor.Red, null, true);
                    while (true)
                    {
                        byte answer = 0;
                        chosenPlayer.ChangeName(otherPlayer);
                        Console.WriteLine();
                        while (true)
                        {
                            Console.Write("Would you also like to rename ");
                            CX.Print(otherPlayer.Name, otherPlayer.Colour);

                            Console.Write("?\n1 - Yes | 2 - No | 0 - Rename ");
                            CX.Print(chosenPlayer.Name, chosenPlayer.Colour);

                            answer = CX.GetKey(limit: 2);
                            if (answer == 0 || answer == 1) break;
                            else if (answer == 2) return;
                            else
                            {
                                Console.WriteLine("That wasn't a valid option.\nPress any key to try again...");
                                Console.ReadKey();
                                Console.Clear();
                            }
                        }
                        if (answer == 0) continue;
                        else if (answer == 1)
                        {
                            Player temporaryPlayer = chosenPlayer;
                            chosenPlayer = otherPlayer;
                            otherPlayer = temporaryPlayer;
                            break;
                        }
                    }
                }
                catch { CX.Catch(true); }
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
                    CX.Print($"1 - {player1.Name}", player1.Colour, null, true);
                    CX.Print($"2 - {player2.Name}", player2.Colour, null, true);
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
                    catch { CX.Catch(false); }
                    finally { Console.Clear(); }
                }

                // Selecting a colour to set for the chosen user.
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("Please select a colour");

                    // We get the colours from the Print method.
                    ConsoleColor?[] colourChoices = new ConsoleColor?[12];
                    colourChoices[0] = CX.Print("1 - Dark Red", ConsoleColor.DarkRed, null, true);
                    colourChoices[1] = CX.Print("2 - Red", ConsoleColor.Red, null, true);
                    colourChoices[2] = CX.Print("3 - Dark Yellow", ConsoleColor.DarkYellow, null, true);
                    colourChoices[3] = CX.Print("4 - Yellow", ConsoleColor.Yellow, null, true);
                    colourChoices[4] = CX.Print("5 - Green", ConsoleColor.Green, null, true);
                    colourChoices[5] = CX.Print("6 - Dark Green", ConsoleColor.DarkGreen, null, true);
                    colourChoices[6] = CX.Print("7 - Dark Blue", ConsoleColor.DarkBlue, null, true);
                    colourChoices[7] = CX.Print("8 - Blue", ConsoleColor.Blue, null, true);
                    colourChoices[8] = CX.Print("9 - Dark Cyan", ConsoleColor.DarkCyan, null, true);
                    colourChoices[9] = CX.Print("10 - Cyan", ConsoleColor.Cyan, null, true);
                    colourChoices[10] = CX.Print("11 - Pink", ConsoleColor.Magenta, null, true);
                    colourChoices[11] = CX.Print("12 - Dark Pink", ConsoleColor.DarkMagenta, null, true);
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
                                CX.Print(otherPlayer.Name, otherPlayer.Colour);
                                Console.WriteLine("?");
                                Console.Write("1 - Yes | 2 - No | 0 - Recolour ");
                                CX.Print(chosenPlayer.Name, chosenPlayer.Colour, null, true);

                                // Processing user input.
                                answer = CX.GetKey(limit: 2);
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
                            if (answer == 1)
                            {
                                Player temporaryPlayer = chosenPlayer;
                                chosenPlayer = otherPlayer;
                                otherPlayer = temporaryPlayer;
                                break;
                            }
                        }
                    }
                    catch { CX.Catch(null); }
                    Console.Clear();
                }
            }
        }
    }
}
