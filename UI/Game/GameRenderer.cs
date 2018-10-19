using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using Game;
using UI.Game;

namespace UI {
    public static partial class GameUI {
        static List<string> BOX = new List<string> {
            "╔╦╗",
            "╠╬╣",
            "╚╩╝"
        };
        static char VERT = '║';
        static char VERT_THICK = '█';
        static char HOR = '═';
        static string HOR_THICK = "▀▄";
        static string MISS = "xD";
        static string HIT = "><";
        static string VALID_PLACE = "[]";

        const int BOX_HEIGHT = 3;
        const int BOX_WIDTH = 6;

        private static string GetTileValue(Boards.BoardTile tile, bool enemy) {
            string s = "    ";
            if (tile.Ship != null && tile.Destroyed && (!enemy || tile.Ship.HP == 0)) s = $"{HIT[0]}{tile.Ship.Index.ToString(),BOX_WIDTH - 4}{HIT[1]}"; // known ship hits 
            else if (enemy && tile.Ship != null && tile.Destroyed) s = $" {HIT,BOX_WIDTH - 4} "; // hits on unknown ships
            else if (tile.Destroyed) s = $" {MISS,BOX_WIDTH - 4} "; // missed shots
            else if (!enemy && tile.Ship != null) s = $" {tile.Ship.Index.ToString(),BOX_WIDTH - 4} "; // show shop on friendly map
            else if (ShipPlacerController.Start != null && ShipPlacer.IsValidEndPoint(tile.Coords)) s = $" {VALID_PLACE,BOX_WIDTH - 4} "; // valid end point for ship
            int ws = (BOX_WIDTH - 2 - s.Length);
            return s.PadLeft((int)Math.Floor((float)ws / 2) + s.Length).PadRight(ws + s.Length);
        }

        private static string AddHit(string renderedBoard, Utils.Utils.Point coords, int value = 0, int range = 1) {
            var sb = new StringBuilder();
            int outOfArea = coords.X - (range - 1);
            int totalRange = (range - 1) * 2 + 1 + (outOfArea < 0 ? outOfArea : 0);
            range--;
            foreach (var line in renderedBoard.Split('\n').Select((s, i) => new { i, s })) {
                var lineSb = new StringBuilder();
                if (line.i >= (coords.Y - range) * (BOX_HEIGHT - 1) && line.i <= (coords.Y + range + 1) * (BOX_HEIGHT - 1)) {
                    lineSb.Append(line.s.Substring(0, Math.Max(0, (coords.X - range) * (BOX_WIDTH - 1))));
                    lineSb.Append(new String((value == 0 ? ' ' : '█'), totalRange * (BOX_WIDTH - 1) + 1));
                    if (line.s.Length > lineSb.Length) {
                        lineSb.Append(line.s.Substring(lineSb.Length));
                    } else {
                        lineSb.Remove(line.s.Length, lineSb.Length - line.s.Length);
                    }
                    sb.Append(lineSb.ToString() + '\n');
                } else {
                    sb.Append(line.s + '\n');
                }
            }
            while (sb[sb.Length - 1] == '\n') sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        static void AddCursorTo(List<StringBuilder> sbs, Boards.Board board) {
            var y_0 = 2 * board.SelectedTile.Coords.Y;
            var x_0 = 5 * board.SelectedTile.Coords.X;
            for (int y = y_0; y < y_0 + BOX_HEIGHT; y++) {
                int x = x_0;
                sbs[y][x++] = VERT_THICK;
                if (y == y_0 || y == y_0 + BOX_HEIGHT - 1) for (; x < x_0 + BOX_WIDTH - 1; x++) sbs[y][x] = HOR_THICK[Convert.ToInt32(y != y_0)];
                sbs[y][x_0 + BOX_WIDTH - 1] = VERT_THICK;
            }
        }

        public static string Render(this Boards.Board board, bool enemy) {
            var sbFinal = new StringBuilder(14400);
            List<StringBuilder> sbs = new List<StringBuilder>();
            for (var y = 0; y < Options.OPTIONS["Board height"]; y++) {
                List<StringBuilder> sb = new List<StringBuilder>();
                for (int i = 0; i < BOX_HEIGHT; i++) sb.Add(new StringBuilder());

                for (var x = 0; x < Options.OPTIONS["Board width"]; x++) {
                    int BOX_Y = (y == 0 ? 0 : 1);
                    int BOX_X = Convert.ToInt32(x != 0);

                    sb[0].Append(BOX[BOX_Y][BOX_X]);
                    sb[0].Append(new String(HOR, BOX_WIDTH - 2));
                    for (int i = 1; i < BOX_HEIGHT - 1; i++) sb[i].Append(VERT);

                    for (int i = 1; i < BOX_HEIGHT - 1; i++) {
                        if (i == (int)Math.Ceiling((float)(BOX_HEIGHT - 2) / 2)) {
                            sb[i].Append(GetTileValue(board.Map[y][x], enemy)); // tile value
                        } else {
                            sb[i].Append(new String(' ', BOX_WIDTH - 2));
                        }
                    }

                    if (y + 1 == Options.OPTIONS["Board height"]) {
                        sb.Last().Append(BOX[2][BOX_X]);
                        sb.Last().Append(new String(HOR, BOX_WIDTH-2));
                        if (x + 1 == Options.OPTIONS["Board width"]) sb.Last().Append(BOX[2][2]);
                    }
                    if (x + 1 == Options.OPTIONS["Board width"]) {
                        sb[0].Append(BOX[BOX_Y][2]);
                        for (int i = 1; i < BOX_HEIGHT - 1; i++) sb[i].Append(VERT);
                    }
                }
                sbs.Add(sb[0]);
                for (int i = 1; i < BOX_HEIGHT - 1; i++) sbs.Add(sb[i]);
                if (y + 1 == Options.OPTIONS["Board height"]) sbs.Add(sb.Last());
            }

            if (!State.TurnEnded) AddCursorTo(sbs, board);

            sbFinal.AppendJoin('\n', sbs);
            while (sbFinal[sbFinal.Length - 1] == '\n') sbFinal.Remove(sbFinal.Length - 1, 1);
            return sbFinal.ToString();
        }
    }
}
