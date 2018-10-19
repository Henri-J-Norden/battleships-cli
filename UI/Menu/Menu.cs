using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using Game;


namespace UI {
    public static partial class Menu {
        public const int ValueWidth = 5;
        public const string Sep = "========================";
        public static bool Stop = true;

        public static MenuNode CurrentNode = MainMenu.Menu;
        
        public static string PadWithSep(string s) {
            float len = (float)(Sep.Length - s.Length) / 2;
            string prefix = Sep.Substring(0, (int)Math.Floor(len));
            string suffix = Sep.Substring(Sep.Length - (int)Math.Ceiling(len) - 1, (int)Math.Ceiling(len));
            return $"{prefix}{s}{suffix}";
        }

        private static void HandleKey(ConsoleKeyInfo key) {
            switch (key.Key) {
                case (ConsoleKey.UpArrow):
                    CurrentNode--;
                    break;
                case (ConsoleKey.DownArrow):
                    CurrentNode++;
                    break;
                case (ConsoleKey.Enter):
                    CurrentNode.SubNodes[CurrentNode.SelectedNodeI].Execute();
                    break;
                case (ConsoleKey.Escape):
                    if (CurrentNode.SubNodes.Last().NodeName != MenuNode.BackNodeName) break; // hax
                    CurrentNode.SelectedNodeI = CurrentNode.SubNodes.Count - 1;
                    goto case ConsoleKey.Enter;
                default:
                    break;
            }
        }

        public static string ConsoleReadValue(string optionName, string currentValue=null) {
            Console.Clear();
            Console.Write($"New value for {optionName}: ");
            string input = Console.ReadLine();
            return currentValue == null ? input : (input == "" ? currentValue : input);
        }

        public static int ConsoleReadValueInt(string optionName, int currentValue) {
            string input = ConsoleReadValue(optionName);
            if (input == "") return currentValue;
            var value = currentValue;
            if (!int.TryParse(input, out value)) return currentValue;
            return Math.Min(value, (int)Math.Pow(10, ValueWidth) - 1);
        }

        

        public static void Run() {
            Stop = false;
            CurrentNode.Execute();
            while (!Stop) {
                Console.Clear();
                Console.WriteLine(CurrentNode.Render());
                var key = Console.ReadKey(false);
                HandleKey(key);
            }
        }
    }
}
