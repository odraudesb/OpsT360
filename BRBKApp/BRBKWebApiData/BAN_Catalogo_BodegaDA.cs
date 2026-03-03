using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;

namespace BRBKWebApiData
{
    public class BAN_Catalogo_BodegaDA : Base
    {
        public BAN_Catalogo_BodegaDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<BAN_Catalogo_Bodega> ConsultarLista(out string OnError)
        {
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_Catalogo_Bodega>(nueva_conexion, 8000, "[BAN_Catalogo_Bodega_Consultar]", null, out OnError);
        }


        public static BAN_Catalogo_Bodega GetEntidad(int? _id)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Catalogo_Bodega>(nueva_conexion, 4000, "[BAN_Catalogo_Bodega_Consultar]", parametros);
            return obj;
        }

        public Int64? Save_Update(BAN_Catalogo_Bodega oEntidad, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_id", oEntidad.id);
            parametros.Add("i_codigo", oEntidad.codigo);
            parametros.Add("i_nombre", oEntidad.nombre);
            parametros.Add("i_idTipo", oEntidad.idTipo);
            parametros.Add("i_capacidadBox", oEntidad.capacidadBox);
            parametros.Add("i_usuarioCrea", oEntidad.usuarioCrea);
            parametros.Add("i_usuarioModifica", oEntidad.usuarioModifica);
            parametros.Add("i_estado", oEntidad.estado);

            var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "BAN_Catalogo_Bodega_Insertar", parametros, out OnError);
            if (!db.HasValue || db.Value < 0)
            {
                return null;
            }
            OnError = string.Empty;
            return db.Value;

        }

    }
}
