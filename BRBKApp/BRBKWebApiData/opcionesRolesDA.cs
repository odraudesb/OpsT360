using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BRBKWebApiData
{
    public class opcionesRolesDA : Base
    {
        public opcionesRolesDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<opcionesRoles> listadoOpciones(long roleId ,out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("RoleId", roleId);
            return sql_puntero.ExecuteSelectControl<opcionesRoles>(nueva_conexion, 4000, "[brbk].[consultarOpcionesRol]", parametros, out OnError);
        }

    }
}
