using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;

namespace BRBKWebApiData
{
    public class BAN_Stowage_OrdenDespachoDA : Base
    {
        public BAN_Stowage_OrdenDespachoDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<BAN_Stowage_OrdenDespacho> ConsultarLista(string idNave, int? idExportador, int? idBloque, string booking, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idNave", idNave);
            parametros.Add("i_idExportador", idExportador);
            parametros.Add("i_idBloque", idBloque);
            parametros.Add("i_booking", booking);
            return sql_puntero.ExecuteSelectControl<BAN_Stowage_OrdenDespacho>(nueva_conexion, 8000, "[BAN_Stowage_OrdenDespacho_Consultar]", parametros, out OnError);
        }

        public static BAN_Stowage_OrdenDespacho GetEntidad(long _id)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Stowage_OrdenDespacho>(nueva_conexion, 4000, "[BAN_Stowage_OrdenDespacho_Consultar]", parametros);
            return obj;
        }

        public static List<BAN_Stowage_OrdenDespacho> ConsultarListaPorBodega(int? idBodega, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idBodega", idBodega);
            return sql_puntero.ExecuteSelectControl<BAN_Stowage_OrdenDespacho>(nueva_conexion, 8000, "[BAN_Stowage_OrdenDespacho_Consultar]", parametros, out OnError);
        }

        public Int64? Save_Update(BAN_Stowage_OrdenDespacho oEntidad, out string OnError)
        {
            OnInit("VBS");
            using (var scope = new System.Transactions.TransactionScope())
            {
                parametros.Clear();
                parametros.Add("i_id", oEntidad.idOrdenDespacho);
                parametros.Add("i_idNave", oEntidad.idNave);
                parametros.Add("i_idExportador", oEntidad.idExportador);
                parametros.Add("i_idBodega", oEntidad.idBodega);
                parametros.Add("i_idBloque", oEntidad.idBloque);
                parametros.Add("i_cantidadPalets", oEntidad.cantidadPalets);
                parametros.Add("i_cantidadBox", oEntidad.cantidadBox);
                parametros.Add("i_arrastre", oEntidad.arrastre);
                parametros.Add("i_pendiente", oEntidad.pendiente);
                parametros.Add("i_estado", oEntidad.estado);
                parametros.Add("i_usuarioCrea", oEntidad.usuarioCrea);
                parametros.Add("i_usuarioModifica", oEntidad.usuarioModifica);
                parametros.Add("i_booking", oEntidad.booking);

                var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[BAN_Stowage_OrdenDespacho_Insertar]", parametros, out OnError);
                if (!db.HasValue || db.Value < 0)
                {
                    return null;
                }
                OnError = string.Empty;

               
                scope.Complete();
                return db.Value;
            }
        }

    }
}
