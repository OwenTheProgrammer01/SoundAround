using System;

// -- DATABASE CONNECTIE --
using System.Data;
using System.Data.SqlClient;

namespace SoundAround
{
    class Database
    {
        // -- VERSIE 1.0 --
        // -- DATUM 23/09/2024 --
        // -- Owen Bal --

        private static String ConnectionString
        {
            get
            {
                // connection string
                string connectionString = "Database=SoundAround;Trusted_Connection=Yes";
                return connectionString;
            }
        }

        // -- Constructors --
        private Database()
        {
        }

        // -- methods --
        // making a connection with the database
        private static SqlConnection GetConnection()
        {
            // create a new connection
            SqlConnection oCon = new SqlConnection(ConnectionString);
            // open the connection
            oCon.Open();
            return oCon;
        }

        // closing the connection
        private static void ReleaseConnection(SqlConnection oCon)
        {
            if (oCon != null)
            {
                oCon.Close();
                oCon.Dispose();
            }
        }

        // making a command based on the SQL and the parameters
        private static SqlCommand BuildCommand(SqlConnection oCon, string sSql, params SqlParameter[] dbParams)
        {
            SqlCommand oCommand = oCon.CreateCommand();
            oCommand.CommandType = CommandType.Text;
            oCommand.CommandText = sSql;
            foreach (SqlParameter oPar in dbParams)
            {
                oCommand.Parameters.Add(oPar);
            }
            return oCommand;
        }

        // making a command based on the SQL and the parameters
        private static SqlCommand BuildCommand(String sSql, params SqlParameter[] dbParams)
        {
            SqlConnection oCon = GetConnection();
            return BuildCommand(oCon, sSql, dbParams);
        }

        // getting a datatable from the database
        public static DataTable GetDT(String sSql, params SqlParameter[] dbParams)
        {
            SqlCommand oCommand = null;
            try
            {
                oCommand = BuildCommand(sSql, dbParams);
                SqlDataAdapter oDA = new SqlDataAdapter();
                oDA.SelectCommand = oCommand;
                DataTable oDT = new DataTable();
                oDA.Fill(oDT);
                return oDT;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (oCommand != null)
                {
                    ReleaseConnection(oCommand.Connection);
                }
            }
        }

        // getting a datareader from the database
        public static SqlDataReader GetDR(string sSql, params SqlParameter[] dbParams)
        {
            SqlCommand oCommand = null;
            SqlDataReader oDR = null;
            try
            {
                oCommand = BuildCommand(sSql, dbParams);
                oDR = oCommand.ExecuteReader(CommandBehavior.CloseConnection);
                return oDR;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (oCommand != null)
                {
                    ReleaseConnection(oCommand.Connection);
                }
            }
        }

        // getting a scalar from the database
        public static Object executeScalar(string sSql, params SqlParameter[] dbParams)
        {
            SqlCommand oCommand = null;
            try
            {
                oCommand = BuildCommand(sSql, dbParams);
                Object oObject = oCommand.ExecuteScalar();
                return oObject;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (oCommand != null)
                {
                    ReleaseConnection(oCommand.Connection);
                }
            }
        }

        //  executing a non query
        public static void ExcecuteSQL(String sSQL, params SqlParameter[] dbParams)
        {
            SqlCommand oCommand = null;
            try
            {
                oCommand = BuildCommand(sSQL, dbParams);
                oCommand.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (oCommand != null)
                {
                    ReleaseConnection(oCommand.Connection);
                }
            }
        }
    }
}