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
        public static List<Counter[]> Row { get; }
        public static List<(Counter, Player)> History { get; }
        public static byte VictoryNumber { get; set; } = 4;
        public static byte BoardWidth { get; set; } = 9;
        public static byte BoardHeight { get; set; } = 10;

        public static void PrintGameBoard(bool endGame = false)
        {
            for(short i = (short)(Row.Count - 1); i >= 0; i--)
            {
                Console.Write("|");
                for (int j = 0; j < Row[i].Length; j++)
                {
                    Row[i][j].PrintCounter();
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

        public static void CustomiseGame()
		{
            // Delete all board data so that it can be rebuilt easily.
            Row.RemoveAll(c => c == c); // Delete All.


            // The VictoryNumber property determines how many counters have to be placed in a row for the game to determine a winner.
            do
            {
                Console.Clear();
                Console.WriteLine("How many consecutive counters would you like for victory?");
                Console.WriteLine("[4] - 4 consecutive counters.");
                Console.WriteLine("[5] - 5 consecutive counters.");
                Console.WriteLine("[6] - 6 consecutive counters.");
                VictoryNumber = Program.GetKey();
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
                if(VictoryNumber < 6) BoardWidth = Program.GetKey();

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
                        BoardHeight = Program.GetKey();
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
            if (Row.Count > 0) Row.RemoveRange(0, Row.Count);

            // Adding default number of rows and colums to the grid.
            while (Row.Count < BoardHeight)
                Row.Add(new Counter[BoardWidth]);

            sbyte middle = BoardWidth % 2 == 0 ? (sbyte)(BoardWidth / 2) : (sbyte)((BoardWidth + 1) / 2);
            middle--;
            sbyte[] counterValues = new sbyte[BoardWidth];
			for (sbyte x = middle, y = BoardWidth % 2 == 0 ? (sbyte)(middle + 1) : middle, z = 2; y < counterValues.Length; x--, y++, z--)
			{
                counterValues[x] = z;
                counterValues[y] = z;
			}

            for (byte i = 0; i < Row.Count; i++)
                for (byte j = 0; j < Row[i].Length; j++)
                    Row[i][j] = new Counter((i, j), counterValues[j]);
        }

        public static void ResetBoard(bool hardReset = true, Player player1 = null, Player player2 = null)
		{
            // Removing any claims players might have to all counters on the board.
            for (short i = (short)(Row.Count - 1); i >= 0; i--)
				foreach (Counter counter in Row[i])
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

        /// <summary>
        ///     Check the board for victory or potential moves the AI can make.
        /// </summary>
        /// <param name="checkedPlayer">The player who's counters will be checked.</param>
        /// <param name="victory">Determines whether or not to activate Victory Counters.</param>
        /// <param name="countersNeeded">Determines how many counters are to be checked.</param>
        /// <param name="latestMove">Pass moves to check against.</param>
        /// <returns>The player checked against for determining a winner and possible moves the AI can make.</returns>
        public static (Player, sbyte) CheckCounter(Counter counter = null)
		{
            counter ??= History[History.Count - 1].Item1;


            /* By default the variable used will match with the boards own definition for the number of counters needed to win.
                However, the code allows alternative values to be passed to the method. The following line of code checks to see
                if 'countersNeeded' has a valid value and if it doesn't it converts it to 'VictoryNumber'. */
            /*
            // * CHECKING VICTORY BY ROWS *
            (Player, sbyte) result = (null, 0);

            result = ScanRow(counter);
            if (result != (null, 0)) return result;

            // * CHECKING VICTORY BY COLUMNS *
            result = ScanColumn(counter);
            if (result != (null, 0)) return result;

            // * CHECKING VICTORY BY TOP LEFT TO BOTTOM RIGHT *
            result = ScanTopLeftToBottomRight(counter);
            if (result != (null, 0)) return result;

            // * CHECKING VICTORY BY BOTTOM LEFT TO TOP RIGHT *
            result = ScanBottomLeftToTopRight(counter);
            if (result != (null, 0)) return result;*/
            return (null, 0);
        }
        /*
        private static (Player, sbyte) ScanRow(Counter counter)
		{
            byte coordinate = counter.Coordinates.Item2;
            Counter[] row = Row[counter.Coordinates.Item1];

            // This variable is used to count the number of consecutive counters in any direction.
            byte countersInARow = 0;
            sbyte counterValue = 0;

            /* It is possible for a player to get a victory by putting a counter in the missing gap as shown: | O | O |  | O |
                assuming the VictoryNumber is 4. Because of this, the code needs to check for an empty space for the
                opposing player to put a counter. There can be 0 and 1 but not 2 unclaimed counters when scanning. *//*
            List<Counter> unclaimedCounters = new List<Counter>();

			for (int i = coordinate - VictoryNumber; i <= coordinate + VictoryNumber; i++)
			{
                // Preventing out of bounds exceptions.
                if (i < 0) i = 0;
                if (i >= row.Length) break;

                if (row[i].ClaimedBy == checkedPlayer) countersInARow++;
                else if (row[i].ClaimedBy == null && countersInARow > 0)
                {
                    unclaimedCounters.Add(row[i]);
                    if (unclaimedCounters.Count > 1)
                    {
                        // Protecting against errors when there are more than one unclaimed counters.
                        if (row[i - 1].ClaimedBy == checkedPlayer) countersInARow = (byte)(i -unclaimedCounters[unclaimedCounters.Count - 2].Coordinates.Item2 - 1);
                        unclaimedCounters.RemoveRange(0, unclaimedCounters.Count);
                    }
                }
                else
                {
                    countersInARow = 0;
                    unclaimedCounters.RemoveRange(0, unclaimedCounters.Count);
                }

                if (countersInARow >= VictoryNumber)
                {
                    byte reference = row[i].Coordinates.Item2;
                    // Used for chaging the status of counters to victory counters.
                    if(victory)
                    {
                        if (unclaimedCounters.Count > 0) return (null, 0);
                        for (int j = reference; j < reference - VictoryNumber - 1; j--)
                            row[j].VictoryCounter = true;
                    }
                    return (checkedPlayer, checkedCounter, counterValue);
                }
            }
            counterValue = 10 - (VictoryNumber - countersInARow);
            return (checkedPlayer, checkedCounter, counterValue);
		}
        private static (Player, Counter, Counter, Counter) ScanColumn(Counter counter)
		{
            Counter counter = null;
            byte countersInAColumn = 0;
            byte coordinate = History[History.Count - 1].Item1.Coordinates.Item1;
			for (int i = coordinate; i > coordinate - countersNeeded && i > -1; i--)
			{
                if (Row[i][column].ClaimedBy == checkedPlayer) countersInAColumn++;
                else return (null, null, null, null);

                if(countersInAColumn >= countersNeeded)
				{
                    if (i + 1 < Row.Count && countersNeeded != VictoryNumber) counter = Row[i + countersNeeded][column].ValidateCounter();
					else
                        for (int j = coordinate; j < coordinate - countersNeeded; j--)
                            Row[j][column].VictoryCounter = true;
                    return (checkedPlayer, null, null, counter);
				}
			}
            return (null, null, null, null);
        }
        private static (Player, Counter, Counter, Counter) ScanTopLeftToBottomRight(Counter counter)
		{
            Counter firstCounter = null, middleCounter = null, lastCounter = null;
            byte recurringCounters = 0;
            List<Counter> unclaimedCounters = new List<Counter>();
            (byte, byte) coordinates = latestMove.Coordinates;

			for (int i = coordinates.Item1 + countersNeeded, j = coordinates.Item2 - countersNeeded; j <= coordinates.Item2 + countersNeeded; i--, j++)
			{
                int difference = 0;
                // Protecting against out of bounds exceptions.
                if (i >= Row.Count)
                {
                    // We add one here because if 'i' == 'Row.Count' it is still out of bounds by one.
                    difference = Row.Count - i + 1;
                    i = Row.Count - 1;

                    // Adjusting 'j' so that the computer still scans on the correct diagonal.
                    j += difference;
                }
                else if (i < 0) break;
                if (j < 0)
                {
                    difference = Math.Abs(j);
                    j = 0;

                    // Readjusting 'i' so that the computer still scans on the correct diagonal.
                    i -= difference;
                }
                else if (j >= BoardWidth) break;


                if (Row[i][j].ClaimedBy == checkedPlayer) recurringCounters++;
                else if (Row[i][j].ClaimedBy == null && recurringCounters > 0)
                {
                    unclaimedCounters.Add(Row[i][j]);
                    if (unclaimedCounters.Count > 1)
                    {
                        // Protecting against errors when there are more than one unclaimed counters.
                        // Calculates how many recurring counters there have been since the last unclaimed counter.
                        if (Row[i + 1][j - 1].ClaimedBy == checkedPlayer) recurringCounters = (byte)(i - unclaimedCounters[unclaimedCounters.Count - 2].Coordinates.Item2 - 1);
                        unclaimedCounters.RemoveRange(0, unclaimedCounters.Count);
                    }
                }
                else
                {
                    recurringCounters = 0;
                    unclaimedCounters.RemoveRange(0, unclaimedCounters.Count);
                }
                if (recurringCounters >= countersNeeded)
				{
                    (byte, byte) reference = ((byte)i, (byte)j);
                    if(recurringCounters == VictoryNumber)
					{
                        if (unclaimedCounters.Count > 0) return (null, null, null, null);
						for (int x = reference.Item1, y = reference.Item2; x < reference.Item1 + countersNeeded; x++, y--)
                            Row[x][y].VictoryCounter = true;
					}
                    else
					{
                        if (unclaimedCounters.Count > 0)
							for (int x = reference.Item1, y = reference.Item2; x < reference.Item1 + countersNeeded; x++, y--)
							{
                                middleCounter = Row[x][y].ClaimedBy == null ? Row[x][y].ValidateCounter() : null;
                                if (middleCounter != null) return (checkedPlayer, null, middleCounter, null);
							}
                        else
						{
                            if (reference.Item1 + countersNeeded < Row.Count && reference.Item2 - countersNeeded > -1) firstCounter = Row[reference.Item1 + countersNeeded][reference.Item2 - countersNeeded].ValidateCounter();
                            if (reference.Item1 - 1 > -1 && reference.Item2 + 1 < BoardWidth) lastCounter = Row[reference.Item1 - 1][reference.Item2 + 1].ValidateCounter();
                            return (checkedPlayer, firstCounter, middleCounter, lastCounter);
						}
					}
				}
            }
            return (null, null, null, null);
		}
        private static (Player, Counter, Counter, Counter) ScanBottomLeftToTopRight(Counter counter)
		{
            Counter firstCounter = null, middleCounter = null, lastCounter = null;
            byte recurringCounters = 0;
            List<Counter> unclaimedCounters = new List<Counter>();
            (byte, byte) coordinates = (latestCounter.Coordinates.Item1, latestCounter.Coordinates.Item2);

			for (int i = coordinates.Item1 - countersNeeded, j = coordinates.Item2 - countersNeeded; j <= coordinates.Item2 + countersNeeded; i++, j++)
			{
                if (i >= Row.Count || j >= BoardWidth) break;

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

                if (Row[i][j].ClaimedBy == checkedPlayer) recurringCounters++;
                else if (Row[i][j].ClaimedBy == null && recurringCounters > 0)
				{
                    unclaimedCounters.Add(Row[i][j]);
                    if (unclaimedCounters.Count > 1)
                    {
                        // Protecting against errors when there are more than one unclaimed counters.
                        // Calculates how many recurring counters there have been since the last unclaimed counter.
                        if (Row[i - 1][j - 1].ClaimedBy == checkedPlayer) recurringCounters = (byte)(i - unclaimedCounters[unclaimedCounters.Count - 2].Coordinates.Item2 - 1);
                        unclaimedCounters.RemoveRange(0, unclaimedCounters.Count);
                    }
                }
                else
                {
                    recurringCounters = 0;
                    unclaimedCounters.RemoveRange(0, unclaimedCounters.Count);
                }
                if(recurringCounters >= countersNeeded)
			    {
                    (byte, byte) reference = Row[i][j].Coordinates;
                    if (countersNeeded == VictoryNumber)
					{
                        // Victory cannot take place if there is an unclaimed counter.
                        if (unclaimedCounters.Count > 0) return (null, null, null, null);

                        for (int x = reference.Item1, y = reference.Item2; x > reference.Item1 - countersNeeded; x--, y--)
                            Row[x][y].VictoryCounter = true;
                    }

                    else
                    {
                        if (unclaimedCounters.Count > 0)
                            for (int x = reference.Item1, y = reference.Item2; x > reference.Item1 - countersNeeded; x--, y--)
                            {
                                middleCounter = Row[x][y].ClaimedBy == null ? Row[x][y].ValidateCounter() : null;
                                if (middleCounter != null) return (checkedPlayer, null, middleCounter, null);
                            }
                        else
                        {
                            if (reference.Item1 - countersNeeded > -1 && reference.Item2 - countersNeeded > -1) firstCounter = Row[reference.Item1 - countersNeeded][reference.Item2 - countersNeeded].ValidateCounter();
                            if (reference.Item1 + 1 < Row.Count && reference.Item2 + 1 < BoardWidth) lastCounter = Row[reference.Item1 + 1][reference.Item2 + 1].ValidateCounter();
                            return (checkedPlayer, firstCounter, middleCounter, lastCounter);
                        }
                    }
                }
            }
            return (null, null, null, null);
        }
        */
        public static void UndoMove(Player player)
		{  
            History[History.Count - 1].Item1.ClaimedBy = null;
            History.RemoveAt(History.Count - 1);
            player.Moves--;
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
            CheckCounter(History[History.Count - 1].Item1);
        }

        static Gameboard()
        {
            // Declaring properties for the class.
            History = new List<(Counter, Player)>();
            VictoryNumber = 4;
            Row = new List<Counter[]>();
        }
    }

}
