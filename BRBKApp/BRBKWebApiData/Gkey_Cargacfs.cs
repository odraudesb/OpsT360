using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRBKWebApiData
{
    public class Gkey_Cargacfs : Base
    {
        public Gkey_Cargacfs() : base()
        {
            //init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static gkey GetGkey(string numero_carga)
        {
            OnInit("N5");
            parametros.Clear();
            parametros.Add("MRN_MSN_HSN ", numero_carga);
            var obj = sql_puntero.ExecuteSelectOnly<gkey>(nueva_conexion, 6000, "FNA_FUN_BREAKBULK_GKEY", parametros);
            
            return obj;
        }

    }
}
