using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BRBKWebApiData
{
    public class maniobraDA : Base
    {
        public maniobraDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }
        
        public static List<maniobra> ListaManiobras( out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<maniobra>(nueva_conexion, 4000, "[brbk].[consultarManiobras]", parametros, out OnError);

        }

        public static maniobra GetManiobra(int _id)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_id", _id);
            return sql_puntero.ExecuteSelectOnly<maniobra>(nueva_conexion, 4000, "[brbk].[consultarManiobras]", parametros);
        }

        public static DataView consultaManiobras()
        {
            OnInit("N4Middleware");
            SqlConnection cn = new SqlConnection(nueva_conexion);

            var d = new DataTable();
            using (var c = cn)
            {
                var coman = c.CreateCommand();
                coman.CommandType = CommandType.StoredProcedure;
                coman.CommandText = "[brbk].consultarManiobras";
                try
                {
                    c.Open();
                    d.Load(coman.ExecuteReader(CommandBehavior.CloseConnection));
                }
                catch 
                {
                    throw;
                }
                finally
                {
                    if (c.State == ConnectionState.Open)
                    {
                        c.Close();
                    }
                    c.Dispose();
                }
            }
            return d.DefaultView;
        }
    }
}
