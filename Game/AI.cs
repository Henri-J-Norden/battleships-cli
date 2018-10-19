using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Game {
    public static class AI {
        public struct Move {
            public Utils.Utils.Point P;
            public int R;

            public Move(Utils.Utils.Point point, int range) {
                R = range;
                P = point;
            }
        }

        public interface IAI {
            (int, int) MoveTimeRange { get; set; }

            Move GetMove();
            void PlaceShips();
        }

        static System.Random rnd = new System.Random();
        static Utils.Utils.Point GetRandomPoint() => new Utils.Utils.Point(rnd.Next(Options.OPTIONS["Board height"]), rnd.Next(Options.OPTIONS["Board width"]));

        public class AIriin : IAI {
            public override string ToString() => "AIriin";
            public (int, int) MoveTimeRange { get; set; } = (666, 666 * 3);

            public Move GetMove() => new Random().GetMove();

            public void PlaceShips() => new Random().PlaceShips();
        }

        public class Random : IAI {
            public override string ToString() => "Random";

            public (int, int) MoveTimeRange { get; set; } = (150, 300);

            public Move GetMove() {
                Utils.Utils.Point p;
                do {
                    p = GetRandomPoint();
                } while (Players.GetOtherPlayer().Board.GetTile(p).Destroyed);

                int range = (rnd.Next(10) == 1 ? 2 : 1);
                if (range == 2 && Players.CurrentPlayer.Nukes <= 0) {
                    range = 1;
                } else {
                    Players.CurrentPlayer.Nukes--;
                }

                return new Move(p, range);
            }

            public void PlaceShips() => PlaceShips(false, false);

            public void PlaceShip() => PlaceShips(false, true);

            public void PlaceShipType() => PlaceShips(true, false);

            void PlaceShips(bool placeOneType, bool placeOne) {
                var currentShip = ShipPlacer.CurrentShip;
                var currentShipCount = ShipPlacer.CurrentShipCount;

                while (!ShipPlacer.PrepareShip()) {
                    if (placeOne && (currentShipCount != ShipPlacer.CurrentShipCount || currentShip != ShipPlacer.CurrentShip) ||
                        placeOneType && (currentShip != ShipPlacer.CurrentShip)) return;

                    var cb = Players.CurrentPlayer.Board;
                    Utils.Utils.Point p;
                    do {
                        p = GetRandomPoint();
                    } while (!ShipPlacer.Select(p));

                    for (int y = 0; y < Options.OPTIONS["Board height"]; y++) { // NOTE: end point is not random
                        for (int x = 0; x < Options.OPTIONS["Board width"]; x++) {
                            if (ShipPlacer.Select(new Utils.Utils.Point(y, x), true)) goto END;
                        }

                    }
                    END:;
                }
            }
        }


    }
}
