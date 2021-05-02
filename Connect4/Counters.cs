using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Linq;

namespace ConnectX
{
    public class Counter
    {
		public bool VictoryCounter { get; set; } = false;
        public char Marker { get; set; } = '_';
		public Player ClaimedBy { get; set; }
		public (byte, byte) Coordinates { get; set; }

        /* Counters in the middle of the board are more likely to yield a better tactical position
            because they are more likely to be a part of one or more set fo consecutive counters. */ 
        public byte PositionValue { get; set; } = 0;
        public sbyte StrategicValue { get; set; } = 0;
        public short GetValue() => (short)(PositionValue + StrategicValue);

		public void ClaimCounter(Player chosenPlayer)
		{
            ClaimedBy = chosenPlayer;
            Marker = chosenPlayer != null ? chosenPlayer.Marker : ' ';
		}

        public void PrintCounter()
        {
            if (ClaimedBy == null)
            {
                Marker = ' ';
                CX.Print($"\u001b[4m{Marker}\u001b[0m", null);
            }
            else
            {
                Marker = ClaimedBy.Marker;
                if (!VictoryCounter) CX.Print($"\u001b[4m{Marker}\u001b[0m", ClaimedBy.Colour);
                else CX.Print($"\u001b[4m{Marker}\u001b[0m", null, ClaimedBy.Colour);
            }
        }

        // Consider removing, it isn't being used for now.
        public Counter ValidateCounter()
		{
            // The computer cannot steal already claimed counters.
            if (ClaimedBy != null) return null;

            // Make sure the counter has counters below it if it is off the ground.
			for (int i = 0; i < Coordinates.Item1; i++)
                if (Gameboard.Rows[i][Coordinates.Item2].ClaimedBy == null) return null;
            return this;
		}

