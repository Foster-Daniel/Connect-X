
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ConnectX
{
    class Program
    {
        // * MENUS & MENU OPTIONS * ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private static void Instructions()
        {
            Console.Clear();
            Console.WriteLine("Welcome to CONNECT 4: Terminal Edition");
            Console.WriteLine("The goal of the game is to match four counters in a row, column or diagonally.");
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
                CX.Print("C", ConsoleColor.Red);
                CX.Print("h", ConsoleColor.DarkYellow);
                CX.Print("a", ConsoleColor.Yellow);
                CX.Print("n", ConsoleColor.Green);
                CX.Print("g", ConsoleColor.Cyan);
                CX.Print("e", ConsoleColor.DarkCyan);
                CX.Print(" C", ConsoleColor.Blue);
                CX.Print("o", ConsoleColor.Magenta);
                CX.Print("l", ConsoleColor.DarkMagenta);
                CX.Print("o", ConsoleColor.DarkRed);
                CX.Print("u", ConsoleColor.Red);
                CX.Print("r", ConsoleColor.DarkYellow);
                Console.WriteLine("s", ConsoleColor.Yellow);
                Console.WriteLine("3 - Change Player Names\n4 - Instructions (How to Play)\n5 - Modify Game\n7 - Restart Game\n0 - Quit Game");
                try
                {
                    byte Option = CX.GetKey();

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
                            Player.ChangeNameChoice(player1, player2);
                            break;
                        case 4: // View the instructions.
                            Instructions();
                            break;
                        case 5:
							Console.WriteLine("You can change the size of the board, change the height of the board and the number of counters needed to win.");
							if (GameEngine.NumberOfPlayers < 2) Console.WriteLine("You can also change the computer difficulty.");
							if (GameEngine.NumberOfPlayers == 2) Console.WriteLine("Modifying the board will end the match, are you sure you want to modify the board?");
							Console.WriteLine((GameEngine.NumberOfPlayers < 2 ? "1 - Modify Game\n" : "1 - Modfiy Board\n") + "2 - Return to Pause Menu");
                            byte answer = CX.GetKey(limit: 2);
                            if (answer == 1) GameEngine.ModifyGame(player1, player2);
                            break;
                        case 6: // Restart the game.
                            Console.WriteLine("Restarting the game\n");
                            Console.Clear();
                            return 6;
                        case 0: // Quit the game.
                            return 0;
                        default:
                            CX.Print("This wasn't an option.", ConsoleColor.Red);
                            Console.WriteLine("Press any key to try again...");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                    }
                }
                catch {CX.Catch(false);}
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
            CX.Print(player1.Name, player1.Colour);
            Console.Write(" vs ");
            CX.Print(player2.Name, player2.Colour);
            Console.WriteLine("!\nPress any key to play...");
            Console.ReadKey();
            Console.Clear();

            // IN-GAME 
            DateTime startingTime = DateTime.Now; // This will be used to show how long a game has been played for.
            while (true)
            {
                // Printing to the terminal who is playing and whose move it is.
                Console.Clear();
                CX.Print(player1.Name, player1.Colour);
                Console.Write(" vs ");
                CX.Print(player2.Name, player2.Colour);
				Console.WriteLine($" | \u001b[4mConnect {Gameboard.VictoryNumber} to win!\u001b[0m");
                Console.Write("\nIt is ");
                CX.Print(activePlayer.Name + "'s", activePlayer.Colour);
                Console.WriteLine(" turn...\n");

                // Printing the game board with player instructions.
                Gameboard.PrintGameBoard();

                // PLAYER INPUT
                if (!activePlayer.IsRobot)
                {
                    byte playerChoice = CX.GetKey(limit: Gameboard.BoardWidth);
                    // Access pause menu.
                    if (playerChoice == 0) // Entering '0' accesses the pause menu.
                    {
                        byte pauseMenuValue = PauseMenu(player1, player2);
                        if (pauseMenuValue == 0) return 0; // Quit the game.
                        else if (pauseMenuValue == 6) Gameboard.ResetBoard(true, player1, player2);
                        continue;
                    }
                    else if (playerChoice == 10 && Gameboard.History.Count > 0)
                    {
                        Gameboard.UndoMove(); // Undo Move... 'CX.GetKey()' Returns 10 when you enter [Z].
                        if (player2.IsRobot) continue;
                    }
                    else if (playerChoice < 10) if (!activePlayer.MakeMove(playerChoice)) continue; // Place a counter on the board. If invalid (false) try again.
                }
                else activePlayer.ComputerMove(activePlayer == player1 ? player2 : player1);
                
                // It makes no sense to check for victory if the player hasn't made enough moves to win.
                if (activePlayer.Moves >= Gameboard.VictoryNumber)
                {
                    winningPlayer = Gameboard.CheckVictory();
                    losingPlayer = winningPlayer != null ? activePlayer == player1 ? player2 : player1 : null;
                }

                // This is the end game screen. This code only runs when a winner has been declared.
                if (winningPlayer != null)
				{
                    Console.Clear();
                    while(true)
					{
                        GameEngine.VictoryMessage(winningPlayer, losingPlayer, startingTime);
 
                        byte answer = CX.GetKey(limit : 2);
                        if (answer == 0) Gameboard.MatchReplay();
                        else if (answer == 1 || answer == 2)
                        {
                            Console.Clear();
                            Gameboard.ResetBoard(true, player1, player2);
                            Gameboard.History.RemoveRange(0, Gameboard.History.Count);
                            Console.WriteLine(answer == 1 ? "Press any key to continue..." : "Thank you for playing\nPress any key to continue...");
                            Console.ReadKey();
                            Console.Clear();
                            winningPlayer = null;
                            losingPlayer = null;
                           if (answer == 1) break;
                           if (answer == 2) return 0;
                        }
                        Console.Clear();
                    }
                }
                activePlayer = activePlayer == player1 ? player2 : player1;
                if (player1.IsRobot && player2.IsRobot) Thread.Sleep(500);
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
                CX.Print("O ", player1.Colour);
                Console.Write("| ");
                CX.Print("O ", player1.Colour);
                Console.Write("| ");
                CX.Print("O ", player2.Colour);
                Console.Write("| ");
                CX.Print("O", player2.Colour, null, true);
                Console.WriteLine("Make a selection:");
                Console.WriteLine("1 - Play Game\n2 - Instructions\n3 - Choose Player Colours\n0 - Quit");

                //try
               // {
                    byte Option = CX.GetKey();
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
