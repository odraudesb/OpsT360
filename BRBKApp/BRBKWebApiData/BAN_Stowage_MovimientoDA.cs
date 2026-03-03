using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;


namespace BRBKWebApiData
{
    public class BAN_Stowage_MovimientoDA : Base
    {
        public BAN_Stowage_MovimientoDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<BAN_Stowage_Movimiento> ConsultarLista(long idStowageAisv, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idStowageAisv", idStowageAisv);
            return sql_puntero.ExecuteSelectControl<BAN_Stowage_Movimiento>(nueva_conexion, 8000, "[BAN_Stowage_Movimiento_Consultar]", parametros, out OnError);
        }


        public static BAN_Stowage_Movimiento GetEntidad(long _id)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Stowage_Movimiento>(nueva_conexion, 4000, "[BAN_Stowage_Movimiento_Consultar]", parametros);
            return obj;
        }

        public static BAN_Stowage_Movimiento GetEntidad(string _barcode)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_barcode", _barcode);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Stowage_Movimiento>(nueva_conexion, 4000, "[BAN_Stowage_Movimiento_Consultar]", parametros);
            return obj;
        }

        public static List<BAN_Stowage_Movimiento> ConsultarListaCargaBodega(string idNave, int? idBodega, int? idExportador, string booking, string barcode, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idNave", idNave);
            parametros.Add("i_idBodega", idBodega);
            parametros.Add("i_idExportador", idExportador);
            parametros.Add("i_booking", booking);
            parametros.Add("i_barcode", barcode);
            return sql_puntero.ExecuteSelectControl<BAN_Stowage_Movimiento>(nueva_conexion, 8000, "[BAN_Stowage_Movimiento_CheckLoad_Consultar]", parametros, out OnError);
        }

        public static List<BAN_Stowage_Movimiento> ConsultarListaCargaParaDespacho(string idNave, int? idExportador, int? idBloque, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idNave", idNave);
            parametros.Add("i_idExportador", idExportador);
            parametros.Add("i_idBloque", idBloque);
            return sql_puntero.ExecuteSelectControl<BAN_Stowage_Movimiento>(nueva_conexion, 8000, "[BAN_Stowage_Movimiento_ParaDespacho_Consultar]", parametros, out OnError);
        }

        public static BAN_Stowage_Movimiento ConsultarListaCargaParaDespachoEsp(string idNave, int? idExportador, int? idBloque, string booking)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idNave", idNave);
            parametros.Add("i_idExportador", idExportador);
            parametros.Add("i_idBloque", idBloque);
            parametros.Add("i_booking", booking);
            return sql_puntero.ExecuteSelectOnly<BAN_Stowage_Movimiento>(nueva_conexion, 4000, "BAN_Stowage_Movimiento_ParaDespacho_Consultar", parametros);
        }


        public Int64? Save_Update(BAN_Stowage_Movimiento oRec, out string OnError)
        {
            OnInit("VBS");
            using (var scope = new System.Transactions.TransactionScope())
            {
                parametros.Clear();
                parametros.Add("i_idMovimiento", oRec.idMovimiento);
                parametros.Add("i_idStowageAisv", oRec.idStowageAisv);
                parametros.Add("i_idUbicacion", oRec.idUbicacion);
                parametros.Add("i_barcode", oRec.barcode);
                parametros.Add("i_idModalidad", oRec.idModalidad);
                parametros.Add("i_tipo", oRec.tipo);
                parametros.Add("i_cantidad", oRec.cantidad);
                parametros.Add("i_observacion", oRec.observacion);
                parametros.Add("i_estado", oRec.estado);
                parametros.Add("i_usuarioCrea", oRec.usuarioCrea);
                parametros.Add("i_usuarioModifica", oRec.usuarioModifica);
                parametros.Add("i_isMix", oRec.isMix);
                parametros.Add("i_referencia", oRec.referencia);

                var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[BAN_Stowage_Movimiento_Insertar]", parametros, out OnError);
                if (!db.HasValue || db.Value < 0)
                {
                    return null;
                }
                OnError = string.Empty;

                if (oRec.Fotos != null)
                {
                    foreach (var oItem in oRec.Fotos)
                    {
                        oItem.idMovimiento = db.Value;
                        BAN_Stowage_Movimiento_FotoDA oFoto = new BAN_Stowage_Movimiento_FotoDA();
                        var dbItem = oFoto.Save_Update(oItem, out OnError);

                        if (!dbItem.HasValue || dbItem.Value < 0)
                        {
                            return null;
                        }
                    }
                }
                scope.Complete();
                return db.Value;
            }
        }

        //Pre Despacho
        public static List<BAN_Stowage_Movimiento> ConsultarListaPorNoOrdenYFila(long idOrdenDespacho, int idFila, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idOrdenDespacho", idOrdenDespacho);
            parametros.Add("i_idFila", idFila);
            return sql_puntero.ExecuteSelectControl<BAN_Stowage_Movimiento>(nueva_conexion, 8000, "[BAN_Stowage_MovimientoPorOrden_Consultar]", parametros, out OnError);
        }
        public Int64? Save_PreDespacho(BAN_Stowage_Movimiento oRec, out string OnError)
        {
            OnInit("VBS");
            using (var scope = new System.Transactions.TransactionScope())
            {
                parametros.Clear();
                parametros.Add("i_idMovimiento", oRec.idMovimiento);
                parametros.Add("i_usuarioDespacho", oRec.usuarioCrea);

                var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[BAN_Stowage_Movimiento_Update_Predespachar]", parametros, out OnError);
                if (!db.HasValue || db.Value < 0)
                {
                    return null;
                }
                OnError = string.Empty;
                scope.Complete();
                return db.Value;
            }
        }

        public Int64? Save_anulacion_Movimiento(BAN_Stowage_Movimiento oRec, out string OnError)
        {
            OnInit("VBS");
            using (var scope = new System.Transactions.TransactionScope())
            {
                parametros.Clear();
                parametros.Add("i_idMovimiento", oRec.idMovimiento);
                parametros.Add("i_usuarioAnulacion", oRec.usuarioModifica);

                var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[BAN_Stowage_Movimiento_anular]", parametros, out OnError);
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

