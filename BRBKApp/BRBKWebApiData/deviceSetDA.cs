using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BRBKWebApiData
{
    public class deviceSetDA:Base
    {
        public deviceSetDA()
        {
            init();
        }
        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<deviceSet> listadoDeviceSet(out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<deviceSet>(nueva_conexion, 4000, "[brbk].consultarDeviceSet", parametros, out OnError);
        }

        public static deviceSet GetDeviceSet(string _id)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_id", _id);
            return sql_puntero.ExecuteSelectOnly<deviceSet>(nueva_conexion, 4000, "[brbk].consultarDeviceSet", parametros);
        }

    }
}
