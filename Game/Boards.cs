using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using static Utils.Utils;

namespace Game {
    public static partial class Boards {
        public static Board CurrentBoard => Players.CurrentPlayer.ShowEnemyBoard ? Players.GetOtherPlayer().Board : Players.CurrentPlayer.Board;

        public class BoardTile {
            public bool Destroyed = false;
            public Ships.Ship Ship;

            public Board Board;
            //public readonly int X, Y;
            public readonly Utils.Utils.Point Coords;
            public readonly string COL, ROW;

            //public bool Selected;
            private Dictionary<Players.Player, bool> _selected = new Dictionary<Players.Player, bool>();
            public bool Selected {
                get {
                    if (!_selected.ContainsKey(Players.CurrentPlayer)) _selected[Players.CurrentPlayer] = false;
                    return _selected[Players.CurrentPlayer];
                }
                internal set => _selected[Players.CurrentPlayer] = value;
            }

            public override string ToString() => $"{COL}{ROW}";

            public BoardTile(Board b, int y, int x) {
                Board = b;
                b.AllTiles.Add(this);
                Coords = new Point(y, x);
                COL = MapIntToChars(Coords.X);
                ROW = (Coords.Y + 1).ToString();
            }

            internal bool Explode() {
                if (Ship != null && !Destroyed) {
                    Ship.HP--;
                    Ship.Player.HP--;
                    Destroyed = true;
                    return true;
                }
                if (!Destroyed) {
                    Destroyed = true;
                }
                return false;
            }

            public void SetShip(Ships.Ship ship) {
                Ship = ship;
                if (!ship.Tiles.Contains(this)) ship.Tiles.Add(this);
            }

            public Ships.Ship GetShipAs(Players.Player player) {
                if (player == Board.Player || Ship == null || Ship.HP == 0) { // all ship info known
                    return Ship;
                } else { 
                    if (!Destroyed) {
                        return null;
                    } else {
                        return Ships.GetUnknownShip(Ship);
                    }
                }
            }
        }

        public class Board {
            public List<List<BoardTile>> Map = new List<List<BoardTile>>();
            public List<BoardTile> AllTiles = new List<BoardTile>();
            public Players.Player Player;

            //public BoardTile SelectedTile;
            private Dictionary<Players.Player, BoardTile> _selectedTile = new Dictionary<Players.Player, BoardTile>();
            public BoardTile SelectedTile {
                get {
                    if (!_selectedTile.ContainsKey(Players.CurrentPlayer)) _selectedTile[Players.CurrentPlayer] = Map[0][0];
                    return _selectedTile[Players.CurrentPlayer];
                }
                internal set => _selectedTile[Players.CurrentPlayer] = value;
            }

            public BoardTile GetTile(Point coords) => Map[coords.Y][coords.X];

            public void SelectTile(Point coords) {
                if (SelectedTile != null) SelectedTile.Selected = false;
                int y = NegMod(coords.Y, Options.OPTIONS["Board height"]);
                int x = NegMod(coords.X, Options.OPTIONS["Board width"]);
                SelectedTile = Map[y][x];
                SelectedTile.Selected = true;
            }

            public Board(Players.Player player) {
                Player = player;
                for (int y = 0; y < Options.OPTIONS["Board height"]; y++) {
                    Map.Add(new List<BoardTile>());
                    for (int x = 0; x < Options.OPTIONS["Board width"]; x++) {
                        Map[y].Add(new BoardTile(this, y, x));
                    }
                }
                SelectTile(new Point(0, 0));
            }

            public List<BoardTile> Explode(Point coords, int range) {
                State.Move m = new State.Move(new AI.Move(coords, range));
                State.MoveHistory.Add(m);

                int totalRange = (range - 1) * 2 + 1;
                range--;
                List<BoardTile> hits = new List<BoardTile>();
                for (int y = Math.Max(0, coords.Y - range); y <= Math.Min(Options.OPTIONS["Board height"] - 1, coords.Y + range); y++) {
                    for (int x = Math.Max(0, coords.X - range); x <= Math.Min(Options.OPTIONS["Board width"] - 1, coords.X + range); x++) {
                        if (Map[y][x].Explode()) hits.Add(Map[y][x]);
                    }
                }
                State.TryEndTurn(hits.Count((bt) => bt.Ship != null) > 0);
                return hits;
            }
        }
    }
}
