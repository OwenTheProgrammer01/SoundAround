using System.Collections.Generic;
//toevoegen voor database
using System.Data;
using System.Data.SqlClient;

namespace SoundAround
{
    internal class BestandtypeDA
    {
        public static List<Bestandtype> Ophalen()
        {
            //het uitlezen van de database
            //we maken een lijst aan voor de landen in te plaatsen
            List<Bestandtype> Bestandtype = new List<Bestandtype>();
            //We maken het statement aan om de landen uit te lezen
            string sSql = "Select Bestandtype_ID, Bestandtype FROM dbo.Bestandtype";
            //hier gaan we de verschillende dingen ophalen uit de database
            //we plaatsen dit in een datatabel
            DataTable BestandtypeDT = Database.GetDT(sSql);
            //Hier lezen we de datatabel uit met een foreacht
            foreach (DataRow BestandtypeDR in BestandtypeDT.Rows)
            {
                Bestandtype bestandtype = new Bestandtype();
                //oEvaluatie.iAccountID = Int32.Parse(EvaluatieDR["Account_ID"].ToString());
                //hier vullen we de gegevens in in de aangemaakte klasse
                bestandtype.Bestandtype_ID = int.Parse(BestandtypeDR["Bestandtype_ID"].ToString());
                bestandtype.bestandtype = BestandtypeDR["Bestandtype"].ToString();
                //hier voegen we de klasse toe aan de lijst van de landen
                Bestandtype.Add(bestandtype);
            }
            return Bestandtype;
        }

        public static bool Toevoegen(Bestandtype Bestandtype)
        {
            try
            {
                //hier geven we de sql string op
                string sql = "INSERT INTO Bestandtype (Bestandtype) VALUES (@Bestandtype)";
                //hier maken we de parameters aan om de dingen te kunnen aanvullen
                SqlParameter ParBestandtype = new SqlParameter("@Bestandtype", Bestandtype.bestandtype);
                //hier sturen de opdracht naar de database
                Database.ExcecuteSQL(sql, ParBestandtype);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Wijzigen(Bestandtype Bestandtype)
        {
            try
            {
                string sql = "UPDATE Bestandtype SET Bestandtype=@Bestandtype WHERE Bestandtype_ID=@Bestandtype_ID";
                SqlParameter ParBestandtype_ID = new SqlParameter("@Bestandtype_ID", Bestandtype.Bestandtype_ID);
                SqlParameter ParBestandtype = new SqlParameter("@Bestandtype", Bestandtype.bestandtype);
                Database.ExcecuteSQL(sql, ParBestandtype_ID, ParBestandtype);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Delete(int Bestandtype_ID)
        {
            try
            {
                string sql = "DELETE FROM Bestandtype WHERE Bestandtype_ID=@Bestandtype_ID";
                SqlParameter ParBestandtype_ID = new SqlParameter("@Bestandtype_ID", Bestandtype_ID);
                Database.ExcecuteSQL(sql, ParBestandtype_ID);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}