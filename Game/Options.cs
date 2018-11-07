using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Game {
    public static class Options {
        //public static void SetOption(string option, string value) => SetOption(option, Convert.ToInt32(value));
        public static void SetOption(string option, int value) => OPTIONS[option] = (IsBool.Contains(option) ? Convert.ToInt32(value != 0) : value);


        public static Dictionary<string, int> OPTIONS = new Dictionary<string, int>() {
            { "Board width", 10 },
            { "Board height", 10 },
            { "Nukes", 1 },
            { "Ships may touch", 1 },
            //{ "Power-Ups", 1 },
            { "Extra turn when hit", 1 },
            //{ "Reveal hit ships", 0 }
            //{ "Board element height", 3 },
            //{ "Board element width", 6 }
        };

        public static HashSet<string> IsBool = new HashSet<string>() {
            "Ships may touch", "Power-Ups", "Extra turn when hit", "Reveal hit ships"
        };

        public static List<Ships.ShipType> Ships = new List<Ships.ShipType>() {
            new Ships.ShipType("Patrol", (1, 1)),
            new Ships.ShipType("Cruiser", (1, 2)),
            new Ships.ShipType("Submarine", (1, 3)),
            new Ships.ShipType("Battleship", (1, 4)),
            new Ships.ShipType("Carrier", (2, 3)),
            new Ships.ShipType("UFO - Unknown Floating Object", (3, 3)),
        };

    }
}
