using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRBKWebApiData
{
    public class naveDA : Base
    {
        public naveDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static nave GetNave(string _nave)
        {
            OnInit("N5");
            parametros.Clear();
            parametros.Add("i_nave", _nave);
            return sql_puntero.ExecuteSelectOnly<nave>(nueva_conexion, 4000, "[brbk].consultarDataNave", parametros);
        }

    }
}