        public sbyte EvaluateValue(Player activePlayer, Player opponent)
        {
            if (ClaimedBy != null) return 0;
            sbyte futureStuff = 0;
            if (Coordinates.Item1 < Gameboard.Rows.Count - 1)
            {
                if (Gameboard.CheckVictory(Gameboard.Rows[(byte)(Coordinates.Item1 + 1)][Coordinates.Item2], true, opponent) == opponent) futureStuff = -50;
                else if (Gameboard.CheckVictory(Gameboard.Rows[(byte)(Coordinates.Item1 + 1)][Coordinates.Item2], true, opponent) == activePlayer) futureStuff = 5;
            }

            // If a victory can be had by either lpayer in the next move. Claim this counter.
            if (Gameboard.CheckVictory(this, true, activePlayer) == activePlayer)
            {
                StrategicValue = 125;
                return StrategicValue;
            }
            if (Gameboard.CheckVictory(this, true, opponent) == opponent)
            {
                if (futureStuff == -50) StrategicValue = 50;
                else if (futureStuff == 5) StrategicValue = 100;
                else StrategicValue = 75;
                return StrategicValue;
            }
            byte[] values = new byte[] {
                EvaluateRow(activePlayer),
                EvaluateColumn(),
                EvaluateBottomLeftToTopRight(activePlayer),
                EvaluateTopLeftToBottomRight(activePlayer),
            };

            byte totalValue = 0;
			for (int i = 0; i < values.Length; i++)
			{
                if (values[i] != values.Max()) values[i] /= 2;
                totalValue += values[i];
			}

            StrategicValue = (sbyte)((totalValue * 2) + futureStuff);
            return StrategicValue;
        }
        private byte EvaluateRow(Player activePlayer)
		{
            Counter[] row = Gameboard.Rows[Coordinates.Item1];
            Player backwardPlayer = null;
            Player forwardPlayer = null;
            if (Coordinates.Item2 - 1 > -1) backwardPlayer = row[Coordinates.Item2 - 1].ClaimedBy;
            if (Coordinates.Item2 + 1 < Gameboard.BoardWidth) forwardPlayer = row[Coordinates.Item2 + 1].ClaimedBy;
            bool playersEqual = backwardPlayer == forwardPlayer;
            byte backwardsInARow = 0, forwardsInARow = 0;
            bool backward = backwardPlayer != null, forward = forwardPlayer != null;
			for (int i = Coordinates.Item2; i < Coordinates.Item2 + Gameboard.VictoryNumber - 2; i++)
			{
                // Protecting from out of bounds exceptions.
                if (i - 2 < 0) backward = false;
                if (i + 2 >= Gameboard.BoardWidth) forward = false;

                if (backward && row[i - 2].ClaimedBy == backwardPlayer) backwardsInARow++;
                else if (backward && row[i - 2].ClaimedBy != backwardPlayer && row[i - 2].ClaimedBy != null) backward = false; // If claimed by opposing player.
                if (forward && row[i + 2].ClaimedBy == forwardPlayer) forwardsInARow++;
                else if (forward && row[i + 2].ClaimedBy != forwardPlayer && row[i + 2].ClaimedBy != null) forward = false; // If claimed by opposing player.
                if (!backward && !forward) break;
            };
            if (playersEqual) return (byte)(backwardsInARow + forwardsInARow);
			else
			{
                if (activePlayer == backwardPlayer) return backwardsInARow >= forwardsInARow ? backwardsInARow : forwardsInARow;
                else return forwardsInARow >= backwardsInARow ? forwardsInARow : backwardsInARow;
            }
		}
        private byte EvaluateColumn()
        {
            if (Coordinates.Item1 == 0) return 1;
            byte column = Coordinates.Item2;
            Player checkedPlayer = Gameboard.Rows[Coordinates.Item1 - 1][column].ClaimedBy;
            byte countersInAColumn = 0;
            for (int i = Coordinates.Item1; i > Coordinates.Item1 - Gameboard.VictoryNumber + 1; i--)
			{
                if (i < 0) break;
                if (Gameboard.Rows[i][column].ClaimedBy == checkedPlayer) countersInAColumn++;
            }
            return countersInAColumn;
		}
        private byte EvaluateTopLeftToBottomRight(Player activePlayer)
		{
            Player backwardPlayer = null;
            Player forwardPlayer = null;
            if (Coordinates.Item1 + 1 < Gameboard.Rows.Count && !(Coordinates.Item2 - 1 < 0)) backwardPlayer = Gameboard.Rows[Coordinates.Item1 + 1][Coordinates.Item2 - 1].ClaimedBy;
            if (!(Coordinates.Item1 - 1 < 0) && Coordinates.Item2 + 1 < Gameboard.BoardWidth) forwardPlayer = Gameboard.Rows[Coordinates.Item1 - 1][Coordinates.Item2 + 1].ClaimedBy;
            bool playersEqual = backwardPlayer == forwardPlayer;
            byte backwardsRecurring = 0, forwardsRecurring = 0;
            bool backward = backwardPlayer != null, forward = forwardPlayer != null;

            for (int i = Coordinates.Item1, j = Coordinates.Item2; j < Coordinates.Item2 + Gameboard.VictoryNumber - 1; i++, j++)
			{
                // Protecting from out of bounds exceptions.
                if (i + 2 >= Gameboard.Rows.Count || j - 2 < 0) backward = false;
                if (i - 2 < 0 || j + 2 >= Gameboard.BoardWidth) forward = false;

                if (backward && Gameboard.Rows[i + 2][j - 2].ClaimedBy == backwardPlayer) backwardsRecurring++;
                else if (backward && Gameboard.Rows[i + 2][j - 2].ClaimedBy != backwardPlayer && Gameboard.Rows[i + 2][j - 2].ClaimedBy != null) backward = false; // If claimed by opposing player.
                if (forward && Gameboard.Rows[i - 2][j + 2].ClaimedBy == forwardPlayer) forwardsRecurring++;
                else if (forward && Gameboard.Rows[i - 2][j + 2].ClaimedBy != forwardPlayer && Gameboard.Rows[i - 2][j + 2].ClaimedBy != null) forward = false; // If claimed by opposing player.
                if (!backward && !forward) break;
            }
            if (playersEqual) return (byte)(backwardsRecurring + forwardsRecurring);
            else
            {
                if (activePlayer == backwardPlayer) return backwardsRecurring >= forwardsRecurring ? backwardsRecurring : forwardsRecurring;
                else return forwardsRecurring >= backwardsRecurring ? forwardsRecurring : backwardsRecurring;
            }
        }
        private byte EvaluateBottomLeftToTopRight(Player activePlayer)
		{
            Player forwardPlayer = null;
            Player backwardPlayer = null;
            if (!(Coordinates.Item1 - 1 < 0) && !(Coordinates.Item2 - 1 < 0)) backwardPlayer = Gameboard.Rows[Coordinates.Item1 - 1][Coordinates.Item2 - 1].ClaimedBy;
            if (Coordinates.Item1 + 1 < Gameboard.Rows.Count && Coordinates.Item2 + 1 < Gameboard.BoardWidth) forwardPlayer = Gameboard.Rows[Coordinates.Item1 + 1][Coordinates.Item2 + 1].ClaimedBy;
            bool playersEqual = backwardPlayer == forwardPlayer;
            byte backwardsRecurring = 0, forwardsRecurring = 0;
            bool backward = true, forward = true;

            for (int i = Coordinates.Item1, j = Coordinates.Item2; j < Coordinates.Item2 + Gameboard.VictoryNumber - 2; i++, j++)
            {
                // Protecting from out of bounds exceptions.
                if (i - 2 < 0 || j - 2 < 0) backward = false;
                if (i + 2 >= Gameboard.Rows.Count || j + 2 >= Gameboard.BoardWidth) forward = false;

                if (backward && Gameboard.Rows[i - 2][j - 2].ClaimedBy == backwardPlayer) backwardsRecurring++;
                else if (backward && Gameboard.Rows[i - 2][j - 2].ClaimedBy != backwardPlayer && Gameboard.Rows[i - 2][j - 2].ClaimedBy != null) backward = false; // If claimed by opposing player.
                if (forward && Gameboard.Rows[i + 2][j + 2].ClaimedBy == forwardPlayer) forwardsRecurring++;
                else if (forward && Gameboard.Rows[i + 2][j + 2].ClaimedBy != forwardPlayer && Gameboard.Rows[i + 2][j + 2].ClaimedBy != null) forward = false; // If claimed by opposing player.
                if (!backward && !forward) break;
            }
            if (playersEqual) return (byte)(backwardsRecurring + forwardsRecurring);
            else
            {
                if (activePlayer == backwardPlayer) return backwardsRecurring >= forwardsRecurring ? backwardsRecurring : forwardsRecurring;
                else return forwardsRecurring >= backwardsRecurring ? forwardsRecurring : backwardsRecurring;
            }
        }
        public Counter((byte, byte)coordinates, byte positionValue)
        {
            Coordinates = coordinates;
            Marker = ' ';
            PositionValue = positionValue;
        }
	}
}
