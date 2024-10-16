using System.Collections.Generic;

// -- DATABASE CONNECTIE --
using System.Data;
using System.Data.SqlClient;

namespace SoundAround
{
    internal class AlbumDA
    {
        public static List<Album> Fetch()
        {
            //het uitlezen van de database
            List<Album> Album = new List<Album>();
            //statement aanmaken om uit te lezen
            string sSql = "Select Album_ID, Album FROM dbo.Album";
            //tabel ophalen uit de database
            DataTable AlbumDT = Database.GetDT(sSql);
            //uitlezen van de data tabel
            foreach (DataRow AlbumDR in AlbumDT.Rows)
            {
                Album artiest = new Album();
                //invullen van de gegevens in de klasse
                artiest.Album_ID = (int)AlbumDR["Album_ID"];
                artiest.album = AlbumDR["Album"].ToString();
                //klasse toevoegen aan de lijst
                Album.Add(artiest);
            }
            return Album;
        }

        public static bool Add(Album Album)
        {
            try
            {
                //hier geven we de sql string op
                string sql = "INSERT INTO Album (Album) VALUES (@Album)";
                //hier maken we de parameters aan om de dingen te kunnen aanvullen
                SqlParameter ParAblum = new SqlParameter("@Album", Album.album);
                //hier sturen de opdracht naar de database
                Database.ExcecuteSQL(sql, ParAblum);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Modify(Album Album)
        {
            try
            {
                string sql = "UPDATE Album SET Album=@Album WHERE Album_ID=@Album_ID";
                SqlParameter ParAlbum_ID = new SqlParameter("@Album_ID", Album.Album_ID);
                SqlParameter ParAlbum = new SqlParameter("@Album", Album.album);
                Database.ExcecuteSQL(sql, ParAlbum_ID, ParAlbum);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Delete(Album Album)
        {
            try
            {
                string sql = "DELETE FROM Album WHERE Album_ID=@Album_ID";
                SqlParameter ParAlbum_ID = new SqlParameter("@Album_ID", Album.Album_ID);
                Database.ExcecuteSQL(sql, ParAlbum_ID);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}