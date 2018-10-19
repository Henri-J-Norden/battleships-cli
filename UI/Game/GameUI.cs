using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Game;
using UI.Game;

namespace UI {
    public static partial class GameUI {
        public static bool Stop = true;


        static void ShowRenderedBoard(string renderedBoard) {
            string game = GameText.Generate(renderedBoard);

            Console.Clear();
            Console.WriteLine(game);
        }

        static void AnimateHit(string board, Utils.Utils.Point point, int hitRange) {
            for (int i = 0; i < 8; i++) {
                board = AddHit(board, point, (i + 1) % 2, hitRange);
                ShowRenderedBoard(board);

                System.Threading.Thread.Sleep(100);
            }
        }

        public static void ShowGame(Utils.Utils.Point animateHit = null, int hitRange = 1, Players.Player player = null) {
            string board = GameText.RenderBoard();
            if (animateHit == null) {
                ShowRenderedBoard(board);
            } else {
                AnimateHit(board, animateHit, hitRange);
            }
        }

        public static void GameWinScreen() {
            Players.CurrentPlayer.ShowEnemyBoard = true;
            Players.CurrentPlayer.Messages.Add($"\n\t CUNGRADULATION {Players.CurrentPlayer}! YOU ARE WINNER!");
            
            var rnd = new Random();

            while (!EnterPressed()) {
                int range = (rnd.Next(3) == 0 ? 2 : 1);
                var p = new Utils.Utils.Point(rnd.Next(Options.OPTIONS["Board height"]), rnd.Next(Options.OPTIONS["Board width"]));
                Boards.CurrentBoard.Explode(p, range);
                string board = GameText.RenderBoard();
                AnimateHit(board, p, range);
                ShowRenderedBoard(board);

                System.Threading.Thread.Sleep(rnd.Next(500));
            }

            Stop = true;
        }

        static bool EnterPressed() {
            return !(!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Enter);
        }

        public static void ShowPlayerSwitchScreen() {
            Console.Clear();
            Console.Write($"\n\n\n\n\n\n\t\t Ready {Players.CurrentPlayer}");
            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
        }

        static void HandleTurnEnd() {
            Players.TogglePlayer();

            Players.CurrentPlayer.Messages.Add("Turn ended... (Press [ENTER] to continue...)");
            ShowGame();
            Players.CurrentPlayer.Messages.Clear();
            Players.TogglePlayer();
            
            while (!EnterPressed()) System.Threading.Thread.Sleep(100);
            ShowPlayerSwitchScreen();

            State.TurnEnded = false;
        }

        public static void Run() {
            Stop = false;
            State.ResetGame();
            while (!Stop) {
                if (State.Win) {
                    GameWinScreen();
                    break;
                }
                if (State.TurnEnded) HandleTurnEnd();
                if (State.Turn == 0) ShipPlacerController.Prepare();
                else if (Players.CurrentPlayer.IsAI) AIUI.MoveAI(Players.CurrentPlayer.AI);
                if (State.TurnEnded) HandleTurnEnd();

                ShowGame();

                if (!Players.CurrentPlayer.IsAI) {
                    var key = Console.ReadKey(false);
                    HandleKey(key);
                }
            }
        }

        private static void HandleKey(ConsoleKeyInfo key) {
            var CB = Boards.CurrentBoard;
            var coords = new Utils.Utils.Point(CB.SelectedTile.Coords);
            switch (key.Key) {
                case (ConsoleKey.UpArrow):
                    coords.Y--;
                    goto SelectTile;
                case (ConsoleKey.DownArrow):
                    coords.Y++;
                    goto SelectTile;
                case (ConsoleKey.LeftArrow):
                    coords.X--;
                    goto SelectTile;
                case (ConsoleKey.RightArrow):
                    coords.X++;
                    goto SelectTile;
                SelectTile: 
                    CB.SelectTile(coords);
                    break;
                case (ConsoleKey.Spacebar):
                    Players.CurrentPlayer.ShowEnemyBoard = !Players.CurrentPlayer.ShowEnemyBoard;
                    break;
                case (ConsoleKey.Enter):
                    if (State.Turn == 0) ShipPlacerController.Select();
                    else CB.SelectedTile.OnEnter();
                    break;
                case (ConsoleKey.Escape):
                    if (State.Turn == 0 && ShipPlacerController.Start != null) {
                        ShipPlacerController.Start = null;
                    } else {
                        Menu.CurrentNode = PauseMenu.GetPauseMenu();
                        Menu.Run();
                    }
                    break;
                
                default:
                    break;
            }
            
        }

        
    }
}
