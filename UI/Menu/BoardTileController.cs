using System;
using System.Collections.Generic;
using System.Text;
using Game;
using static UI.Menu;

namespace UI {
    public static class BoardTileController {
        static void AddHitMessages(List<Boards.BoardTile> hits) {
            SortedDictionary<Ships.Ship, List<Boards.BoardTile>> hitShips = new SortedDictionary<Ships.Ship, List<Boards.BoardTile>>();
            foreach (var hit in hits) {
                if (hitShips.ContainsKey(hit.Ship)) {
                    hitShips[hit.Ship].Add(hit);
                } else {
                    hitShips[hit.Ship] = new List<Boards.BoardTile>() { hit };
                }
            }
            foreach (var hitShip in hitShips.Keys) {
                foreach (var hit in hitShips[hitShip]) {
                    Players.CurrentPlayer.Messages.Add($"HIT {hit.Board.Player} @ {hit}");
                    hit.Board.Player.Messages.Add($"{Players.CurrentPlayer} hit your {hit.Ship} at {hit}");
                }
                if (hitShip.HP == 0) {
                    Players.CurrentPlayer.Messages.Add($"{hitShip.Player}: You sunk my {hitShip.Type.Name}!");
                    hitShip.Player.Messages.Add($"{Players.CurrentPlayer}: Hah I sunk your {hitShip.Type.Name}!");
                }
            }
        }

        public static MenuNode GetMenu(this Boards.BoardTile bt) {
            var n = new MenuNode("__DOOT__", new List<MenuNode>() {
                new MenuNode("__TILE__", null, null, (m) => {
                    var ship = bt.GetShipAs(Players.CurrentPlayer);
                    m.SubNodes = new List<MenuNode>() {
                        new MenuNode(PadWithSep($" {bt.Board.Player}'s base: {bt.ToString()} ")),
                        new MenuNode(Sep),
                        new MenuNode(
                            (ship == null ? $"  No {(bt.Board.Player == Players.CurrentPlayer ? "" : "known ")}ships at this tile" : "Ship at this tile:\n" +
                                $"\t{ship}\n" +
                                $"\t  Condition at this tile: {(bt.Destroyed ? "destroyed" : "OK")}"
                            )),
                        new MenuNode(""),
                        new MenuNode(Sep),
                        new MenuNode("Bomb [1x1]", ()=>{
                            GameUI.ShowGame(bt.Coords);
                            AddHitMessages(bt.Board.Explode(bt.Coords, 1));
                            Menu.Stop = true;
                        }),
                        new MenuNode($"Nuke [3x3] ({Players.CurrentPlayer.Nukes} left)", ()=>{
                            if (Players.CurrentPlayer.Nukes > 0) {
                                GameUI.ShowGame(bt.Coords, 2);
                                Players.CurrentPlayer.Nukes--;
                                AddHitMessages(bt.Board.Explode(bt.Coords, 2));
                                Menu.Stop = true;
                            }
                        }),
                        //new MenuNode(Sep),
                        

                    };

                })
            });
            n.Selectable = false;
            n.SetUpdateAction((m) => Menu.Stop = true);
            return n.SubNodes[0];
        }

        public static void OnEnter(this Boards.BoardTile bt) {
            Menu.CurrentNode = bt.GetMenu();
            Menu.Run();

        }
    }
}
