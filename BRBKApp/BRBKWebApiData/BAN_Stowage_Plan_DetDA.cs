using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using BRBKWebApiData;

namespace BRBKWebApiData
{
    public class BAN_Stowage_Plan_DetDA : Base
    {
        public BAN_Stowage_Plan_DetDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<BAN_Stowage_Plan_Det> ConsultarLista(long idStowageCab , out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idStowageCab", idStowageCab);
            return sql_puntero.ExecuteSelectControl<BAN_Stowage_Plan_Det>(nueva_conexion, 8000, "[BAN_Stowage_Plan_Det_Consultar]", parametros, out OnError);
        }


        public static BAN_Stowage_Plan_Det GetEntidad(long? _id)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Stowage_Plan_Det>(nueva_conexion, 4000, "[BAN_Stowage_Plan_Det_Consultar]", parametros);
            return obj;
        }

        public Int64? SaveAisvExterno(BAN_Stowage_Plan_Det objeto, out string OnError, out long? idAisv)
        {
            idAisv = null;
            OnInit("VBS");
            using (var scope = new System.Transactions.TransactionScope())
            {

                parametros.Clear();
                parametros.Add("i_idStowageDet", objeto.idStowageDet);
                parametros.Add("i_idStowageCab", objeto.idStowageCab);
                parametros.Add("i_idHold", objeto.idHold);
                parametros.Add("i_piso", objeto.piso);
                parametros.Add("i_boxSolicitado", objeto.boxSolicitado);
                parametros.Add("i_idCargo", objeto.idCargo);
                parametros.Add("i_idExportador", objeto.idExportador);
                parametros.Add("i_idMarca", objeto.idMarca);
                parametros.Add("i_idConsignatario", objeto.idConsignatario);
                parametros.Add("i_idBodega", objeto.idBodega);
                parametros.Add("i_idBloque", objeto.idBloque);
                parametros.Add("i_boxAutorizado", objeto.boxAutorizado);
                parametros.Add("i_reservado", objeto.reservado);
                parametros.Add("i_disponible", objeto.disponible);
                parametros.Add("i_comentario", objeto.comentario);
                parametros.Add("i_fechaDocumento", objeto.fechaDocumento);
                parametros.Add("i_estado", objeto.estado);
                parametros.Add("i_usuarioCrea", objeto.usuarioCrea);
                parametros.Add("i_usuarioModifica", objeto.usuarioModifica);

                var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[BAN_Stowage_Plan_Det_Insertar]", parametros, out OnError);
                if (!db.HasValue || db.Value < 0)
                {
                    return null;
                }

                OnError = string.Empty;

                if (objeto.ListaAISV != null)
                {
                    foreach (var oItem in objeto.ListaAISV)
                    {
                        oItem.idStowageDet = db.Value;
                        BAN_Stowage_Plan_Aisv eAisv = new BAN_Stowage_Plan_Aisv();
                        eAisv.idStowageDet = db.Value;
                        eAisv.fecha = DateTime.Now;
                        eAisv.idHoraInicio = 1;
                        eAisv.horaInicio = "00:00";
                        eAisv.idHoraFin = 1;
                        eAisv.horaFin = "01:00";
                        eAisv.box = oItem.box;
                        eAisv.comentario = "INGRESO AUTOMÁTICO DE AISV (APP) DE OTRA PROCEDENCIA";
                        eAisv.aisv = oItem.aisv;
                        eAisv.dae = oItem.dae.Trim();
                        eAisv.booking = oItem.booking.Trim();
                        eAisv.IIEAutorizada = true;
                        eAisv.daeAutorizada = true;
                        eAisv.placa = oItem.placa;
                        eAisv.idChofer = oItem.idChofer;
                        eAisv.chofer = oItem.chofer;
                        eAisv.estado = "ING";
                        eAisv.usuarioCrea = oItem.usuarioCrea;

                        BAN_Stowage_Plan_AisvDA oAisv = new BAN_Stowage_Plan_AisvDA();
                        var dbItem = oAisv.Save_Update(eAisv, out OnError);

                        if (!dbItem.HasValue || dbItem.Value < 0)
                        {
                            return null;
                        }
                        eAisv.idStowageAisv = long.Parse(dbItem.ToString());
                        idAisv = eAisv.idStowageAisv;
                    }
                }
                scope.Complete();
                return db.Value;
            }
        }
    }
}
