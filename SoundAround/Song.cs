using System.IO;

namespace SoundAround
{
    internal class Song
    {
        //alle nodige gegevens aanmaken om op te vragen en weg te schrijven in de klasse
        public int Song_ID { get; set; }
        public int Bestandtype_ID { get; set; }
        public int Artiest_ID { get; set; }
        public int Genre_ID { get; set; }
        public int Album_ID { get; set; }
        public Stream Bestand { get; set; }
        public string Naam { get; set; }
        public string Duur { get; set; }
    }
}