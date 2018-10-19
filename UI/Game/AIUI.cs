using System;
using System.Collections.Generic;
using System.Text;
using Game;

namespace UI.Game {
    public static class AIUI {
        static Random rnd = new Random();

        public static void MoveTo(AI.IAI AI, Utils.Utils.Point p) {
            Func<Utils.Utils.Point> currentP = () => Boards.CurrentBoard.SelectedTile.Coords;

            while (!currentP().Equals(p)) {
                var move = new Utils.Utils.Point(currentP());

                switch (rnd.Next(2)) {
                    case 0:
                        if (p.Y != currentP().Y) {
                            move.Y += (p.Y > currentP().Y ? 1 : -1);
                        } else {
                            goto case 1;
                        }
                        break;
                    case 1:
                        if (p.X != currentP().X) {
                            move.X += (p.X > currentP().X ? 1 : -1);
                        } else {
                            goto case 0;
                        }
                        break;
                }

                while (Console.KeyAvailable) {
                    if (Console.ReadKey(true).Key == ConsoleKey.Enter) {
                        Boards.CurrentBoard.SelectTile(p);
                    }
                }

                Boards.CurrentBoard.SelectTile(move);
                GameUI.ShowGame();
                System.Threading.Thread.Sleep(rnd.Next(AI.MoveTimeRange.Item1, AI.MoveTimeRange.Item2));
            }

            
        }

        public static void MoveAI(AI.IAI AI) {
            var p = AI.GetMove();
            Players.CurrentPlayer.ShowEnemyBoard = true;
            GameUI.ShowGame();
            System.Threading.Thread.Sleep(1000);
            MoveTo(AI, p.P);
            GameUI.ShowGame(p.P, p.R);
            Boards.CurrentBoard.Explode(p.P, p.R);
        }
    }
}
