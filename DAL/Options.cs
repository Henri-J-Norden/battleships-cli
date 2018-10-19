using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using static Game.Options;

namespace DAL {
    public static class Options {
        const string Sep = "=";

        public static void Load(string filePath) {
            foreach (string l in File.ReadAllLines(filePath)) {
                var val = l.Split(Sep);
                OPTIONS[val[0].Trim()] = Int32.Parse(val[1].Trim());
            }
        }

        public static void Save(string filePath) {
            var sb = new StringBuilder();
            foreach (var k in OPTIONS.Keys) {
                sb.AppendLine($"{k} {Sep} {OPTIONS[k]}");
            }

            File.WriteAllText(filePath, sb.ToString());
        }
    }
}
