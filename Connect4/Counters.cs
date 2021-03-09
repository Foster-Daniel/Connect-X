using System;
using System.Collections.Generic;
using System.Text;

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
        public sbyte PositionValue { get; set; } = 0;
        public sbyte StrategicValue { get; set; } = 0;
        public sbyte GetValue() => (sbyte)(PositionValue + StrategicValue);

		public void ClaimCounter(Player chosenPlayer)
		{
            ClaimedBy = chosenPlayer;
            Marker = chosenPlayer.Marker;
		}

        public void PrintCounter()
        {
            if (ClaimedBy == null)
            {
                Marker = '_';
                Program.Print(Marker.ToString(), null);
            }
            else
            {
                Marker = ClaimedBy.Marker;
                if (!VictoryCounter) Program.Print(Marker.ToString(), ClaimedBy.Colour);
                else Program.Print(Marker.ToString(), null, ClaimedBy.Colour);
            }
        }

        public Counter ValidateCounter()
		{
            // The computer cannot steal already claimed counters.
            if (ClaimedBy != null) return null;

            // Make sure the counter has counters below it if it is off the ground.
			for (int i = 0; i < Coordinates.Item1; i++)
                if (Gameboard.Row[i][Coordinates.Item2].ClaimedBy == null) return null;
            return this;
		}

        public Counter((byte, byte)coordinates, sbyte positionValue)
        {
            Coordinates = coordinates;
            Marker = '_';
            PositionValue = positionValue;
        }
    }
}
