using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRBKWebApiData
{
    public class tarjaCabDA : Base
    {
        public tarjaCabDA(): base()
        {
            //init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = null;
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<tarjaCab> listadotarjaCab(string _estado,out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_estado", _estado);
            return sql_puntero.ExecuteSelectControl<tarjaCab>(nueva_conexion, 4000, "[brbk].consultartarjaCab", parametros, out OnError);
        }

        public static tarjaCab GetTarjaCab(long _id)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<tarjaCab>(nueva_conexion, 4000, "[brbk].consultartarjaCab", parametros);
            if (obj != null)
            {
                try { obj.Estados = estadosDA.GetEstado(obj.estado); } catch { }
            }
           
            return obj;
        }

        public static tarjaCab GetTarjaCab(string _nave,string _idAgente, string _mrn, out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_nave", _nave);
            parametros.Add("i_idAgente", _idAgente);
            parametros.Add("i_mrn", _mrn);
            var obj = sql_puntero.ExecuteSelectOnly<tarjaCab>(nueva_conexion, 4000, "[brbk].consultartarjaCabEsp", parametros);
            if (obj != null)
            {
                try { obj.Estados = estadosDA.GetEstado(obj.estado);
                    obj.Detalle = tarjaDetDA.listadoTarjaDet(long.Parse(obj.idTarja.ToString()), out OnError);
                } catch { OnError = ""; }
                
            }
            else
            {
                OnError = "";
            }
            
            return obj;
        }

    }
}
