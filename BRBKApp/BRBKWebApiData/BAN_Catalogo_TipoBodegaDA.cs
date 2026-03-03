using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;

namespace BRBKWebApiData
{
    public class BAN_Catalogo_TipoBodegaDA : Base
    {
        public BAN_Catalogo_TipoBodegaDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<BAN_Catalogo_TipoBodega> ConsultarLista(out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_Catalogo_TipoBodega>(nueva_conexion, 8000, "[BAN_Catalogo_TipoBodega_Consultar]", null, out OnError);
        }


        public static BAN_Catalogo_TipoBodega GetEntidad(int _id)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Catalogo_TipoBodega>(nueva_conexion, 4000, "[BAN_Catalogo_TipoBodega_Consultar]", parametros);
            return obj;
        }
    }
}
