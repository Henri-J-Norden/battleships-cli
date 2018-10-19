// written by Henri J. Norden
// tested by Henri J. Norden
// published by Henri J. Norden
// development overview by Airiin Kadakmaa


using System;
using System.Collections.Generic;



namespace Main {
    class Program {
        static void Main(string[] args) {
            Console.CursorVisible = false;

            while (true) {
                UI.Menu.CurrentNode = UI.MainMenu.Menu;
                UI.Menu.Run();
                UI.GameUI.Run();
            }

            Console.WriteLine("PROGRAM_END");
            Console.ReadLine();
        }
    }
}
