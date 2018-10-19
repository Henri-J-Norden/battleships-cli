using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Linq;
using static UI.Menu;

namespace UI {
    public static class FileBrowser {
        public static Action<string> InputFunc(MenuNode returnTo, Action<string> pathInputFunc) => (s) => {
            pathInputFunc(s);
            CurrentNode = returnTo;
        };

        public static List<MenuNode> GetDirectoryNodes(Action<string> inputFunc, bool allowNewFile, string path = "./") {
            var l = new List<MenuNode>();
            if (allowNewFile) {
                l.Add(new MenuNode("New file...", () => {
                    string f;
                    while (true) {
                        f = ConsoleReadValue("file name");
                        if (Directory.GetFileSystemEntries(path, f).Length == 0) break;
                        Console.Clear();
                        Console.Write("ERROR: File already exists! Please enter a different name.");
                        while (Console.ReadKey(true).Key != ConsoleKey.Enter) ;
                    }
                    inputFunc(Path.Combine(path, f));
                }));
            }

            l.Add(new MenuNode(PadWithSep("Folders")));
            string[] dirs = { };
            if (Directory.GetParent(Path.Combine(path, "..")) != null) dirs = new string[] { Path.Combine("..") };
            dirs = dirs.Concat(Directory.GetDirectories(path)).ToArray();
            foreach (var d in dirs) {
                l.Add(new MenuNode(Path.GetFileName(d), null, null, (m) => {
                    m.SubNodes = GetDirectoryNodes(inputFunc, allowNewFile, Path.Combine(path, Path.GetFileName(d)));
                }));
            }

            l.Add(new MenuNode(PadWithSep("Files")));
            var files = Directory.GetFiles(path);
            foreach (var f in files) {
                l.Add(new MenuNode(Path.GetFileName(f), () => inputFunc(f)));
            }
            return l;
        }

        /*

        public static MenuNode GetMenu(MenuNode returnTo, Action<string> pathInputFunc, bool allowNewFile = false, string startDirectory = "./") {
            Action<string> inputFunc = (s) => {
                pathInputFunc(s);
                CurrentNode = returnTo;
            };

            var n = new MenuNode("__DOOT__", new List<MenuNode>() {
                new MenuNode("__MENU__", null, null, (m) => {
                    m.SubNodes = GetDirectoryNodes(startDirectory, inputFunc, allowNewFile);
                })
            });
            n.SetUpdateAction((m) => CurrentNode = returnTo);
            return n.SubNodes[0];
        }

        public static void UseMenu(MenuNode returnTo, Action<string> pathInputFunc, bool allowNewFile = false, string startDirectory = "./") {
            var m = GetMenu(returnTo, pathInputFunc, allowNewFile, startDirectory);
            CurrentNode = m;
        }
        */
    }
}
