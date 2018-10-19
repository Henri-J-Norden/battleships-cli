using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Game;
using static UI.Menu;

namespace UI {
    public static class MainMenu {

        // == Main menu definition ==
        public static readonly MenuNode Menu = new MenuNode("__DOOT__", new List<MenuNode>() {
            new MenuNode(DesignStrs.Title + DesignStrs.Logo),
            new MenuNode("Start game", new List<MenuNode>() {
                new MenuNode(DesignStrs.Title),
                new MenuNode("New game", ()=>Stop=true),
                new MenuNode("Load game", ()=>{ }),
                new MenuNode(Sep),
                new MenuNode("Players...", null, null, (m) => {
                    m.SubNodes = new List<MenuNode>() {
                        new MenuNode(DesignStrs.Title),
                        new MenuNode(""),
                    }.Concat(GeneratePlayerNodes()).ToList();
                }),
            }),

            new MenuNode(Sep),
            new MenuNode("Ships", null, null , (m) => {
                m.SubNodes = new List<MenuNode>() {
                    new MenuNode(DesignStrs.Title),
                    new MenuNode("Add new battleship", ()=>Options.Ships.Add(new Ships.ShipType("SS. New Ship", (1, 1))), null, null, true),
                    new MenuNode(Sep),
                    new MenuNode("Load...", null, null, (n) => n.SubNodes = FileBrowser.GetDirectoryNodes(FileBrowser.InputFunc(m.ParentNode, (s) => DAL.Ships.Load(s)), false)),
                    new MenuNode("Save...", null, null, (n) => n.SubNodes = FileBrowser.GetDirectoryNodes(FileBrowser.InputFunc(m.ParentNode, (s) => DAL.Ships.Save(s)), true)),
                    new MenuNode(Sep)
                }.Concat(GenerateShipNodes()).ToList();
            }),

            new MenuNode("Options", (List<MenuNode>) new List<MenuNode>() {
                new MenuNode(DesignStrs.Title + DesignStrs.Options),
                new MenuNode("Load...", null, null, (m) => m.SubNodes = FileBrowser.GetDirectoryNodes(FileBrowser.InputFunc(m.ParentNode, (s) => DAL.Options.Load(s)), false)),
                new MenuNode("Save...", null, null, (m) => m.SubNodes = FileBrowser.GetDirectoryNodes(FileBrowser.InputFunc(m.ParentNode, (s) => DAL.Options.Save(s)), true)),
                new MenuNode(Sep)
            }.Concat(GenerateOptionNodes()).ToList()),

            new MenuNode(Sep),
            new MenuNode("Exit", () => Environment.Exit(0))
        }, false);



        private static List<MenuNode> GeneratePlayerNodes() {
            var l = new List<MenuNode>();
            
            for (int i = 0; i < Players.List.Count; i++) {
                var Player = Players.List[i];
                l.Add(new MenuNode(PadWithSep($" Player {i + 1} ")));
                l.Add(new MenuNode($"Name \"{Player.Name}\"", () => Player.Name = ConsoleReadValue($"{Player.Name} name", Player.Name), null, null, true));
                l.Add(new MenuNode("Computer", () => Player.IsAI = !Player.IsAI, () => (Player.IsAI ? MenuNode.TrueOp : ""), null, true));
                if (Player.IsAI) l.Add(new MenuNode($"AI type: {Player.AI}", null, null, (m) => {
                    m.SubNodes = GenerateAINodes(Player);
                }));
                l.Add(new MenuNode(""));
            }
            return l;
        }

        private static List<MenuNode> GenerateAINodes(Players.Player player) {
            var l = new List<MenuNode>();
            foreach (var ai in Players.AIs) {
                l.Add(new MenuNode($"{ai}", () => { }, null, (m) => {
                    player.AI = ai;
                    MenuNode.GoTo(m.ParentNode.ParentNode);
                }));
            }
            return l;
        }

        private static MenuNode NewOptionMenuNode(string nodeName, string option) {
            bool isBool = Options.IsBool.Contains(option);
            if (isBool) {
                return new MenuNode(nodeName,
                () => Options.SetOption(option, Convert.ToInt32(Options.OPTIONS[option] == 0)),
                () => (Options.OPTIONS[option] != 0) ? MenuNode.TrueOp : "");
            } else {
                return new MenuNode(nodeName,
                () => Options.SetOption(option, ConsoleReadValueInt(option, Options.OPTIONS[option])),
                () => Convert.ToString(Options.OPTIONS[option]));
            }
        }

        private static List<MenuNode> GenerateOptionNodes() {
            //var opsNode = MainMenu.SubNodes.First(m => m.NodeName == "Options");
            var l = new List<MenuNode>();
            foreach (string option in Options.OPTIONS.Keys) l.Insert(0, NewOptionMenuNode(option, option));
            return l;
        }

        private static List<MenuNode> GenerateShipNodes() {
            var l = new List<MenuNode>();

            for (int i = 0; i < Options.Ships.Count; i++) {
                var boat = Options.Ships[i];
                l.Add(new MenuNode($"{boat.Count}x {boat.Name} {boat.Size.ToString()}", null, null, (m) => {
                    if (!Options.Ships.Contains(boat)) {
                        MenuNode.GoTo(m.ParentNode--);
                        return;
                    }
                    m.SubNodes = new List<MenuNode>() {
                        new MenuNode($"Name \"{boat.Name}\"", ()=>boat.Name = ConsoleReadValue($"{boat.Name} name", boat.Name), null, null, true),
                        new MenuNode($"Amount", ()=>boat.Count = ConsoleReadValueInt($"{boat.Name} amount", boat.Count), ()=>boat.Count.ToString(), null, true),
                        new MenuNode($"Width", ()=>boat.Size.Item1 = ConsoleReadValueInt($"{boat.Name} width", boat.Size.Item1), ()=>boat.Size.Item1.ToString(), null, true),
                        new MenuNode($"Length", ()=>boat.Size.Item2 = ConsoleReadValueInt($"{boat.Name} length", boat.Size.Item2), ()=>boat.Size.Item2.ToString(), null, true),
                        new MenuNode(Sep),
                        new MenuNode("Delete", ()=>Options.Ships.Remove(boat), null, null, true)
                    };
                }));
            }
            return l;
        }
    }
}
