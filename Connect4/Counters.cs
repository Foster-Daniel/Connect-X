using System;
using System.Collections.Generic;
using System.Text;

namespace Connect4
{
    public class Counters
    {
        public char Marker { get; set; }
        public short[] Coordinates { get; set; }
		public Player ClaimedBy { get; set; }
		public void CounterInformation()
        {
            Program.Print($"The counter is at coordinates {Coordinates[0]}, {Coordinates[1]} and is this colour", ClaimedBy.Colour);
        }

        public void ClaimCounter(Player chosenPlayer)
		{
            ClaimedBy = chosenPlayer;
		}

        public void PrintCounter()
        {
            if(ClaimedBy == null) Program.Print(Marker.ToString(), null);
            else Program.Print(Marker.ToString(), ClaimedBy.Colour);
        }

        public Counters()
        {
            Marker = 'O';
            Coordinates = new short[2];
        }
    }
}
