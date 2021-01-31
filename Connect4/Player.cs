using System;
using System.Collections.Generic;

namespace Connect4
{
    public class Player
    {
        public string Name {get; set;}

        public ConsoleColor? Colour {get; set;}
        public bool Victory {get; set;}
        public List<Gameboard> MoveCalculator {get; set;}

        public Player(string name, ConsoleColor colour)
        {
            this.Colour = colour;
            this.Name = name;
            MoveCalculator = new List<Gameboard>();
        }

        public void MakeMove(byte move, Gameboard theGameBoard, Player chosenPlayer, Player otherPlayer)
		{
            move--;
            for (short i = 0; i < theGameBoard.Row.Count; i++)
			{
                if (theGameBoard.Row[i][move].ClaimedBy == chosenPlayer || theGameBoard.Row[i][move].ClaimedBy == otherPlayer) continue;
                else
                {
                    theGameBoard.Row[i][move].ClaimCounter(chosenPlayer);
                    break;
                }
			}
		}


    }
}
