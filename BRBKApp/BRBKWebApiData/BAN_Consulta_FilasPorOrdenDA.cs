

using ApiModels.AppModels;
using SqlConexion;
using System.Collections.Generic;

namespace BRBKWebApiData
{
    public class BAN_Consulta_FilasPorOrdenDA : Base
    {
        public BAN_Consulta_FilasPorOrdenDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<BAN_Consulta_FilasPorOrden> ConsultarLista(long _idOrdenDespacho, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idOrdenDespacho", _idOrdenDespacho);
            return sql_puntero.ExecuteSelectControl<BAN_Consulta_FilasPorOrden>(nueva_conexion, 8000, "[BAN_ConsultaFilasPorOrden]", parametros, out OnError);
        }
    }

    

}
