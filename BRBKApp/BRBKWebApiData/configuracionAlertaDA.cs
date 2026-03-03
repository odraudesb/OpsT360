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
    public class configuracionAlertaDA : Base
    {
        public configuracionAlertaDA() : base()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<configuracionAlerta> listadoConfigNotificaAlertas(out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<configuracionAlerta>(nueva_conexion, 4000, "[brbk].[consultarConfiguracionAlerta]", parametros, out OnError);
        }

        public static configuracionAlerta GetConfigNotificaAlertas(long _id)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<configuracionAlerta>(nueva_conexion, 4000, "[brbk].[consultarConfiguracionAlerta]", parametros);
            try { obj.grupoMail = grupoMailDA.GetGrupos(obj.idGrupoMail); } catch { }
            return obj;
        }
    }
}
