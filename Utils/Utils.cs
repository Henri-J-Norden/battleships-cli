using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace Utils {
    public static class Utils {
        public class Point {
            public int Y, X;
            public Point(int y, int x) {
                Y = y;
                X = x;
            }
            public Point(Point point) {
                Y = point.Y;
                X = point.X;
            }

            public override bool Equals(object obj) {
                var o = obj as Point;
                if (o == null) return false;
                return (o.X == X && o.Y == Y);
            }
        }
        public class Size : Point {
            public Size(int y, int x) : base(y, x) { }
        };

        public static int NegMod(int value, int mod) => value % mod + (value < 0 ? mod : 0);

        public static string MapIntToChars(int integer) {
            StringBuilder sb = new StringBuilder();
            while (true) {
                var prefix = integer / ('Z' - 'A');
                if (prefix == 0) break;
                integer -= prefix * ('Z' - 'A');
                sb.Append((char)(prefix + 'A'));
            }
            sb.Append((char)(integer + 'A'));
            return sb.ToString();
        }
    }
}
