using System.Collections.Generic;
//toevoegen voor database
using System.Data;
using System.Data.SqlClient;

namespace SoundAround
{
    internal class ArtiestDA
    {
        public static List<Artiest> Ophalen()
        {
            //het uitlezen van de database
            //we maken een lijst aan voor de landen in te plaatsen
            List<Artiest> Artiest = new List<Artiest>();
            //We maken het statement aan om de landen uit te lezen
            string sSql = "Select Artiest_ID, Artiest FROM dbo.Artiest";
            //hier gaan we de verschillende dingen ophalen uit de database
            //we plaatsen dit in een datatabel
            DataTable ArtiestDT = Database.GetDT(sSql);
            //Hier lezen we de datatabel uit met een foreacht
            foreach (DataRow ArtiestDR in ArtiestDT.Rows)
            {
                Artiest artiest = new Artiest();
                //oEvaluatie.iAccountID = Int32.Parse(EvaluatieDR["Account_ID"].ToString());
                //hier vullen we de gegevens in in de aangemaakte klasse
                artiest.Artiest_ID = (int) ArtiestDR["Artiest_ID"];
                artiest.artiest = ArtiestDR["Artiest"].ToString();
                //hier voegen we de klasse toe aan de lijst van de landen
                Artiest.Add(artiest);
            }
            return Artiest;
        }

        public static bool Toevoegen(Artiest Artiest)
        {
            try
            {
                //hier geven we de sql string op
                string sql = "INSERT INTO Artiest (Artiest) VALUES (@Artiest)";
                //hier maken we de parameters aan om de dingen te kunnen aanvullen
                SqlParameter ParArtiest = new SqlParameter("@Artiest", Artiest.artiest);
                //hier sturen de opdracht naar de database
                Database.ExcecuteSQL(sql, ParArtiest);
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