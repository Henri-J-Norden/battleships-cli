using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Game {
public class Ships {
        private static readonly ShipType UnknownType = new ShipType("Unknown", (0, 0));

        public static Ship GetUnknownShip(Ship ship) {
            Ship s = new Ship(UnknownType, ship.Index);
            s.HP = -1;
            return s;
        }

        public class Ship : IComparable<Ship> {
            public ShipType Type;
            public int Index;
            public int HP;

            public List<Boards.BoardTile> Tiles;
            public Players.Player Player;

            public override string ToString() => $"{Type.Name} [HP: {(HP < 0 ? "?" : HP.ToString())} / {(Type.HP == 0 ? "?" : Type.HP.ToString())}]";

            internal Ship(ShipType shipType, int index) {
                Type = shipType;
                Index = index;
                HP = Type.HP;
            }

            public int CompareTo(Ship other) {
                if (Index == other.Index) return 0;
                if (Index < other.Index) return -1;
                return 1;
            }

            public Ship(Players.Player player, ShipType shipType, Utils.Utils.Point start, Utils.Utils.Point end) {
                Player = player;
                Player.Ships.Add(this);
                Type = shipType;
                Index = Player.Ships.Count;
                HP = Type.HP;
                Player.HP += HP;

                for (int y = Math.Min(start.Y, end.Y); y <= Math.Max(start.Y, end.Y); y++) {
                    for (int x = Math.Min(start.X, end.X); x <= Math.Max(start.X, end.X); x++) {
                        Player.Board.Map[y][x].SetShip(this);
                    }
                }
            }
        }


    [Serializable, XmlRoot(ElementName = "ShipType")]
    public class ShipType {
            public (int, int) Size;
            public int HP => Size.Item1 * Size.Item2;
            public string Name;
            public int Count;

            public ShipType(string name, (int, int) size, int count = 1) {
                Name = name;
                Size = size;
                Count = count;
            }

            internal ShipType() { }

            public override string ToString() {
                return $"ShipType \"{Name}\"";
            }
        }
    }
}
