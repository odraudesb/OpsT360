using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRBKWebApiData
{
    public class pasePuertaDA : Base
    {
        public pasePuertaDA() : base()
        {
            //init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static pasePuerta GetPasePuerta(string _numeroPase)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_numeroPase", _numeroPase);
            var obj = sql_puntero.ExecuteSelectOnly<pasePuerta>(nueva_conexion, 4000, "[brbk].consultarPasePuerta", parametros);
            try
            {
                obj.tarjaDet = tarjaDetDA.GetTarjaDet(long.Parse(obj.idTarjaDet.ToString()));
            }
            catch { }

            return obj;
        }
    }
}
