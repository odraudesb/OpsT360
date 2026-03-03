using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRBKWebApiData
{
    public class BAN_Catalogo_PisoDA: Base
    {
        public BAN_Catalogo_PisoDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<BAN_Catalogo_Piso> ConsultarLista(out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_Catalogo_Piso>(nueva_conexion, 8000, "[BAN_Catalogo_Piso_Consultar]", null, out OnError);
        }


        public static BAN_Catalogo_Piso GetEntidad(int _id)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Catalogo_Piso>(nueva_conexion, 4000, "[BAN_Catalogo_Piso_Consultar]", parametros);
            return obj;
        }
    }
}
