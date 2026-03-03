using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace AccesoDatos
{
   
    public class BDCon
    {
        public static string SQLControl(SqlException s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (SqlError ei in s.Errors)
            {
                sb.AppendLine(string.Format("{0}, {1}, {2}, {3}", ei.LineNumber, ei.Message, ei.Procedure, ei.Server));
            }
            return sb.ToString();
        }


        private static string _cde;
        internal string CadenaBasica { get { return _cde; } }

        public BDCon()
        {
          
        }
    

        public DataOperacion<bool> SetBasica(string basic)
        {
            basic = System.Configuration.ConfigurationManager.ConnectionStrings[basic]?.ConnectionString;

            if (string.IsNullOrEmpty(basic))
            { return DataOperacion<bool>.CrearFalla("La cadena de conexion no puede ser nula o vacía"); }


            // aqui se cae
            using (SqlConnection c = new SqlConnection(basic))
                
                try
                {
                    c.Open();
                    var xc = c.CreateCommand();
                    xc.CommandText = "SELECT 1";
                    xc.ExecuteScalar();
                    xc.Dispose();
                    _cde = basic;
                    return DataOperacion<bool>.CrearResultadoExitoso(true);
                }
                catch (SqlException e)
                {
                    return DataOperacion<bool>.CrearFalla(SQLControl(e));
                }
                catch (Exception e)
                {
                    return DataOperacion<bool>.CrearFalla(e.Message);
                }
                finally
                {
                    if (c.State == ConnectionState.Open) { c.Close(); }
                }
            }



        

        public static DataOperacion<bool> VerificarConexion(string _conn , string _comm = null)
        {
            if (string.IsNullOrEmpty(_conn))
            { return DataOperacion<bool>.CrearFalla("La cadena de conexion no puede ser nula o vacía"); }
            using (SqlConnection c = new SqlConnection(_conn))
            {
                try
                {
                    c.Open();
                    var xc = c.CreateCommand();
                    xc.CommandText = string.IsNullOrEmpty(_comm) ? "SELECT 1" : _comm;
                    xc.ExecuteScalar();
                    xc.Dispose();
                    return DataOperacion<bool>.CrearResultadoExitoso(true);
                }
                catch (SqlException e)
                {
                    return DataOperacion<bool>.CrearFalla(SQLControl(e));
                }
                catch (Exception e)
                {
                    return DataOperacion<bool>.CrearFalla(e.Message);
                }
                finally
                {
                    if (c.State == ConnectionState.Open) { c.Close(); }
                }
            }
        }
    }
}
