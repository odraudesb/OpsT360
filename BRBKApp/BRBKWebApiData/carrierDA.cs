using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BRBKWebApiData
{
    public class carrierDA : Base
    {
        public carrierDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static carrier GetCurrier(string _ruc)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_ruc", _ruc);
            return sql_puntero.ExecuteSelectOnly<carrier>(nueva_conexion, 4000, "[brbk].consultarCarrier", parametros);
        }

        
    }
}
