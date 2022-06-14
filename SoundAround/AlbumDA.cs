using System.Collections.Generic;
//toevoegen voor database
using System.Data;
using System.Data.SqlClient;

namespace SoundAround
{
    internal class AlbumDA
    {
        public static List<Album> Ophalen()
        {
            //het uitlezen van de database
            //we maken een lijst aan voor de landen in te plaatsen
            List<Album> Album = new List<Album>();
            //We maken het statement aan om de landen uit te lezen
            string sSql = "Select Album_ID, Album FROM dbo.Album";
            //hier gaan we de verschillende dingen ophalen uit de database
            //we plaatsen dit in een datatabel
            DataTable AlbumDT = Database.GetDT(sSql);
            //Hier lezen we de datatabel uit met een foreacht
            foreach (DataRow AlbumDR in AlbumDT.Rows)
            {
                Album artiest = new Album();
                //oEvaluatie.iAccountID = Int32.Parse(EvaluatieDR["Account_ID"].ToString());
                //hier vullen we de gegevens in in de aangemaakte klasse
                artiest.Album_ID = (int)AlbumDR["Album_ID"];
                artiest.album = AlbumDR["Album"].ToString();
                //hier voegen we de klasse toe aan de lijst van de landen
                Album.Add(artiest);
            }
            return Album;
        }

        public static bool Toevoegen(Album Album)
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

        public static bool Wijzigen(Album Album)
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