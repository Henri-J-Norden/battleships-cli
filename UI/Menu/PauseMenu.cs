using System;
using System.Collections.Generic;
using System.Text;
using static UI.Menu;
using Game;

namespace UI {
    public static class PauseMenu {
        public static MenuNode GetPauseMenu() {
            MenuNode n = new MenuNode("__DOOT__", new List<MenuNode>() {
                new MenuNode("__TILE__", null, null, (m) => {
                    m.SubNodes = new List<MenuNode>() {
                        new MenuNode(DesignStrs.Logo),
                        new MenuNode(Sep),
                        new MenuNode("Self-destruct", () => {
                            Players.CurrentPlayer.ShowEnemyBoard = false;
                            GameUI.ShowGame(new Utils.Utils.Point(1, 1), Int16.MaxValue);
                            Players.CurrentPlayer.Board.Explode(new Utils.Utils.Point(1, 1), int.MaxValue);
                            Players.CurrentPlayer.Messages.Add("Goodbye cruel world...");
                            Players.GetOtherPlayer().Messages.Add($"{Players.CurrentPlayer} blew himself up!");
                            UI.Menu.Stop = true;
                        }),
                        new MenuNode(Sep),
                        new MenuNode("QUIT", () => {
                            UI.Menu.Stop = true;
                            GameUI.Stop = true;
                        }),
                    };
                    if (State.Turn == 0) {
                        m.SubNodes.Insert(2, new MenuNode(Sep));
                        m.SubNodes.Insert(2, new MenuNode("Randomize all ships", () => {
                            new AI.Random().PlaceShips();
                            Menu.Stop = true;
                        }));
                        m.SubNodes.Insert(2, new MenuNode($"Randomize all {ShipPlacer.CurrentShip.Name}s", () => {
                            new AI.Random().PlaceShipType();
                            ShipPlacer.Start = null;
                            Menu.Stop = true;
                        }));
                        m.SubNodes.Insert(2, new MenuNode($"Randomize one {ShipPlacer.CurrentShip.Name}", () => {
                            new AI.Random().PlaceShip();
                            ShipPlacer.Start = null;
                            Menu.Stop = true;
                        }));
                    }
                }),
            });
            n.Selectable = false;
            n.SetUpdateAction((m) => Menu.Stop = true);
            return n.SubNodes[0];
        }
    }
}
