using System.Text;
using System.Collections.Generic;
using System;
using System.IO;
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
            List<Song> Song = new List<Song>();
            //We maken het statement aan om de landen uit te lezen
            string sSql = "Select Song_ID, Bestandtype_ID, Artiest_ID, Album_ID, Bestand, Naam, Duur FROM dbo.Song";
            //hier gaan we de verschillende dingen ophalen uit de database
            //we plaatsen dit in een datatabel
            DataTable SongDT = Database.GetDT(sSql);
            //Hier lezen we de datatabel uit met een foreacht
            foreach (DataRow SongDR in SongDT.Rows)
            {
                Song song = new Song();
                //oEvaluatie.iAccountID = Int32.Parse(EvaluatieDR["Account_ID"].ToString());
                //hier vullen we de gegevens in in de aangemaakte klasse
                song.Song_ID = (int) SongDR["Song_ID"];
                song.Bestandtype_ID = (int) SongDR["Bestandtype_ID"];
                song.Artiest_ID = (int) SongDR["Artiest_ID"];
                song.Album_ID = (int) SongDR["Album_ID"];
                song.Bestand = (byte[]) SongDR["Bestand"];
                song.Naam = SongDR["Naam"].ToString();
                song.Duur = SongDR["Duur"].ToString();
                //hier voegen we de klasse toe aan de lijst van de landen
                Song.Add(song);
            }
            return Song;
        }

        public static bool Toevoegen(Song song)
        {
            try
            {
                //hier geven we de sql string op
                string sql = "INSERT INTO Song (Bestandtype_ID, Artiest_ID, Album_ID, Bestand, Naam, Duur) VALUES (@Bestandtype_ID, @Artiest_ID, @Album_ID, @Bestand, @Naam, @Duur)";
                //hier maken we de parameters aan om de dingen te kunnen aanvullen
                SqlParameter ParBestandtype_ID = new SqlParameter("@Bestandtype_ID", song.Bestandtype_ID);
                SqlParameter ParArtiest_ID = new SqlParameter("@Artiest_ID", song.Artiest_ID);
                SqlParameter ParAlbum_ID = new SqlParameter("@Album_ID", song.Album_ID);
                SqlParameter ParBestand = new SqlParameter("@Bestand", song.Bestand);
                SqlParameter ParNaam = new SqlParameter("@Naam", song.Naam);
                SqlParameter ParDuur = new SqlParameter("@Duur", song.Duur);
                //hier sturen de opdracht naar de database
                Database.ExcecuteSQL(sql, ParBestandtype_ID, ParArtiest_ID, ParAlbum_ID, ParBestand, ParNaam, ParDuur);
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
                string sql = "UPDATE Song SET Betandtype_ID=@Bestandtype_ID, Artiest_ID=@Artiest_ID, Album_ID=@Album_ID, Bestand=@Bestand, Naam=@Naam, Duur=@Duur WHERE Song_ID=@Song_ID";
                SqlParameter ParSongID = new SqlParameter("@Song_ID", song.Song_ID);
                SqlParameter ParBestandtype_ID = new SqlParameter("@Bestandtype_ID", song.Bestandtype_ID);
                SqlParameter ParArtiest_ID = new SqlParameter("@Artiest_ID", song.Artiest_ID);
                SqlParameter ParAlbum_ID = new SqlParameter("@Album_ID", song.Album_ID);
                SqlParameter ParBestand = new SqlParameter("@Bestand", song.Bestand);
                SqlParameter ParNaam = new SqlParameter("@Naam", song.Naam);
                SqlParameter ParDuur = new SqlParameter("@Duur", song.Duur);
                Database.ExcecuteSQL(sql, ParSongID, ParBestandtype_ID, ParArtiest_ID, ParAlbum_ID, ParBestand, ParNaam, ParDuur);
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