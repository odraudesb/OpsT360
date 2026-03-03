using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRBKWebApiData
{
    public class Valida_Paletizado : Base
    {
        public Valida_Paletizado()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<ValidaPaletizado> Validacion_Peletizado(string XmlContenedor, out string OnError)
        {
            OnInit("N5");
            parametros.Clear();
            parametros.Add("XmlContenedor", XmlContenedor);
            return sql_puntero.ExecuteSelectControl<ValidaPaletizado>(nueva_conexion, 6000, "CFS_VALIDA_SERVICIO_PALETIZADO", parametros, out OnError);
        }
    }
}
