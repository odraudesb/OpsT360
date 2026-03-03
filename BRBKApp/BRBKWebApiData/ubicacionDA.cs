using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BRBKWebApiData
{
    public class ubicacionDA : Base
    {
        public ubicacionDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<ubicacion> listadoUbicacion(out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<ubicacion>(nueva_conexion, 4000, "[brbk].consultarUbicacion", parametros, out OnError);
        }

        public static ubicacion GetUbicacion(string _id)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_id", _id);
            return sql_puntero.ExecuteSelectOnly<ubicacion>(nueva_conexion, 4000, "[brbk].consultarUbicacion", parametros);
        }

        public static DataView consultaUbicacion()
        {
            OnInit("N4Middleware");
            SqlConnection cn = new SqlConnection(nueva_conexion);

            var d = new DataTable();
            using (var c = cn)
            {
                var coman = c.CreateCommand();
                coman.CommandType = CommandType.StoredProcedure;
                coman.CommandText = "[brbk].consultarUbicacion";
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

