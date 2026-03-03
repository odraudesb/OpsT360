using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BRBKWebApiData
{
    public class parametrosDA : Base
    {
        public parametrosDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<parametros> listadoParametros(out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<parametros>(nueva_conexion, 4000, "[brbk].[BRBK_Lee_Configuraciones]", parametros, out OnError);
        }

    }
}
