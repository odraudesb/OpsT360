using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlConexion;

namespace BRBKWebApiData
{
    public abstract class Base
    {
        protected static Cls_Conexion sql_puntero = null;
        protected static Dictionary<string, object> parametros = null;
        public static String nueva_conexion = string.Empty;
             
        protected virtual void init()
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
        }

    }
}
