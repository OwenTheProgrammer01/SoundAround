using System;

namespace SoundAround
{
    internal class Song
    {
        public int Song_ID { get; set; }
        public int FileType_ID { get; set; }
        public int Artist_ID { get; set; }
        public int Album_ID { get; set; }
        public byte[] SongFile { get; set; }
        public string Name { get; set; }
        public string Duration { get; set; }
    }
}