using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BRBKWebApiData
{
    public class productosDA : Base
    {
        public productosDA() : base()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<productos> listadoProductos( out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<productos>(nueva_conexion, 4000, "[brbk].consultarProductos", parametros, out OnError);
        }

        public static productos GetProducto( int _id)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<productos>(nueva_conexion, 4000, "[brbk].consultarProductos", parametros);
            try
            {
                obj.Maniobra = maniobraDA.GetManiobra(obj.idManiobra);
                obj.Items = itemsDA.GetItems(obj.item);
            }catch { }
            return obj;
        }

        
        public static DataView consultaProductos()
        {
            OnInit("N4Middleware");
            SqlConnection cn = new SqlConnection(nueva_conexion);

            var d = new DataTable();
            using (var c = cn)
            {
                var coman = c.CreateCommand();
                coman.CommandType = CommandType.StoredProcedure;
                coman.CommandText = "[brbk].consultarProductos";
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
