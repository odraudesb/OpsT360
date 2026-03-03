using ApiModels.AppModels;
using SqlConexion;
using System.Collections.Generic;

namespace BRBKWebApiData
{
    public class BAN_Catalogo_AlturaDA : Base
    {
        public BAN_Catalogo_AlturaDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<BAN_Catalogo_Altura> ConsultarLista(out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_Catalogo_Altura>(nueva_conexion, 8000, "[BAN_Catalogo_Altura_Consultar]", null, out OnError);
        }


        public static BAN_Catalogo_Altura GetEntidad(int? _id)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Catalogo_Altura>(nueva_conexion, 4000, "[BAN_Catalogo_Altura_Consultar]", parametros);
            return obj;
        }
    }
}
