using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;

namespace Connect4
{
    public class Gameboard
    {
        public List<Counters[]> Row { get; set;}
        private static int Instance = 0;

        public static int ReturnInstanceCount()
        {
            return Instance;
        }

        public void PrintGameBoard()
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
            Console.WriteLine("-------------------");
            Console.WriteLine("|1|2|3|4|5|6|7|8|9| <-- Enter a number to place a counter into the column.");
            Console.WriteLine("[0] = Pause\n");
        }

        public Gameboard()
        {  
            Instance++;

            Row = new List<Counters[]>();
            while(Row.Count < 11)
                Row.Add(new Counters[9]);

            for(short i = 0; i < (short)Row.Count; i++)
                for(short j = 0; j < Row[i].Length; j++)
                    Row[i][j] = new Counters();
        }
    }

}
