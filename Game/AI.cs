using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        static int GetRange(int nukeProbabilityOneIn) {
            if (Players.CurrentPlayer.Nukes > 0 && rnd.Next(nukeProbabilityOneIn) == 0) {
                Players.CurrentPlayer.Nukes--;
                return 2;
            }
            return 1;
        }

        public interface IAI {
            (int, int) MoveTimeRange { get; set; }

            Move GetMove();
            void PlaceShips();
        }

        static System.Random rnd = new System.Random();

        public class Competitive : IAI {
            public override string ToString() => "Competitive";

            public (int, int) MoveTimeRange { get; set; } = (100, 200);

            private Utils.Utils.Point SelectAroundPoint(Utils.Utils.Point p) {
                for (int add = -1; add < 2; add += 2) {
                    for (int axis = 0; axis < 2; axis++) {
                        var p_ = new Utils.Utils.Point(p.Y + add * (axis % 2), p.X + add * ( (axis + 1) % 2 ));
                        if (p_.X < 0 || p_.Y < 0 || p_.X >= Options.OPTIONS["Board width"] || p_.Y >= Options.OPTIONS["Board height"]) continue;
                        if (!Players.GetOtherPlayer().Board.GetTile(p_).Destroyed) return p_;
                    }
                }
                return null;
            }

            public Move GetMove() {
                var knownShip = Players.GetOtherPlayer().Ships.FirstOrDefault(s => s.HP != 0 && s.HP != s.Type.HP);
                
                if (knownShip != null) {
                    foreach (var tile in knownShip.Tiles) {
                        if (!tile.Destroyed) continue;

                        Utils.Utils.Point p = SelectAroundPoint(tile.Coords);
                        
                        if (p != null) return new Move(p, 1);
                    }
                }

                return new Move(Random.GetRandomPoint(), GetRange(1));
            }

            public void PlaceShips() => new Random().PlaceShips();
        }

        public class Cheater : IAI {
            public override string ToString() => "Cheater";

            public (int, int) MoveTimeRange { get; set; } = (50, 150);

            public Move GetMove() {
                var range = GetRange(2);

                if (rnd.Next(2) == 0) {
                    var ships = Players.GetOtherPlayer().Ships.Where(s => s.HP != 0).ToList();
                    var ship = ships[rnd.Next(ships.Count())];
                    var p = ship.Tiles.First(t => !t.Destroyed).Coords;
                    return new Move(p, range);
                } else {
                    return new Move(Random.GetRandomPoint(), range);
                }
            }

            public void PlaceShips() => new Random().PlaceShips();

        }

        public class Random : IAI {
            public override string ToString() => "Random";

            public (int, int) MoveTimeRange { get; set; } = (150, 300);

            public static Utils.Utils.Point GetRandomPoint(bool allowDestroyed=false) {
                if (allowDestroyed) {
                    return new Utils.Utils.Point(rnd.Next(Options.OPTIONS["Board height"]), rnd.Next(Options.OPTIONS["Board width"]));
                } else {
                    var tiles = Players.GetOtherPlayer().Board.AllTiles.Where(t => !t.Destroyed).ToList();
                    return tiles[rnd.Next(tiles.Count)].Coords;
                }
            }

            public Move GetMove() => new Move(GetRandomPoint(), GetRange(10));

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
                        p = GetRandomPoint(true);
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

        public class AIriin : IAI {
            public override string ToString() => "AIriin";
            public (int, int) MoveTimeRange { get; set; } = (666, 666 * 3);

            public Move GetMove() => new Random().GetMove();

            public void PlaceShips() => new Random().PlaceShips();
        }

    }
}
