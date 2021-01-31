using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Connect4
{
    class Program
    {
        // * TEXT COLOUR * ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        ///     Print text in different colours by passing them as parameters to this method. Colours are reset at the end of the function.
        /// </summary>
        /// <param name="line">The string that you want to print.</param>
        /// <param name="foregroundColour">The colour you want the text to be. Leave it null to use the default colour.</param>
        /// <param name="backgroundColour">The colour you want the background to be. Leave it null to use the default colour.</param>
        /// <param name="endLine">False by default. Set to true to end with a new line.</param>
        /// <returns>Returns the foreground colour.</returns>
        public static ConsoleColor? Print(string line, ConsoleColor? foregroundColour = null, ConsoleColor? backgroundColour = null, bool endLine = false)
        {
            line = endLine ? line + '\n' : line;
            if(foregroundColour != null) Console.ForegroundColor = (ConsoleColor)foregroundColour;
            if(backgroundColour != null) Console.BackgroundColor = (ConsoleColor)backgroundColour;
			Console.Write(line);
            Console.ResetColor();
            return foregroundColour;
        }

        /// <summary>
        ///     Use this function to receive input from the user and have that input be in a colour of your choosing.
        /// </summary>
        /// <param name="foregroundColour"></param>
        /// <param name="backgroundColour"></param>
        /// <returns>Returns the input of the user.</returns>
        public static Data Read<Data>(ConsoleColor? foregroundColour = null, ConsoleColor? backgroundColour = null)
        {
            if (foregroundColour != null) Console.ForegroundColor = (ConsoleColor)foregroundColour;
            if (backgroundColour != null) Console.BackgroundColor = (ConsoleColor)backgroundColour;

            // Getting the user input in a string form as it is the leat likely to provide errors.
            string input = Console.ReadLine();
            // Returning the data to the variable.
            Console.ResetColor();
            return (Data)Convert.ChangeType(input, typeof(Data));
            // Error Handling : WILL BE DONE LATER
                //input = Regex.Replace(input, "[a-z.,\\s!\"#¤%&/()=?`@£${[\\]}+;:öåä~¨^'*§½<>|]", "", RegexOptions.IgnoreCase,);

            // Resetting the Console Color so that any colour changes are not carried through to other functions.
        }

        // * CATCH * ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Catch(bool? justNumbers)
        {
            Console.Clear();
            if (justNumbers == true) Print("Please only enter the numbers listed followed by ", ConsoleColor.DarkRed);
            else if (justNumbers == false) Print("Please only enter the options listed by either entering their names or the number associated with it\nfollowed by ", ConsoleColor.DarkRed);
            else Print("Please only enter the options listed by either entering the name of the colour or the number associated with it\nfollowed by ", ConsoleColor.DarkRed);
            
            Print("[ENTER]", ConsoleColor.Red, ConsoleColor.DarkRed);
            Print(".", ConsoleColor.DarkRed, null, true);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }

        public static byte GetKey(bool menu = false)
		{
            ConsoleKey playerChoice = ConsoleKey.D0;
            while(true)
	    	{
                playerChoice = Console.ReadKey().Key;
                switch (playerChoice)
			    {
                    case ConsoleKey.Escape:
                        return !menu ? 0 : 1;
                    case ConsoleKey.P:
                        return !menu ? 0 : 1;
                    case ConsoleKey.D0:
                        return 0;
                    case ConsoleKey.D1:
                        return 1;
                    case ConsoleKey.D2:
                        return 2;
                    case ConsoleKey.D3:
                        return 3;
                    case ConsoleKey.D4:
                        return 4;
                    case ConsoleKey.D5:
                        return 5;
                    case ConsoleKey.D6:
                        return 6;
                    case ConsoleKey.D7:
                        return 7;
                    case ConsoleKey.D8:
                        return 8;
                    case ConsoleKey.D9:
                        return 9;
                    default:
                        Print("Please only choose from the given options....");
                        break;
                }
            }
        }

        // * MENUS & MENU OPTIONS * ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private static void Instructions()
        {
            Console.Clear();
            Console.WriteLine("Welcome to CONNECT 4: Terminal Edition");
            Console.WriteLine("The goal of the game is to match four counters in a row, collumn or diagonally.");
            Console.WriteLine("To place a counter into the grid you select the relevant number shown below the column.");
            Console.WriteLine("The counter will be placed at the lowest possible point in that collumn.\n\n\n");
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
            Console.Clear();
        }

        private static void ChangePlayerName(Player player1, Player player2)
        {
            while (true)
            {
                Player chosenPlayer = player1, otherPlayer = player2;
                Console.Clear();
                Console.WriteLine("What player would you like to change the name of?");
                Print($"1 - {player1.Name}\n", player1.Colour, null, true);
                Print($"2 - {player2.Name}\n", player2.Colour, null, true);
                Console.WriteLine("Please enter your choice...");
                Console.WriteLine("Enter \"-1\" to exit without changing any player names...");
                try
                {
                    sbyte playerNameChangeChoice = sbyte.Parse(Console.ReadLine());
                    if(playerNameChangeChoice == 1 || playerNameChangeChoice == 2)
					{
                        chosenPlayer = player2;
                        otherPlayer = player1;
					}
                    else if (playerNameChangeChoice == -1) return;
                    else Print("That wasn't a valid option, please try again...\n", ConsoleColor.Red, null, true);
                    while (true)
                    {
                        sbyte answer = 0;
                        Console.Clear();
                        Console.Write("Please enter the new name for ");

                        Print(chosenPlayer.Name, chosenPlayer.Colour);
                        Console.WriteLine(".");
                        chosenPlayer.Name = Read<string>(chosenPlayer.Colour);
                        Console.WriteLine();
                        while (true)
                        {
                            Console.Write("Would you also like to rename ");
                            Print(otherPlayer.Name, otherPlayer.Colour);

                            Console.Write("?\n1 - Yes | 2 - No | 0 - Rename ");
                            Print(chosenPlayer.Name, chosenPlayer.Colour);

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
                catch {Catch(true);}
                finally {Console.Clear();}
            }
        }

        private static void PlayerColourChanger(Player player1, Player player2)
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
                    Print($"1 - {player1.Name}", player1.Colour, null, true);
                    Print($"2 - {player2.Name}", player2.Colour, null, true);
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
                    catch { Catch(false); }
                    finally { Console.Clear(); }
                }

                // Selecting a colour to set for the chosen user.
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("Please select a colour");

                    // We get the colours from the Print method.
                    ConsoleColor?[] colourChoices = new ConsoleColor?[12];
                    colourChoices[0] = Print("1 - Dark Red", ConsoleColor.DarkRed, null, true);
                    colourChoices[1] = Print("2 - Red", ConsoleColor.Red, null, true);
                    colourChoices[2] = Print("3 - Dark Yellow", ConsoleColor.DarkYellow, null, true);
                    colourChoices[3] = Print("4 - Yellow", ConsoleColor.Yellow, null, true);
                    colourChoices[4] = Print("5 - Green", ConsoleColor.Green, null, true);
                    colourChoices[5] = Print("6 - Dark Green", ConsoleColor.DarkGreen, null, true);
                    colourChoices[6] = Print("7 - Dark Blue", ConsoleColor.DarkBlue, null, true);
                    colourChoices[7] = Print("8 - Blue", ConsoleColor.Blue, null, true);
                    colourChoices[8] = Print("9 - Dark Cyan", ConsoleColor.DarkCyan, null, true);
                    colourChoices[9] = Print("10 - Cyan", ConsoleColor.Cyan, null, true);
                    colourChoices[10] = Print("11 - Pink", ConsoleColor.Magenta, null, true);
                    colourChoices[11] = Print("12 - Dark Pink", ConsoleColor.DarkMagenta, null, true);
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
                                Print(otherPlayer.Name, otherPlayer.Colour);
                                Console.WriteLine("?");
                                Console.Write("1 - Yes | 2 - No | 0 - Recolour ");
                                Print(chosenPlayer.Name, chosenPlayer.Colour, null, true);

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
                    catch { Catch(null); }
                    Console.Clear();
                }
            }
        }

        public static byte PauseMenu(Player player1, Player player2)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("* PAUSE MENU *");
                Console.WriteLine("Select an option by entering the relevant number.");
                Console.WriteLine("1 - Continue Playing");
                Console.Write("2 - ");
                Print("C", ConsoleColor.Red);
                Print("h", ConsoleColor.DarkYellow);
                Print("a", ConsoleColor.Yellow);
                Print("n", ConsoleColor.Green);
                Print("g", ConsoleColor.Cyan);
                Print("e", ConsoleColor.DarkCyan);
                Print(" C", ConsoleColor.Blue);
                Print("o", ConsoleColor.Magenta);
                Print("l", ConsoleColor.DarkMagenta);
                Print("o", ConsoleColor.DarkRed);
                Print("u", ConsoleColor.Red);
                Print("r", ConsoleColor.DarkYellow);
                Console.WriteLine("s", ConsoleColor.Yellow);
                Console.WriteLine("3 - Change Player Names\n4 - Instructions (How to Play)\n5 - Restart Game\n0 - Quit Game");
                try
                {
                    byte Option = GetKey();

                    Console.Clear();
                    switch (Option)
                    {
                        case 1: // Resume the game.
                            Console.WriteLine("Returning to the game\nPress any key...");
                            Console.ReadKey();
                            Console.Clear();
                            return 1;
                        case 2: // Change the playerss colours.
                            Console.Clear();
                            PlayerColourChanger(player1, player2);
                            break;
                        case 3:
                            ChangePlayerName(player1, player2);
                            break;
                        case 4: // View the instructions.
                            Instructions();
                            break;
                        case 5: // Restart the game.
                            Console.WriteLine("Restarting the game\n");
                            Console.Clear();
                            return 5;
                        case 0: // Quit the game.
                            return 0;
                        default:
                            Print("This wasn't an option.", ConsoleColor.Red);
                            Console.WriteLine("Press any key to try again...");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                    }
                }
                catch {Catch(false);}

            }
        }

        // * THE GAME * ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private static int PlayGame(Player player1, Player player2)
        {
            // Used to decide who makes the first move.
            Random random = new Random();

            // Used for when the player is making a move.
            bool player1Turn = true; // True = Player1 Move, False = Player 2 Move.
            if (random.Next(2) == 1) player1Turn = false;

            // * SETTING UP THE GAME * ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            GameEngine.GameMode gameMode = GameEngine.ChooseGameMode(player1, player2); // Get game mode.
            byte numberOfPlayers = GameEngine.ChooseNumberOfPlayers(player1, player2); // Declare and name players.

            // Printing a last message to the user / users playing.
            Console.Clear();
            Print(player1.Name, player1.Colour);
            Console.Write(" vs ");
            Print(player2.Name, player2.Colour);
            Console.WriteLine("!\nPress any key to play...");
            Console.ReadKey();
            Console.Clear();

            // * IN-GAME * ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            Gameboard theGame = new Gameboard();

            while (true)
            {
                Console.Clear();
                Print(player1.Name, player1.Colour);
                Console.Write(" vs ");
                Print(player2.Name, player2.Colour);
                Console.Write("\nIt is ");
                if (player1Turn) Print(player1.Name + "'s", player1.Colour);
                else Print(player2.Name + "'s", player2.Colour);
                Console.WriteLine(" turn...\n");
                // Printing the game board with player instructions.
                theGame.PrintGameBoard();

                // * PLAYER INPUT * ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                byte playerChoice = 0;
                try { playerChoice = GetKey(); }
                catch{Console.WriteLine();}

                Console.Clear();
                if (playerChoice == 0)
                {
                    byte pauseMenuValue = PauseMenu(player1, player2);
                    if (pauseMenuValue == 0) return 0; // Quit the game.
                    else if (pauseMenuValue == 5) break; // Restart the game by breaking out of the while loop.
                }
                else if (player1Turn)
                {
                    player1.MakeMove((byte)playerChoice, theGame, player1, player2);
                    player1Turn = false;
                }
                else if (!player1Turn)
                {
                    player1.MakeMove((byte)playerChoice, theGame, player2, player1);
                    player1Turn = true;
                }
        }
            return 0;
        }

        // * MAIN MENU * ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        static void Main()
        {
            // Initialising the player objects
            Player player1 = new Player("Player 1", ConsoleColor.Red);
            Player player2 = new Player("Player 2", ConsoleColor.Yellow);

            // The game should only end when the player decides so, thus a while loop keeping the game alive is required.
            while (true)
            {
                // The print method is used to change the colour of onscreen text should it be supported.
                // The console needs the ability to change the colour of text to differentiate counters.
                Console.Write("* CONNECT 4 * ");
                Print("O ", player1.Colour);
                Console.Write("| ");
                Print("O ", player1.Colour);
                Console.Write("| ");
                Print("O ", player2.Colour);
                Console.Write("| ");
                Print("O", player2.Colour, null, true);
                Console.WriteLine("Make a selection:");
                Console.WriteLine("1 - Play Game\n2 - Instructions\n3 - Choose Player Colours\n0 - Quit");

                try
                {
                    byte Option = GetKey();
                    Console.Clear();
                    switch (Option)
                    {
                        case 1:
                            int Score = PlayGame(player1, player2);
                            break;
                        case 2:
                            Instructions();
                            break;
                        case 3:
                            PlayerColourChanger(player1, player2);
                            break;
                        case 0:
                            Console.WriteLine("Thank you for playing, press any key to quit!");
                            Console.ReadKey();
                            return;
                    }
                }
                catch(Exception e) {
                    Console.WriteLine(e);
                    ;}
                finally {Console.Clear();}
            }
        }
    }
}
