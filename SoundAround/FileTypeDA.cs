using System.Collections.Generic;

// -- DATABASE CONNECTIE --
using System.Data;
using System.Data.SqlClient;

namespace SoundAround
{
    internal class FileTypeDA
    {
        public static List<FileType> Fetch()
        {
            //het uitlezen van de database
            List<FileType> Bestandtype = new List<FileType>();
            //statement aanmaken om uit te lezen
            string sSql = "Select FileType_ID, FileType FROM dbo.FileType";
            //tabel ophalen uit de database
            DataTable BestandtypeDT = Database.GetDT(sSql);
            //uitlezen van de data tabel
            foreach (DataRow BestandtypeDR in BestandtypeDT.Rows)
            {
                FileType bestandtype = new FileType();
                //invullen van de gegevens in de klasse
                bestandtype.FileType_ID = (int) BestandtypeDR["FileType_ID"];
                bestandtype.filetype = BestandtypeDR["FileType"].ToString();
                //klasse toevoegen aan de lijst
                Bestandtype.Add(bestandtype);
            }
            return Bestandtype;
        }

        public static bool Add(FileType Bestandtype)
        {
            try
            {
                //hier geven we de sql string op
                string sql = "INSERT INTO FileType (FileType) VALUES (@FileType)";
                //hier maken we de parameters aan om de dingen te kunnen aanvullen
                SqlParameter ParBestandtype = new SqlParameter("@FileType", Bestandtype.filetype);
                //hier sturen de opdracht naar de database
                Database.ExcecuteSQL(sql, ParBestandtype);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Modify(FileType Bestandtype)
        {
            try
            {
                string sql = "UPDATE FileType SET FileType=@FileType WHERE FileType_ID=@FileType_ID";
                SqlParameter ParBestandtype_ID = new SqlParameter("@FileType_ID", Bestandtype.FileType_ID);
                SqlParameter ParBestandtype = new SqlParameter("@FileType", Bestandtype.filetype);
                Database.ExcecuteSQL(sql, ParBestandtype_ID, ParBestandtype);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Delete(FileType Bestandtype)
        {
            try
            {
                string sql = "DELETE FROM FileType WHERE FileType_ID=@FileType_ID";
                SqlParameter ParBestandtype_ID = new SqlParameter("@FileType_ID", Bestandtype.FileType_ID);
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