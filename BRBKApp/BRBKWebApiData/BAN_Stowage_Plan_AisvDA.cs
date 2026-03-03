using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using BRBKWebApiData;

namespace BRBKWebApiData
{
    public class BAN_Stowage_Plan_AisvDA : Base
    {
        public BAN_Stowage_Plan_AisvDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<BAN_Stowage_Plan_Aisv> ConsultarLista(string estado, string aisv, long? idStowageDet, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_estado", estado);
            parametros.Add("i_id", null);
            parametros.Add("i_idStowageDet", idStowageDet);
            parametros.Add("i_aisv", aisv);
            return sql_puntero.ExecuteSelectControl<BAN_Stowage_Plan_Aisv>(nueva_conexion, 8000, "[BAN_Stowage_Plan_Aisv_Consultar]", parametros, out OnError);
        }

        public static List<BAN_Stowage_Plan_Aisv> ConsultarListaXAISV(string aisv, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_aisv", aisv);
            return sql_puntero.ExecuteSelectControl<BAN_Stowage_Plan_Aisv>(nueva_conexion, 8000, "[BAN_Stowage_Plan_Aisv_Consultar]", parametros, out OnError);
        }

        public static BAN_Stowage_Plan_Aisv GetEntidad(long? _id)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Stowage_Plan_Aisv>(nueva_conexion, 4000, "[BAN_Stowage_Plan_Aisv_Consultar]", parametros);
            return obj;
        }
        public static BAN_Stowage_Plan_Aisv GetEntidad(string booking)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_booking", booking);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Stowage_Plan_Aisv>(nueva_conexion, 4000, "[BAN_Stowage_Plan_Aisv_ConsultarXBooking]", parametros);
            return obj;
        }

        public Int64? Save_Update(BAN_Stowage_Plan_Aisv objeto, out string OnError)
        {
            //OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idStowageAisv", objeto.idStowageAisv);
            parametros.Add("i_idStowageDet", objeto.idStowageDet);
            parametros.Add("i_fecha", objeto.fecha);
            parametros.Add("i_idHoraInicio", objeto.idHoraInicio);
            parametros.Add("i_horaInicio", objeto.horaInicio);
            parametros.Add("i_idHoraFin", objeto.idHoraFin);
            parametros.Add("i_horaFin", objeto.horaFin);
            parametros.Add("i_box", objeto.box);
            parametros.Add("i_comentario", objeto.comentario);
            parametros.Add("i_aisv", objeto.aisv);
            parametros.Add("i_dae", objeto.dae);
            parametros.Add("i_booking", objeto.booking);
            parametros.Add("i_IIEAutorizada", objeto.IIEAutorizada);
            parametros.Add("i_daeAutorizada", objeto.daeAutorizada);
            parametros.Add("i_placa", objeto.placa);
            parametros.Add("i_idChofer", objeto.idChofer);
            parametros.Add("i_chofer", objeto.chofer);
            parametros.Add("i_idCapacidadHoraBodega", objeto.idCapacidadHoraBodega);
            parametros.Add("i_estado", objeto.estado);
            parametros.Add("i_usuarioCrea", objeto.usuarioCrea);
            parametros.Add("i_usuarioModifica", objeto.usuarioModifica);

            var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[BAN_Stowage_Plan_Aisv_Insertar]", parametros, out OnError);
            if (!db.HasValue || db.Value < 0)
            {
                return null;
            }
            OnError = string.Empty;
            return db.Value;
        }
    }

    public class BAN_AISV_GeneradosDA : Base
    {
        public BAN_AISV_GeneradosDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<BAN_AISV_Generados> ConsultarListadoAISV(string codigo, out string OnError)
        {
            OnInit("Portal_Servicios");
            parametros.Clear();
            parametros.Add("i_aisv_codigo", codigo);
            return sql_puntero.ExecuteSelectControl<BAN_AISV_Generados>(nueva_conexion, 8000, "[BAN_AISV_consultar_X_AISV]", parametros, out OnError);
        }
    }
}
