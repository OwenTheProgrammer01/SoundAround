using System.Text;
using System.Collections.Generic;
using System;
using System.IO;

// -- DATABASE CONNECTIE --
using System.Data;
using System.Data.SqlClient;

namespace SoundAround
{
    internal class SongDA
    {
        public static List<Song> Fetch()
        {
            //het uitlezen van de database
            List<Song> Song = new List<Song>();
            //We maken het statement aan om uit te lezen
            string sSql = "Select Song_ID, FileType_ID, Artist_ID, Album_ID, SongFile, Name, Duration FROM dbo.Song";
            //tabel ophalen uit de database
            DataTable SongDT = Database.GetDT(sSql);
            //uitlezen van de data tabel
            foreach (DataRow SongDR in SongDT.Rows)
            {
                Song song = new Song();
                //invullen van de gegevens in de klasse
                song.Song_ID = (int) SongDR["Song_ID"];
                song.FileType_ID = (int) SongDR["FileType_ID"];
                song.Artist_ID = (int) SongDR["Artist_ID"];
                song.Album_ID = (int) SongDR["Album_ID"];
                song.SongFile = (byte[]) SongDR["SongFile"];
                song.Name = SongDR["Name"].ToString();
                song.Duration = SongDR["Duration"].ToString();
                //klasse toevoegen aan de lijst
                Song.Add(song);
            }
            return Song;
        }

        public static bool Add(Song song)
        {
            try
            {
                //hier geven we de sql string op
                string sql = "INSERT INTO Song (FileType_ID, Artist_ID, Album_ID, SongFile, Name, Duration) VALUES (@FileType_ID, @Artist_ID, @Album_ID, @SongFile, @Name, @Duration)";
                //hier maken we de parameters aan om de dingen te kunnen aanvullen
                SqlParameter ParFileType_ID = new SqlParameter("@FileType_ID", song.FileType_ID);
                SqlParameter ParArtiest_ID = new SqlParameter("@Artist_ID", song.Artist_ID);
                SqlParameter ParAlbum_ID = new SqlParameter("@Album_ID", song.Album_ID);
                SqlParameter ParSongFile = new SqlParameter("@SongFile", song.SongFile);
                SqlParameter ParName = new SqlParameter("@Name", song.Name);
                SqlParameter ParDuration = new SqlParameter("@Duration", song.Duration);
                //hier sturen de opdracht naar de database
                Database.ExcecuteSQL(sql, ParFileType_ID, ParArtiest_ID, ParAlbum_ID, ParSongFile, ParName, ParDuration);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Modify(Song song)
        {
            try
            {
                string sql = "UPDATE Song SET FileType_ID=@FileType_ID, Artist_ID=@Artist_ID, Album_ID=@Album_ID, SongFile=@SongFile, Name=@Name, Duration=@Duration WHERE Song_ID=@Song_ID";
                SqlParameter ParSongID = new SqlParameter("@Song_ID", song.Song_ID);
                SqlParameter ParFileType_ID = new SqlParameter("@FileType_ID", song.FileType_ID);
                SqlParameter ParArtist_ID = new SqlParameter("@Artist_ID", song.Artist_ID);
                SqlParameter ParAlbum_ID = new SqlParameter("@Album_ID", song.Album_ID);
                SqlParameter ParSongFile = new SqlParameter("@SongFile", song.SongFile);
                SqlParameter ParName = new SqlParameter("@Name", song.Name);
                SqlParameter ParDuration = new SqlParameter("@Duration", song.Duration);
                Database.ExcecuteSQL(sql, ParSongID, ParFileType_ID, ParArtist_ID, ParAlbum_ID, ParSongFile, ParName, ParDuration);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Delete(Song song)
        {
            try
            {
                string sql = "DELETE FROM Song WHERE Song_ID=@Song_ID";
                SqlParameter ParSong_ID = new SqlParameter("@Song_ID", song.Song_ID);
                Database.ExcecuteSQL(sql, ParSong_ID);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}