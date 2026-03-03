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
    public class downloadConfirmationDA : Base
    {
        public downloadConfirmationDA() : base()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }
        public static downloadConfirmation GetDownloadConfirmationXId(long _id)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<downloadConfirmation>(nueva_conexion, 4000, "mty.consultarDownloadConfirmation", parametros);
            return obj;
        }

        public downloadConfirmation Save_Update(downloadConfirmation oDownload, out string OnError)
        {
            long v_id = -1;
            OnInit("N4Middleware");
          
                parametros.Clear();
                parametros.Add("i_gkey", oDownload.gkey);
                parametros.Add("i_container", oDownload.container);
                parametros.Add("i_dataContainer", oDownload.dataContainer);
                parametros.Add("i_position", oDownload.position);
                parametros.Add("i_referencia", oDownload.referencia);
                parametros.Add("i_xmlN4", oDownload.xmlN4);
                parametros.Add("i_respuestaN4", oDownload.respuestaN4);
                parametros.Add("i_estado", oDownload.estado);
                parametros.Add("i_usuarioCrea", oDownload.usuarioCrea);

                var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "mty.insertarDownloadConfirmation", parametros, out OnError);
                if (!db.HasValue || db.Value < 0)
                {
                    return null;
                }
                OnError = string.Empty;
                v_id = db.Value;
           

            return GetDownloadConfirmationXId(v_id); ;
        }
    }
}
