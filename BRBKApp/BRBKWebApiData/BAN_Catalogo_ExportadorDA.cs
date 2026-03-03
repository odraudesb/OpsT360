using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;

namespace BRBKWebApiData
{
    public class BAN_Catalogo_ExportadorDA : Base
    {
        public BAN_Catalogo_ExportadorDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }
        public static List<BAN_Catalogo_Exportador> ConsultarListaExportador(string RucLinea, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_rucCliente", RucLinea);
            return sql_puntero.ExecuteSelectControl<BAN_Catalogo_Exportador>(nueva_conexion, 8000, "[BAN_Catalogo_Exportador_Consultar]", parametros, out OnError);
        }

        public static BAN_Catalogo_Exportador GetExportaador(long _id)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Catalogo_Exportador>(nueva_conexion, 4000, "[BAN_Catalogo_Exportador_Consultar]", parametros);
            return obj;
        }

        public static BAN_Catalogo_Exportador GetExportaadorPorRucLinea(int _idLinea, string ruc)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idLinea", _idLinea);
            parametros.Add("i_ruc", ruc);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Catalogo_Exportador>(nueva_conexion, 4000, "[BAN_Catalogo_Exportador_ConsultarXRucLinea]", parametros);
            return obj;
        }

        public Int64? Save_Update(BAN_Catalogo_Exportador oEntidad, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_id", oEntidad.id);
            parametros.Add("i_idLinea", oEntidad.idLinea);
            parametros.Add("i_ruc", oEntidad.ruc);
            parametros.Add("i_nombre", oEntidad.nombre);
            parametros.Add("i_usuarioCrea", oEntidad.usuarioCrea);
            parametros.Add("i_usuarioModifica", oEntidad.usuarioModifica);
            parametros.Add("i_estado", oEntidad.estado);

            var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[BAN_Catalogo_Exportador_Insertar]", parametros, out OnError);
            if (!db.HasValue || db.Value < 0)
            {
                return null;
            }
            OnError = string.Empty;
            return db.Value;
        }
    }
}
