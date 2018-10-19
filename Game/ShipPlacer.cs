using System;
using System.Collections.Generic;
using System.Text;

namespace Game {
    public static class ShipPlacer {
        public static Ships.ShipType CurrentShip;
        public static int CurrentShipCount;
        static List<Ships.ShipType> ShipQueue;

        public static Utils.Utils.Point Start;

        public static bool PrepareShip() {
            if (ShipQueue == null) {
                ShipQueue = new List<Ships.ShipType>(Options.Ships);
            }
            if (CurrentShip == null) {
                CurrentShip = ShipQueue[0];
                ShipQueue.RemoveAt(0);
                CurrentShipCount = 0;
            }
            if (CurrentShipCount >= CurrentShip.Count) {
                CurrentShip = null;
                if (ShipQueue.Count == 0) { // no more ships to put down - end ship placement for this player
                    ShipQueue = null;
                    CurrentShipCount = 0;
                    State.EndTurn();
                    return true;
                } else {
                    return PrepareShip();
                }
            }
            return false;
        }

        public static bool IsValidStartPoint(Utils.Utils.Point start) {
            int offset = (Options.OPTIONS["Ships may touch"] == 0 ? 1 : 0);
            for (int y = start.Y - offset; y <= start.Y + offset; y++) {
                for (int x = start.X - offset; x <= start.X + offset; x++) {
                    if (x < 0 || y < 0 || x >= Options.OPTIONS["Board width"] || y >= Options.OPTIONS["Board height"]) continue;
                    if (Players.CurrentPlayer.Board.Map[y][x].Ship != null) return false;
                }
            }
            return true;
        }

        public static bool IsValidEndPoint(Utils.Utils.Point end) {
            if (Start == null) throw new NullReferenceException("Ship start point not set!");
            int ySize = Math.Abs(end.Y - Start.Y) + 1;
            int xSize = Math.Abs(end.X - Start.X) + 1;
            if (!((ySize == CurrentShip.Size.Item1 && xSize == CurrentShip.Size.Item2) || (ySize == CurrentShip.Size.Item2 && xSize == CurrentShip.Size.Item1))) return false;
            int offset = (Options.OPTIONS["Ships may touch"] == 0 ? 1 : 0);
            for (int y = Math.Min(Start.Y, end.Y) - offset; y <= Math.Max(Start.Y, end.Y) + offset; y++) {
                for (int x = Math.Min(Start.X, end.X) - offset; x <= Math.Max(Start.X, end.X) + offset; x++) {
                    if (x < 0 || y < 0 || x >= Options.OPTIONS["Board width"] || y >= Options.OPTIONS["Board height"]) continue;
                    if (Players.CurrentPlayer.Board.Map[y][x].Ship != null) return false;
                }
            }
            return true;
        }

        public static bool Select(Utils.Utils.Point point, bool AI = false) {
            if (Start == null) {
                if (!IsValidStartPoint(point)) return false;
                Start = point;
                if (!AI && CurrentShip.HP == 1) Select(point);
            } else {
                if (IsValidEndPoint(point)) {
                    new Ships.Ship(Players.CurrentPlayer, CurrentShip, Start, point);
                    CurrentShipCount++;
                    Start = null;
                    if (!AI) PrepareShip();
                } else {
                    if (!AI) Start = null;
                    return false;
                }
            }
            return true;
        }
    }
}
