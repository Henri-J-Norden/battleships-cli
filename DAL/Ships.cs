using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;


namespace DAL {
    public static class Ships {
        const char Sep = '=';
        const char Sub = '\t';

        static readonly XmlSerializer Serializer = new XmlSerializer(typeof(List<Game.Ships.ShipType>));

        public static void Load(string filePath) {
            var stream = File.Open(filePath, FileMode.Open);
            Game.Options.Ships = (List<Game.Ships.ShipType>) Serializer.Deserialize(stream);
            stream.Close();
        }

        public static void Save(string filePath) {
            var stream = File.Open(filePath, FileMode.Create);
            Serializer.Serialize(stream, Game.Options.Ships);
            stream.Close();
        }
    }
}
