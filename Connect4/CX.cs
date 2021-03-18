using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectX
{
	class CX
	{
        /// <summary>
        ///     Receive input from the user without the neccesity of pressing Enter afterwards.
        /// </summary>
        /// <param name="limit">Limit the numeric digits with this property. For example: if the limit is '4' then the user can press the keys: 0, 1, 2, 3, but not higher.</param>
        /// <param name="message">Write an optional message that will appear as an error message to the user if they enter keys incorrectly.</param>
        /// <param name="menu">When pressing 'Z' in-game it undos a move, in a menu this should be disabled. Set to true to disable the key.</param>
        /// <returns>A byte value used for player input.</returns>
        public static byte GetKey(byte limit = 9, string message = "\nPlease only choose from the given options....", bool menu = false)
        {
            ConsoleKey playerChoice = ConsoleKey.D0;
            while (true)
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
            if (foregroundColour != null) Console.ForegroundColor = (ConsoleColor)foregroundColour;
            if (backgroundColour != null) Console.BackgroundColor = (ConsoleColor)backgroundColour;
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
    }
}
