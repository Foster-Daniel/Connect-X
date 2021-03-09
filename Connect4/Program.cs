
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace ConnectX
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
        public static string Read(ConsoleColor? foregroundColour = null, ConsoleColor? backgroundColour = null)
        {
            if (foregroundColour != null) Console.ForegroundColor = (ConsoleColor)foregroundColour;
            if (backgroundColour != null) Console.BackgroundColor = (ConsoleColor)backgroundColour;

            // Getting the user input in a string form as it is the leat likely to provide errors.
            string input = Console.ReadLine();
            Console.ResetColor();
            return input;

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

        public static byte GetKey(byte limit = 9, string message = "\nPlease only choose from the given options....", bool menu = false)
		{
            ConsoleKey playerChoice = ConsoleKey.D0;
            while(true)
	    	{
                try
                {
                    playerChoice = Console.ReadKey().Key;
                }
				catch (Exception e) { Console.WriteLine(e); }
                switch (playerChoice)
			    {
                    case ConsoleKey.Escape:
                        return !menu ? 0 : 1;
                    case ConsoleKey.P:
                        return !menu ? 0 : 1;
                    case ConsoleKey.D0:
                        return 0;
                    case ConsoleKey.D1:
                        if (limit < 1) break;
                        return 1;
                    case ConsoleKey.D2:
                        if (limit < 2) break;
                        return 2;
                    case ConsoleKey.D3:
                        if (limit < 3) break;
                        return 3;
                    case ConsoleKey.D4:
                        if (limit < 4) break;
                        return 4;
                    case ConsoleKey.D5:
                        if (limit < 5) break;
                        return 5;
                    case ConsoleKey.D6:
                        if (limit < 6) break;
                        return 6;
                    case ConsoleKey.D7:
                        if (limit < 7) break;
                        return 7;
                    case ConsoleKey.D8:
                        if (limit < 8) break;
                        return 8;
                    case ConsoleKey.D9:
                        if (limit < 9) break;
                        return 9;
                    case ConsoleKey.Z:
                        if (menu) break;
                        return 10;
                }
                Console.WriteLine(message);
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
                Console.WriteLine("3 - Change Player Names\n4 - Instructions (How to Play)\n5 - Modify Board\n6 - Restart Game\n0 - Quit Game");
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
                            Player.ColourChanger(player1, player2);
                            break;
                        case 3:
                            Player.ChangeName(player1, player2);
                            break;
                        case 4: // View the instructions.
                            Instructions();
                            break;
                        case 5:
							Console.WriteLine("You can change the size of the board, change the height of the board and the number of counters needed to win...");
							Console.WriteLine("Modifying the board will end the match, are you sure you want to modify the board?");
							Console.WriteLine("1 - Modfiy Game\n2 - Return to Pause Menu");
                            byte answer = GetKey(limit: 2);
                            if (answer == 1)
                            {
                                Gameboard.ResetBoard(hardReset: true, player1, player2);
                                Gameboard.CustomiseGame();
                            }
                            break;
                        case 6: // Restart the game.
                            Console.WriteLine("Restarting the game\n");
                            Console.Clear();
                            return 6;
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
            // Used to decide who makes the first move. It is only used once in the next statement.
            Random random = new Random();
            Player activePlayer = random.Next(2) == 0 ? player1 : player2;
            

            // These are used for declaring a winner and displaying information at the end of the game.
            Player winningPlayer = null;
            Player losingPlayer = null;


            // Setting up the game.
            Gameboard.Mode = GameEngine.ChooseGameMode(player1, player2); // Get game mode.
            GameEngine.ChooseNumberOfPlayers(player1, player2); // Declare and name players.
            Gameboard.SetUpBoard();
            
            
            // Printing a last message to the user / users playing.
            Console.Clear();
            Print(player1.Name, player1.Colour);
            Console.Write(" vs ");
            Print(player2.Name, player2.Colour);
            Console.WriteLine("!\nPress any key to play...");
            Console.ReadKey();
            Console.Clear();


            // IN-GAME 
            DateTime startingTime = DateTime.Now; // This will be used to show how long a game has been played for.
            while (true)
            {
                // Printing to the terminal who is playing and whose move it is.
                Console.Clear();
                Print(player1.Name, player1.Colour);
                Console.Write(" vs ");
                Print(player2.Name, player2.Colour);
				Console.WriteLine($" | Connect {Gameboard.VictoryNumber} to win!");
                Console.Write("\nIt is ");
                Print(activePlayer.Name + "'s", activePlayer.Colour);
                Console.WriteLine(" turn...\n");


                // Printing the game board with player instructions.
                Gameboard.PrintGameBoard();


                // PLAYER INPUT
                if (!activePlayer.IsRobot)
                {
                    //Receiving input from the user.
                    byte playerChoice = 0;
                    playerChoice = GetKey(limit: Gameboard.BoardWidth);


                    // Access pause menu.
                    if (playerChoice == 0) // Entering '0' accesses the pause menu.
                    {
                        byte pauseMenuValue = PauseMenu(player1, player2);
                        if (pauseMenuValue == 0) return 0; // Quit the game.
                        else if (pauseMenuValue == 6) Gameboard.ResetBoard(true, player1, player2);
                        continue;
                    }
                    else if (playerChoice == 10 && Gameboard.History.Count > 0) Gameboard.UndoMove(activePlayer); // Undo Move... 'GetKey()' Returns 10 when you enter [Z].
                    else if (playerChoice < 10) activePlayer.MakeMove(playerChoice); // Place a counter on the board.
                }
                else activePlayer.ComputerMove(activePlayer == player1 ? player2 : player1);
                
                // It makes no sense to check for victory if the player hasn't made enough moves to win.
                if (activePlayer.Moves >= Gameboard.VictoryNumber)
                {
                    winningPlayer = Gameboard.CheckCounter(Gameboard.History[Gameboard.History.Count - 1].Item1).Item1;
                    losingPlayer = winningPlayer != null ? activePlayer == player1 ? player2 : player1 : null;
                }

                // This is the end game screen. This code only runs when a winner has been declared.
                if (winningPlayer != null)
				{
                    Console.Clear();
                    while(true)
					{
                        GameEngine.VictoryMessage(winningPlayer, losingPlayer, startingTime);
 
                        byte answer = GetKey();
                        if (answer == 1)
                        {
                            Console.Clear();
                            Gameboard.ResetBoard(true, player1, player2);
                            Gameboard.History.RemoveRange(0, Gameboard.History.Count);
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                        }
                        else if (answer == 2)
                        {
                            Console.Clear();
                            Gameboard.ResetBoard(true, player1, player2);
                            Gameboard.History.RemoveRange(0, Gameboard.History.Count);
                            Console.WriteLine("Thank you for playing\nPress any key to continue...");
                            Console.ReadKey();
                            player1.Moves = 0;
                            player2.Moves = 0;
                            Console.Clear();
                            startingTime = DateTime.Now;
                            return 0;
                        }
                        else if (answer == 3) Gameboard.MatchReplay();
                        Console.Clear();
                    }
                }
                activePlayer = activePlayer == player1 ? player2 : player1;
            }
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
                Console.Write("* CONNECT X * ");
                Print("O ", player1.Colour);
                Console.Write("| ");
                Print("O ", player1.Colour);
                Console.Write("| ");
                Print("O ", player2.Colour);
                Console.Write("| ");
                Print("O", player2.Colour, null, true);
                Console.WriteLine("Make a selection:");
                Console.WriteLine("1 - Play Game\n2 - Instructions\n3 - Choose Player Colours\n0 - Quit");

                //try
               // {
                    byte Option = GetKey();
                    Console.Clear();
                    switch (Option)
                    {
                        case 1:
                            int score = PlayGame(player1, player2);
                            break;
                        case 2:
                            Instructions();
                            break;
                        case 3:
                            Player.ColourChanger(player1, player2);
                            break;
                        case 0:
                            Console.WriteLine("Thank you for playing, press any key to quit!");
                            Console.ReadKey();
                            return;
                    }
                //}
                //catch(Exception e) {
                //    Console.WriteLine(e);
                //    ;}
                //finally {Console.Clear();}
            }
        }
    }
}
