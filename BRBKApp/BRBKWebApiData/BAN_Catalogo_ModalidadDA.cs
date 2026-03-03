using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;


namespace BRBKWebApiData
{
    public class BAN_Catalogo_ModalidadDA : Base
    {
        public BAN_Catalogo_ModalidadDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<BAN_Catalogo_Modalidad> ConsultarLista(out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_Catalogo_Modalidad>(nueva_conexion, 4000, "BAN_Catalogo_Modalidad_Consultar", parametros, out OnError);
        }

        public static List<BAN_Catalogo_Modalidad> ConsultarListaModalidadEmbarque(out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_Catalogo_Modalidad>(nueva_conexion, 4000, "BAN_Catalogo_Modalidad_Consultar_Embarque", parametros, out OnError);
        }
    }
}

