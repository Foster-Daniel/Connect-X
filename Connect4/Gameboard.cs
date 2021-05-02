using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace ConnectX
{
    public class Gameboard
    {
        public enum GameMode : sbyte { Classic, Infinity }
        public static GameMode Mode { get; set; }
        public static List<Counter[]> Rows { get; }
        public static List<(Counter, Player)> History { get; }
        public static byte VictoryNumber { get; set; } = 4;
        public static byte BoardWidth { get; set; } = 9;
        public static byte BoardHeight { get; set; } = 10;

        public static void PrintGameBoard(bool endGame = false)
        {
            for(short i = (short)(Rows.Count - 1); i >= 0; i--)
            {
                Console.Write("|");
                for (int j = 0; j < Rows[i].Length; j++)
                {
                    Rows[i][j].PrintCounter();
                    Console.Write("|");
                }
                Console.WriteLine();
            }
            if(!endGame)
			{
                string divider = "---------------";
                string userInstructions = "|1|2|3|4|5|6|7|";

                for (byte i = 8; i <= BoardWidth; i++)
				{
                    divider += "--";
                    userInstructions += $"{i}|";
				}
                Console.WriteLine(divider);
                Console.WriteLine(userInstructions + " <-- Enter a number to place a counter into the column.");
                if(History.Count < 1) Console.WriteLine("[0] = Pause\n");
                if (History.Count > 0)
                {
                    Console.WriteLine("[0] = Pause");
                    Console.WriteLine("[Z] = Undo Last Move\n");
                }
            }
        }

        public static void CustomiseGame(Player player1, Player player2)
		{
            ResetBoard(true, player1, player2);
            // Delete all board data so that it can be rebuilt easily.
            Rows.RemoveAll(c => true); // Delete All.

            // The VictoryNumber property determines how many counters have to be placed in a row for the game to determine a winner.
            do
            {
                Console.Clear();
                Console.WriteLine("How many consecutive counters would you like for victory?");
                Console.WriteLine("[4] - 4 consecutive counters.");
                Console.WriteLine("[5] - 5 consecutive counters.");
                Console.WriteLine("[6] - 6 consecutive counters.");
                VictoryNumber = CX.GetKey();
            } while (VictoryNumber < 4 || VictoryNumber > 6);

            // The BoardWidth property determines the width of the board.
            do
            {
                Console.Clear();
			    Console.WriteLine("How wide would you like the board to be?");
                // Only showing the relevant options depending on the VictoryNumber.
                if(VictoryNumber == 4) Console.WriteLine("[7] - 7 columns wide.");
                if (VictoryNumber == 4 || VictoryNumber == 5)
                {
                    Console.WriteLine("[8] - 8 columns wide.");
                    Console.WriteLine("[9] - 9 columns wide.");
                }
                if(VictoryNumber < 6) BoardWidth = CX.GetKey();

                // The board is too small if VictoryNumber is 6 and the width is less than 9, because of this the board is automatically set to 9.
                if(VictoryNumber == 5 && BoardWidth < 8) continue;
            } while ((BoardWidth < 6 || BoardWidth > 10) && VictoryNumber < 6);

            // Classic mode has a fixed height so it needs to be set.
            if (Mode == GameMode.Classic)
			{
                if (VictoryNumber < 6)
                    do
                    {
                        Console.Clear();
                        Console.WriteLine("How tall would you like the board to be?");
                        if (VictoryNumber == 4) Console.WriteLine("[7] - 7 columns tall.");
                        if (VictoryNumber == 4 || VictoryNumber == 5)
                        {
                            Console.WriteLine("[8] - 8 columns tall.");
                            Console.WriteLine("[9] - 9 columns tall.");
                        }
                        if (VictoryNumber == 4 || VictoryNumber == 5) Console.WriteLine("[0] - 10 columns tall. (Press 0)");
                        BoardHeight = CX.GetKey();
                        BoardHeight = BoardHeight == 0 ? BoardHeight = 10 : BoardHeight;

                        // Making sure the board isn't too short for the VictoryNumber.
                        if (VictoryNumber == 5 && BoardHeight < 9) continue;

                    } while (BoardHeight < 7 || BoardHeight > 10);
                else BoardHeight = 10;
            }
            Console.Clear();
            SetUpBoard();
        }

        public static void SetUpBoard()
		{
            // Destroying the board to prevent errors when building.
            if (Rows.Count > 0) Rows.RemoveRange(0, Rows.Count);

            // Adding default number of rows and colums to the grid.
            while (Rows.Count < BoardHeight)
                Rows.Add(new Counter[BoardWidth]);

            byte middle = BoardWidth % 2 == 0 ? (byte)(BoardWidth / 2) : (byte)((BoardWidth + 1) / 2);
            middle--;
            byte[] counterValues = new byte[BoardWidth];
			for (byte x = middle, y = BoardWidth % 2 == 0 ? (byte)(middle + 1) : middle; y < counterValues.Length; x--, y++)
			{
                counterValues[x] = x;
                counterValues[y] = x;
			}

            for (byte i = 0; i < Rows.Count; i++)
                for (byte j = 0; j < Rows[i].Length; j++)
                    Rows[i][j] = new Counter((i, j), counterValues[j]);
        }

        public static void ResetBoard(bool hardReset = true, Player player1 = null, Player player2 = null)
		{
            // Removing any claims players might have to all counters on the board.
            for (short i = (short)(Rows.Count - 1); i >= 0; i--)
				foreach (Counter counter in Rows[i])
				{
                    counter.ClaimedBy = null;

                    // Counters lost victory status when the board resets.
                    counter.VictoryCounter = false;
                }
            if(hardReset)
			{
                // Resetting the stats for the players and the board.
                player1.Moves = 0;
                player2.Moves = 0;
                History.RemoveRange(0, History.Count);
			}
        }

        public static void RowDropper()
        {
			for (int i = 0; i < Rows.Count - 1; i++)
				for (int j = 0; j < Rows[i].Length; j++)
                    Rows[i][j].ClaimCounter(Rows[i + 1][j].ClaimedBy);

			foreach (Counter counter in Rows[Rows.Count - 1])
                counter.ClaimCounter(null);
        }

        /// <summary>
        ///     Checks if the most recent counter or any counter passed to the method achieved victory for the user who placed it.
        /// </summary>
        /// <param name="counter">The counter used to determine whether or not the game has been won.</param>
        /// <param name="test">Set to true if you want to test whether a victory would occur if the passed counter was claimed differently</param>
        /// <param name="testPlayer">If in test mode, this player will be who the counter will be temporarily claimed against.</param>
        /// <returns>If there is a win-state on the board from the last counter, it returns the player who placed it as the winner. Otherwise it returns null</returns>
        public static Player CheckVictory(Counter counter = null, bool test = false, Player testPlayer = null)
		{
            Player result = null;
            Player storedPlayer = null;
            counter ??= History[History.Count - 1].Item1;

            if (test)
			{
                storedPlayer = counter.ClaimedBy;
                counter.ClaimedBy = testPlayer;
			}
            // * CHECKING VICTORY BY ROWS *
            result = ScanRow(counter, test);
            if (result != null)
            {
                if (test) counter.ClaimedBy = storedPlayer;
                return result;
            }

            // * CHECKING VICTORY BY COLUMNS *
            result = ScanColumn(counter, test);
            if (result != null)
            {
                if (test) counter.ClaimedBy = storedPlayer;
                return result;
            }

            // * CHECKING VICTORY BY TOP LEFT TO BOTTOM RIGHT *
            result = ScanTopLeftToBottomRight(counter, test);
            if (result != null)
            {
                if (test) counter.ClaimedBy = storedPlayer;
                return result;
            }

            // * CHECKING VICTORY BY BOTTOM LEFT TO TOP RIGHT *
            result = ScanBottomLeftToTopRight(counter, test);
            if (result != null)
            {
                if (test) counter.ClaimedBy = storedPlayer;
                return result;
            }
            
            if (test) counter.ClaimedBy = storedPlayer;
            return null;
        }

        private static Player ScanRow(Counter counter, bool test)
        {
            byte coordinate = counter.Coordinates.Item2;
            Counter[] row = Rows[counter.Coordinates.Item1];
            Player checkedPlayer = counter.ClaimedBy;

            // This variable is used to count the number of consecutive counters in any direction.
            byte countersInARow = 0;

            for (int i = coordinate - VictoryNumber; i <= coordinate + VictoryNumber; i++)
            {
                // Preventing out of bounds exceptions.
                if (i < 0) i = 0;
                if (i >= row.Length) break;

                countersInARow = (byte)(row[i].ClaimedBy == checkedPlayer? ++countersInARow : 0);
                if (countersInARow >= VictoryNumber)
                {
                    if (!test)
                    {
                        byte reference = row[i].Coordinates.Item2;
                        // Used for changing the status of counters to victory counters.
                        for (int j = reference; j < reference - VictoryNumber - 1; j--)
                            row[j].VictoryCounter = true;
                    }
                    return checkedPlayer;
                }
            }
            return null;
		}
        private static Player ScanColumn(Counter counter, bool test)
		{
            Player player = counter.ClaimedBy;
            byte countersInAColumn = 0;
            byte coordinate = counter.Coordinates.Item1;
            byte column = counter.Coordinates.Item2;
			for (int i = coordinate; i > coordinate - VictoryNumber && i > -1; i--)
			{
                if (Rows[i][column].ClaimedBy == player) countersInAColumn++;
                else return null;

                if(countersInAColumn >= VictoryNumber)
				{
                    if (!test)
                        for (int j = coordinate; j < coordinate - VictoryNumber; j--)
                            Rows[j][column].VictoryCounter = true;

                    return player;
				}
			}
            return null;
        }
        private static Player ScanTopLeftToBottomRight(Counter counter, bool test)
		{
            Player player = counter.ClaimedBy;
            byte recurringCounters = 0;
            (byte, byte) coordinates = counter.Coordinates;

			for (int i = coordinates.Item1 + VictoryNumber, j = coordinates.Item2 - VictoryNumber; j <= coordinates.Item2 + VictoryNumber; i--, j++)
			{
                // Protecting against out of bounds exceptions.
                int difference = 0;
                if (i < 0 || j >= BoardWidth) break;
                if (i >= Rows.Count)
                {
                    difference = Rows.Count - i + 1; // We add one here because if 'i' == 'Rows.Count' it is still out of bounds by one.
                    i = Rows.Count - 1;
                    j += difference; // Adjusting 'j' so that the computer still scans on the correct diagonal.
                }
                if (j < 0)
                {
                    difference = Math.Abs(j);
                    j = 0;
                    i -= difference; // Readjusting 'i' so that the computer still scans on the correct diagonal.
                }
                recurringCounters = (byte)(Rows[i][j].ClaimedBy == player ? ++recurringCounters : 0);
                if (recurringCounters >= VictoryNumber)
				{
                    if (!test)
                    {
                        (byte, byte) reference = ((byte)i, (byte)j);
                        for (int x = reference.Item1, y = reference.Item2; x < reference.Item1 + VictoryNumber; x++, y--)
                            Rows[x][y].VictoryCounter = true;
                    }
                    return player;
				}
            }
            return null;
		}
        private static Player ScanBottomLeftToTopRight(Counter counter, bool test)
		{
            Player player = counter.ClaimedBy;
            byte recurringCounters = 0;
            (byte, byte) coordinates = (counter.Coordinates.Item1, counter.Coordinates.Item2);

			for (int i = coordinates.Item1 - VictoryNumber, j = coordinates.Item2 - VictoryNumber; j <= coordinates.Item2 + VictoryNumber; i++, j++)
			{
                if (i >= Rows.Count || j >= BoardWidth) break;
                int difference = 0;
                if(i < 0)
				{
                    difference = i;
                    i = 0;
                    j -= difference;
				}
                if(j < 0)
				{
                    difference = j;
                    j = 0;
                    i -= difference;
				}

                recurringCounters = (byte)(Rows[i][j].ClaimedBy == player ? ++recurringCounters : 0);
                if(recurringCounters >= VictoryNumber)
			    {
                    if (!test)
                    {
                        (byte, byte) reference = Rows[i][j].Coordinates;
                        for (int x = reference.Item1, y = reference.Item2; x > reference.Item1 - VictoryNumber; x--, y--)
                            Rows[x][y].VictoryCounter = true;
                    }
                    return player;
                }
            }
            return null;
        }
        
        public static void UndoMove()
		{
            if (History.Count > 0)
			{
                History[History.Count - 1].Item2.Moves--;
                History[History.Count - 1].Item1.ClaimedBy = null;
                History.RemoveAt(History.Count - 1);
                if (GameEngine.NumberOfPlayers == 1 && History.Count > 0)
                {
                    History[History.Count - 1].Item2.Moves--;
                    History[History.Count - 1].Item1.ClaimedBy = null;
                    History.RemoveAt(History.Count - 1);
                }
			}
		}

        public static void MatchReplay()
        {
            ResetBoard(false);
            for(short i = 0; i < History.Count; i++)
            {
                Console.Clear();
                PrintGameBoard(true);
                History[i].Item1.ClaimedBy = History[i].Item2;
                History[i].Item1.Marker = History[i].Item2.Marker;
                Thread.Sleep(500);
            }
            CheckVictory(History[History.Count - 1].Item1);
        }

        static Gameboard()
        {
            // Declaring properties for the class.
            History = new List<(Counter, Player)>();
            VictoryNumber = 4;
            Rows = new List<Counter[]>();
        }
    }

}
