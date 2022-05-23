using System.Collections.Generic;
//toevoegen voor database
using System.Data;
using System.Data.SqlClient;

namespace SoundAround
{
    internal class SongDA
    {
        public static List<Song> Ophalen()
        {
            //het uitlezen van de database
            //we maken een lijst aan voor de landen in te plaatsen
            List<Song> Songs = new List<Song>();
            //We maken het statement aan om de landen uit te lezen
            string sSql = "Select * FROM dbo.Song";
            //hier gaan we de verschillende dingen ophalen uit de database
            //we plaatsen dit in een datatabel
            DataTable SongDT = Database.GetDT(sSql);
            //Hier lezen we de datatabel uit met een foreacht
            foreach (DataRow SongDR in SongDT.Rows)
            {
                Song song = new Song();
                //oEvaluatie.iAccountID = Int32.Parse(EvaluatieDR["Account_ID"].ToString());
                //hier vullen we de gegevens in in de aangemaakte klasse
                song.Song_ID = int.Parse(SongDR["Song_ID"].ToString());
                song.Bestandtype_ID = int.Parse(SongDR["Bestandtype_ID"].ToString());
                song.Artiest_ID = int.Parse(SongDR["Artiest_ID"].ToString());
                song.Genre_ID = int.Parse(SongDR["Genre_ID"].ToString());
                song.Album_ID = int.Parse(SongDR["Album_ID"].ToString());
                //song.Bestand = SongDR["Bestand"].ToString();
                song.Naam = SongDR["Naam"].ToString();
                //song.Duur = SongDR["Duur"].ToString();
                //hier voegen we de klasse toe aan de lijst van de landen
                Songs.Add(song);
            }
            return Songs;
        }

        public static bool Toevoegen(Song song)
        {
            try
            {
                //hier geven we de sql string op
                string sql = "INSERT INTO Song (Bestandtype_ID, Artiest_ID, Genre_ID, Album_ID, Bestand, Naam, Duur) VALUES (@Bestandtype_ID, @Artiest_ID, @Genre_ID, @Album_ID, @Bestand, @Naam, @Duur)";
                //hier maken we de parameters aan om de dingen te kunnen aanvullen
                SqlParameter ParBestandtype_ID = new SqlParameter("@Bestandtype_ID", song.Bestandtype_ID);
                SqlParameter ParArtiest_ID = new SqlParameter("@Artiest_ID", song.Artiest_ID);
                SqlParameter ParGenre_ID = new SqlParameter("@Genre_ID", song.Genre_ID);
                SqlParameter ParAlbum_ID = new SqlParameter("@Album", song.Album_ID);
                SqlParameter ParBestand = new SqlParameter("@Bestand", song.Bestand);
                SqlParameter ParNaam = new SqlParameter("@Naam", song.Naam);
                SqlParameter ParDuur = new SqlParameter("@Duur", song.Duur);
                //hier sturen de opdracht naar de database
                Database.ExcecuteSQL(sql, ParBestandtype_ID);
                return true;

            }
            catch
            {
                return false;
            }
        }

        public static bool Wijzigen(Song song)
        {
            try
            {
                string sql = "UPDATE Song SET Betandtype_ID=@Bestandtype_ID, Artiest_ID=@Artiest_ID, Genre_ID=@Genre_ID, Album_ID=@Album_ID, Bestand=@Bestand, Naam=@Naam, Duur=@Duur WHERE Song_ID=@Song_ID";
                SqlParameter ParSongID = new SqlParameter("@Song_ID", song.Song_ID);
                SqlParameter ParBestandtype_ID = new SqlParameter("@Bestandtype_ID", song.Bestandtype_ID);
                SqlParameter ParArtiest_ID = new SqlParameter("@Artiest_ID", song.Artiest_ID);
                SqlParameter ParGenre_ID = new SqlParameter("@Genre_ID", song.Genre_ID);
                SqlParameter ParAlbum_ID = new SqlParameter("@Album_ID", song.Album_ID);
                SqlParameter ParBestand = new SqlParameter("@Bestand", song.Bestand);
                SqlParameter ParNaam = new SqlParameter("@Naam", song.Naam);
                SqlParameter ParDuur = new SqlParameter("@Duur", song.Duur);
                Database.ExcecuteSQL(sql, ParSongID);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Delete(int SongID)
        {
            try
            {
                string sql = "DELETE FROM Song WHERE Song_ID=@SongID";
                SqlParameter ParSongID = new SqlParameter("@SongID", SongID);
                Database.ExcecuteSQL(sql, ParSongID);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}