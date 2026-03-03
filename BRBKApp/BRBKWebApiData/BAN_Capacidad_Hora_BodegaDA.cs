using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;

namespace BRBKWebApiData
{
    public class BAN_Capacidad_Hora_BodegaDA : Base
    {
        public BAN_Capacidad_Hora_BodegaDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<BAN_Capacidad_Hora_Bodega> ConsultarListadoCapacidadPorNave(string idNave, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idNave", idNave);
            return sql_puntero.ExecuteSelectControl<BAN_Capacidad_Hora_Bodega>(nueva_conexion, 8000, "[BAN_Capacidad_Hora_Bodega_Consultar]", parametros, out OnError);
        }

        public static List<BAN_Capacidad_Hora_Bodega> ConsultarConfiguracionCapacidadPorNave(string idNave, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idNave", idNave);
            return sql_puntero.ExecuteSelectControl<BAN_Capacidad_Hora_Bodega>(nueva_conexion, 8000, "[BAN_Capacidad_Hora_Bodega_Consultar]", parametros, out OnError);
        }

        public static BAN_Capacidad_Hora_Bodega GetCapacidadHoraEspecifico(long _id)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Capacidad_Hora_Bodega>(nueva_conexion, 4000, "[]", parametros);

            return obj;
        }

        public Int64? Save_Update(BAN_Capacidad_Hora_Bodega oEntidad , out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_id", oEntidad.id);
            parametros.Add("i_idNave", oEntidad.idNave);
            parametros.Add("i_nave", oEntidad.nave);
            parametros.Add("i_idHoraInicio", oEntidad.idHoraInicio);
            parametros.Add("i_horaInicio", oEntidad.horaInicio);
            parametros.Add("i_idHoraFin", oEntidad.idHoraFin);
            parametros.Add("i_horaFin", oEntidad.horaFin);
            parametros.Add("i_idBodega", oEntidad.idBodega);
            parametros.Add("i_idBloque", oEntidad.idBloque);
            parametros.Add("i_box", oEntidad.box);
            parametros.Add("@i_disponible", oEntidad.disponible);
            parametros.Add("i_usuarioCrea", oEntidad.usuarioCrea);
            parametros.Add("i_usuarioModifica", oEntidad.usuarioModifica);
            parametros.Add("i_estado", oEntidad.estado);

            var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[BAN_Capacidad_Hora_Bodega_Insertar]", parametros, out OnError);
            if (!db.HasValue || db.Value < 0)
            {
                return null;
            }
            OnError = string.Empty;
            return db.Value;
        }

        public static List<BAN_Capacidad_Hora_Bodega> Save_Update(string xmlDatos, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_Datos", xmlDatos);
            return sql_puntero.ExecuteSelectControl<BAN_Capacidad_Hora_Bodega>(nueva_conexion, 8000, "[]", parametros, out OnError);
        }
    }
}

