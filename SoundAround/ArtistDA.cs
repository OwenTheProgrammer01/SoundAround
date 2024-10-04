using System.Collections.Generic;

// -- DATABASE CONNECTIE --
using System.Data;
using System.Data.SqlClient;

namespace SoundAround
{
    internal class ArtistDA
    {
        public static List<Artist> Ophalen()
        {
            //het uitlezen van de database
            List<Artist> Artiest = new List<Artist>();
            //statement aanmaken om uit te lezen
            string sSql = "Select Artist_ID, Artist FROM dbo.Artist";
            //tabel ophalen uit de database
            DataTable ArtiestDT = Database.GetDT(sSql);
            //uitlezen van de data tabel
            foreach (DataRow ArtiestDR in ArtiestDT.Rows)
            {
                Artist artiest = new Artist();
                //invullen van de gegevens in de klasse
                artiest.Artist_ID = (int) ArtiestDR["Artist_ID"];
                artiest.artist = ArtiestDR["Artist"].ToString();
                //klasse toevoegen aan de lijst
                Artiest.Add(artiest);
            }
            return Artiest;
        }

        public static bool Toevoegen(Artist Artiest)
        {
            try
            {
                //hier geven we de sql string op
                string sql = "INSERT INTO Artist (Artist) VALUES (@Artist)";
                //hier maken we de parameters aan om de dingen te kunnen aanvullen
                SqlParameter ParArtiest = new SqlParameter("@Artist", Artiest.artist);
                //hier sturen de opdracht naar de database
                Database.ExcecuteSQL(sql, ParArtiest);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Wijzigen(Artist Artiest)
        {
            try
            {
                string sql = "UPDATE Artist SET Artist=@Artist WHERE Artist_ID=@Artist_ID";
                SqlParameter ParArtiest_ID = new SqlParameter("@Artist_ID", Artiest.Artist_ID);
                SqlParameter ParArtiest = new SqlParameter("@Artist", Artiest.artist);
                Database.ExcecuteSQL(sql, ParArtiest_ID, ParArtiest);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Delete(Artist Artiest)
        {
            try
            {
                string sql = "DELETE FROM Artist WHERE Artist_ID=@Artist_ID";
                SqlParameter ParArtiest_ID = new SqlParameter("@Artist_ID", Artiest.Artist_ID);
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