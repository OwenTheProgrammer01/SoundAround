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
            string sSql = "Select * FROM dbo.Album";
            //hier gaan we de verschillende dingen ophalen uit de database
            //we plaatsen dit in een datatabel
            DataTable AlbumDT = Database.GetDT(sSql);
            //Hier lezen we de datatabel uit met een foreacht
            foreach (DataRow AlbumDR in AlbumDT.Rows)
            {
                Album artiest = new Album();
                //oEvaluatie.iAccountID = Int32.Parse(EvaluatieDR["Account_ID"].ToString());
                //hier vullen we de gegevens in in de aangemaakte klasse
                artiest.Album_ID = int.Parse(AlbumDR["Album_ID"].ToString());
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

        public static bool Wijzigen(Artiest Artiest)
        {
            try
            {
                string sql = "UPDATE Artiest SET Artiest=@Artiest WHERE Artiest_ID=@Artiest_ID";
                SqlParameter ParArtiest_ID = new SqlParameter("@Artiest_ID", Artiest.Artiest_ID);
                SqlParameter ParArtiest = new SqlParameter("@Artiest", Artiest.artiest);
                Database.ExcecuteSQL(sql, ParArtiest_ID, ParArtiest);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Delete(int Artiest_ID)
        {
            try
            {
                string sql = "DELETE FROM Artiest WHERE Aritest_ID=@Artiest_ID";
                SqlParameter ParArtiest_ID = new SqlParameter("@Artiest_ID", Artiest_ID);
                Database.ExcecuteSQL(sql, ParArtiest_ID);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}