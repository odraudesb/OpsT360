using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;

namespace BRBKWebApiData
{
    public class BAN_Catalogo_UbicacionDA : Base
    {
        public BAN_Catalogo_UbicacionDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<BAN_Catalogo_Ubicacion> ConsultarLista(out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_Catalogo_Ubicacion>(nueva_conexion, 8000, "[BAN_Catalogo_Ubicacion_Consultar]", null, out OnError);
        }


        public static BAN_Catalogo_Ubicacion GetEntidad(int _id)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Catalogo_Ubicacion>(nueva_conexion, 4000, "[BAN_Catalogo_Ubicacion_Consultar]", parametros);
            return obj;
        }

        public static BAN_Catalogo_Ubicacion GetEntidad(string _barcode)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_barcode", _barcode);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Catalogo_Ubicacion>(nueva_conexion, 4000, "[BAN_Catalogo_Ubicacion_Consultar]", parametros);
            return obj;
        }

        public Int64? Save_Update(BAN_Catalogo_Ubicacion oEntidad, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_id", oEntidad.id);
            parametros.Add("i_idBodega", oEntidad.idBodega);
            parametros.Add("i_idBloque", oEntidad.idBloque);
            parametros.Add("i_idFila", oEntidad.idFila);
            parametros.Add("i_idAltura", oEntidad.idAltura);
            parametros.Add("i_idProfundidad", oEntidad.idProfundidad);
            parametros.Add("i_barcode", oEntidad.barcode);
            parametros.Add("i_descripcion", oEntidad.descripcion);
            parametros.Add("i_capacidadBox", oEntidad.capacidadBox);
            parametros.Add("i_mt2", oEntidad.mt2);
            parametros.Add("i_disponible", oEntidad.disponible);
            parametros.Add("i_estado", oEntidad.estado);
            parametros.Add("i_usuarioCrea", oEntidad.usuarioCrea);
            parametros.Add("i_usuarioModifica", oEntidad.usuarioModifica);
            

            var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[BAN_Catalogo_Ubicacion_Insertar]", parametros, out OnError);
            if (!db.HasValue || db.Value < 0)
            {
                return null;
            }
            OnError = string.Empty;
            return db.Value;

        }
    }
}
