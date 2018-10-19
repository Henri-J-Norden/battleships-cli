using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Game;
using static Game.ShipPlacer;

namespace UI.Game {
    public static class ShipPlacerController {
        public static Utils.Utils.Point Start {
            get => ShipPlacer.Start;
            set => ShipPlacer.Start = value;
        }

        public static void Prepare() {
            if (Players.CurrentPlayer.IsAI) {
                Players.CurrentPlayer.AI.PlaceShips();
                GameUI.ShowPlayerSwitchScreen();
                Players.GetOtherPlayer().Messages.Clear();
                State.TurnEnded = false;
                return;
            }

            Players.CurrentPlayer.ShowEnemyBoard = false;

            if (PrepareShip()) {
                Players.CurrentPlayer.Messages.Clear();
                return;
            }

            Players.CurrentPlayer.Messages.Clear();

            Players.CurrentPlayer.Messages.Add($"Select {(Start == null ? "start" : "end")} point for {CurrentShip.Size.Item1}x{CurrentShip.Size.Item2} {CurrentShip.Name} (#{CurrentShipCount + 1} of {CurrentShip.Count})...");
        }


        public static void Select() => ShipPlacer.Select(Boards.CurrentBoard.SelectedTile.Coords);
    }
}
