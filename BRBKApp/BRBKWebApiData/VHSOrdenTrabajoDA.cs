using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BRBKWebApiData
{
    [Serializable]
    public class VHSOrdenTrabajoDA : Base
    {
        private static string _dbname = "N4Middleware";
        public VHSOrdenTrabajoDA() : base()
        {
            
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<VHSOrdenTrabajo> GetOrdenes(out string OnError)
        {
            OnInit(_dbname);
            parametros.Clear();

            List<VHSOrdenTrabajo> list = sql_puntero.ExecuteSelectControl<VHSOrdenTrabajo>(nueva_conexion, 4000, "[vhs].lista_order_trabajo_pendientes", parametros, out OnError);
            
            return list;
        }
    }
}
