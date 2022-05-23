using System.Collections.Generic;
//toevoegen voor database
using System.Data;
using System.Data.SqlClient;

namespace SoundAround
{
    internal class GenreDA
    {
        public static List<Genre> Ophalen()
        {
            //het uitlezen van de database
            //we maken een lijst aan voor de landen in te plaatsen
            List<Genre> Genre = new List<Genre>();
            //We maken het statement aan om de landen uit te lezen
            string sSql = "Select * FROM dbo.Genre";
            //hier gaan we de verschillende dingen ophalen uit de database
            //we plaatsen dit in een datatabel
            DataTable GenreDT = Database.GetDT(sSql);
            //Hier lezen we de datatabel uit met een foreacht
            foreach (DataRow GenreDR in GenreDT.Rows)
            {
                Genre genre = new Genre();
                //oEvaluatie.iAccountID = Int32.Parse(EvaluatieDR["Account_ID"].ToString());
                //hier vullen we de gegevens in in de aangemaakte klasse
                genre.Genre_ID = int.Parse(GenreDR["Genre_ID"].ToString());
                genre.genre = GenreDR["Genre"].ToString();
                //hier voegen we de klasse toe aan de lijst van de landen
                Genre.Add(genre);
            }
            return Genre;
        }

        public static bool Toevoegen(Genre genre)
        {
            try
            {
                //hier geven we de sql string op
                string sql = "INSERT INTO Genre (Genre) VALUES (@Genre)";
                //hier maken we de parameters aan om de dingen te kunnen aanvullen
                SqlParameter ParGenre = new SqlParameter("@Genre", genre.genre);
                //hier sturen de opdracht naar de database
                Database.ExcecuteSQL(sql, ParGenre);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Wijzigen(Genre genre)
        {
            try
            {
                string sql = "UPDATE Genre SET Genre=@Genre WHERE Genre_ID=@Genre_ID";
                SqlParameter ParGenre_ID = new SqlParameter("@Genre_ID", genre.Genre_ID);
                SqlParameter ParGenre = new SqlParameter("@Genre", genre.genre);
                Database.ExcecuteSQL(sql, ParGenre_ID, ParGenre);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Delete(int Genre_ID)
        {
            try
            {
                string sql = "DELETE FROM Genre WHERE Genre_ID=@Genre_ID";
                SqlParameter ParGenre_ID = new SqlParameter("@Genre_ID", Genre_ID);
                Database.ExcecuteSQL(sql, ParGenre_ID);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}