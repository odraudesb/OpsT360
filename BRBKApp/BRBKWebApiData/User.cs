using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRBKWebApiData
{
    public class UserDA : Base
    {
        public UserDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<User> listadoUser(out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<User>(nueva_conexion, 4000, "[brbk].[consultarUser]", parametros, out OnError);
        }

        public static User GetUser(string _id)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_id", _id);
            return sql_puntero.ExecuteSelectOnly<User>(nueva_conexion, 4000, "[brbk].[consultarUser]", parametros);
        }
    }
}
