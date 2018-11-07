using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
//using Game;

namespace UI {
    public static partial class Menu {
        public class MenuNode {
            public static string TrueOp = "X  ";
            public static string BackNodeName = "Back...";

            public string NodeName;

            public bool Selectable = true;
            public bool HasBackButton = false;
     
            public MenuNode ParentNode;
            public int SelectedNodeI = 0;

            Action _InputFunc; // called when enter is pressed on this node and it has no subnodes
            Func<string> _OutputFunc; // if set, _OutputFunc's return value will be displayed in front of the node's name
            Action<MenuNode> _Update; // called right after a node is entered (enter pressed, or going back to this node) and if updateAfterInput then also after _InputFunc is called
            private List<MenuNode> _subNodes;

            public List<MenuNode> SubNodes {
                get => _subNodes;
                set {
                    foreach (var node in value) node.ParentNode = this;

                    if (Selectable && !HasBackButton) {
                        value.Add(new MenuNode(Sep));
                        value.Add(new MenuNode(BackNodeName, () => GoTo(ParentNode)));
                        HasBackButton = true;
                    }

                    _subNodes = value;
                }
            }

            public override string ToString() => $"MenuNode \"{NodeName}\"";
            private bool IsEndNode() => SubNodes == null || SubNodes.Count == 0;
            private bool IsOption() => IsEndNode() && _OutputFunc != null;

            public static void GoTo(MenuNode m) {
                if (m != null) {
                    m._Update?.Invoke(m);
                    CurrentNode = m;
                }
            }

            public void SetUpdateAction(Action<MenuNode> update = null) {
                if (update != null) _Update = (m) => {
                    m.HasBackButton = false;
                    update(m);
                };
            }

            public MenuNode() { }

            public MenuNode(string nodeName, Action inputFunc, Func<string> outputFunc = null, Action<MenuNode> update = null, bool updateParentAfterInput = false) {
                NodeName = nodeName;
                if (inputFunc != null) _InputFunc = () => {
                    inputFunc();
                    if (updateParentAfterInput) ParentNode._Update?.Invoke(ParentNode);
                };
                _OutputFunc = outputFunc;
                SetUpdateAction(update);
            }

            public MenuNode(string nodeName, List<MenuNode> subNodes, bool selectable = true) {
                NodeName = nodeName;
                Selectable = selectable;

                SubNodes = subNodes;
            }

            public MenuNode(string separator) {
                NodeName = separator;
                Selectable = false;
            }

            public void Execute() {
                _Update?.Invoke(this);
                /*
                if (ParentNode != null && CurrentNode != ParentNode) { // update function changed the current node
                    CurrentNode.Execute();
                    return;
                }*/
                if (!IsEndNode()) {
                    CurrentNode = this;
                    SelectedNodeI = SubNodes.FindIndex(m => m.Selectable);
                } else {
                    _InputFunc();
                }
            }

            public string Render() {
                StringBuilder s = new StringBuilder();
                for (int i = 0; i < SubNodes.Count; i++) {
                    var node = SubNodes[i];
                    s.Append(
                        String.Format("{0} {1} {2}\n",
                            (SelectedNodeI == i ? ">" : " "),
                            (node.IsOption() ? $"[{node._OutputFunc(),ValueWidth}]" : ""),
                            node.NodeName
                        )
                    );
                }
                return s.ToString();
            }

            private void _incrementSelection(bool selectPrevious = false) {
                int i = SelectedNodeI;
                while (true) {
                    if ((selectPrevious ? --i : ++i) < 0) i = SubNodes.Count - 1;
                    if (i >= SubNodes.Count) i = 0;
                    if (SubNodes[i].Selectable) {
                        SelectedNodeI = i;
                        break;
                    }
                }
            }

            public static MenuNode operator ++(MenuNode m) {
                m._incrementSelection();
                return m;
            }

            public static MenuNode operator --(MenuNode m) {
                m._incrementSelection(true);
                return m;
            }

        }
    }
}
