using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;

namespace Game {
    public static class State {
        public static int Turn = 0;
        public static bool Win = false;

        public static bool TurnEnded = false;

        public static List<Move> MoveHistory;

        public struct Move {
            public AI.Move M;
            public int PlayerI;
            public bool EnemyBoard;
            
            public Move(AI.Move m) {
                PlayerI = Players.List.IndexOf(Players.CurrentPlayer);
                EnemyBoard = Boards.CurrentBoard != Players.CurrentPlayer.Board;
                M = m;
            }
        }


        public static void EndTurn() {
            if (Players.CurrentPlayer == Players.List.Last()) Turn++;
            Players.TogglePlayer();
            TurnEnded = true;
        }

        public static bool CheckHP() {
            if (Players.CurrentPlayer.HP <= 0) {
                Players.TogglePlayer();
                return true;
            } else if (Players.GetOtherPlayer().HP <= 0) {
                return true;
            }
            return false;
        }

        public static void ResetGame() {
            Turn = 0;
            TurnEnded = false;
            Win = false;
            Players.CurrentPlayer = Players.List[0];

            foreach (var player in Players.List) {
                player.Messages.Clear();
                player.Ships.Clear();
                player.Board = new Boards.Board(player);
                player.Nukes = Options.OPTIONS["Nukes"];
                player.HP = 0;
            }

            MoveHistory = new List<Move>();
        }

        public static void TryEndTurn(bool hit) {
            if (CheckHP()) {
                Win = true;
                TurnEnded = true;
                return;
            }
            if (hit && Options.OPTIONS["Extra turn when hit"] != 0) { // dont end turn

            } else {
                EndTurn();
            }
        }



    }
}
