using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRBKWebApiData
{
    public class workPositionDA : Base
    {
        public workPositionDA() : base()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static workPosition GetWorkPosition(string _idPosition, string _user,string _containers)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_idPosition", _idPosition);
            parametros.Add("i_usuarioCrea", _user);
            var obj = sql_puntero.ExecuteSelectOnly<workPosition>(nueva_conexion, 4000, "mty.consultarWorkPosition", parametros);

            if (!string.IsNullOrEmpty(_containers))
            {
                var objDC = GetDataContainersExpo(_containers);

                if (obj == null)
                {
                    obj = new workPosition();
                }
                obj.dataContenedor = objDC;
            }

            return obj;
        }
        public static workPosition GetWorkPositionXUser( string _user)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_usuarioCrea", _user);
            var obj = sql_puntero.ExecuteSelectOnly<workPosition>(nueva_conexion, 4000, "mty.consultarPOWXUsuario", parametros);
            return obj;
        }
        public static List<workPositionN4List> GetListaPosicionesN4( out string OnError)
        {
            OnInit("N5");
            parametros.Clear();
            var obj = sql_puntero.ExecuteSelectControl<workPositionN4List>(nueva_conexion, 4000, "mty.consultarListaWorkPosition", parametros, out OnError);
            return obj;
        }

        public Int64? Save_Update(workPosition oPOW, out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_ip", oPOW.ip);
            parametros.Add("i_imei", oPOW.imei);
            parametros.Add("i_idPosition", oPOW.idPosition);
            parametros.Add("i_namePosition", oPOW.namePosition);
            parametros.Add("i_estado", oPOW.estado);
            parametros.Add("i_usuarioCrea", oPOW.usuarioCrea);

            var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "mty.insertarWorkPosition", parametros, out OnError);
            if (!db.HasValue || db.Value < 0)
            {
                return null;
            }

            OnError = string.Empty;
            return db.Value;
        }


        public static dataContainers GetDataContainersExpo(string _numcontainers)
        {
            OnInit("N5");
            parametros.Clear();
            parametros.Add("cntr", _numcontainers);
            var obj = sql_puntero.ExecuteSelectOnly<dataContainers>(nueva_conexion, 4000, "movil_load_data", parametros);
            return obj;
        }

        public static dataContainers GetDataContainersImpo(string _numcontainers)
        {
            OnInit("N5");
            parametros.Clear();
            parametros.Add("cntr", _numcontainers);
            var obj = sql_puntero.ExecuteSelectOnly<dataContainers>(nueva_conexion, 4000, "movil_disch_data", parametros);
            return obj;
        }
    }
}
