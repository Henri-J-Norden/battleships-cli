using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Game {
    public static class Players {
        public static int MaxHP => Options.Ships.Sum((s) => s.HP * s.Count);

        public static List<AI.IAI> AIs = new List<AI.IAI>() {
            new AI.Random(),
            new AI.AIriin(),
            new AI.Cheater(),
            
        };

        public static readonly List<Player> List = new List<Player>() {
            new Player("Player 1"),
            new Player("Player 2", AIs[0])
        };

        //public static int SelectedPlayerI = 0;
        public static Player CurrentPlayer;

        public static void AddPlayer(string name) => List.Add(new Player(name));
        public static Player GetOtherPlayer(Player from = null) => List.First(p => p != (from ?? CurrentPlayer));
        public static void TogglePlayer() => CurrentPlayer = GetOtherPlayer();

        public class Player {
            public string Name;
            public override string ToString() => Name;

            public bool IsAI = false;
            public AI.IAI AI = AIs[0];

            public Boards.Board Board;
            public bool ShowEnemyBoard = true;
            public List<Ships.Ship> Ships = new List<Ships.Ship>();

            public int HP;

            public int Nukes;

            public List<string> Messages = new List<string>();

            public Player(string name) {
                Name = name;
                if (CurrentPlayer == null)
                    CurrentPlayer = this;

                Board = new Boards.Board(this);

                Nukes = Options.OPTIONS["Nukes"];
            }

            public Player(string name, AI.IAI ai) : this(name) {
                IsAI = true;
                AI = ai;
            }
        }
    }
}
