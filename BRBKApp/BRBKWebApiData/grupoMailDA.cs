using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BRBKWebApiData
{
    public class grupoMailDA : Base
    {
        public grupoMailDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<grupoMail> ListaGrupos(out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<grupoMail>(nueva_conexion, 4000, "[brbk].[consultarGrupoMail]", parametros, out OnError);

        }

        public static grupoMail GetGrupos(int _id)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_id", _id);
            return sql_puntero.ExecuteSelectOnly<grupoMail>(nueva_conexion, 4000, "[brbk].[consultarGrupoMail]", parametros);
        }

        public static DataView consultaGrupos()
        {
            OnInit("N4Middleware");
            SqlConnection cn = new SqlConnection(nueva_conexion);

            var d = new DataTable();
            using (var c = cn)
            {
                var coman = c.CreateCommand();
                coman.CommandType = CommandType.StoredProcedure;
                coman.CommandText = "[brbk].[consultarGrupoMail]";
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
