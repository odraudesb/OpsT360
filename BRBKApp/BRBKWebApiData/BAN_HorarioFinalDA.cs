using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRBKWebApiData
{
    public class BAN_HorarioFinalDA : Base
    {
        public BAN_HorarioFinalDA()
        {
            init();
        }
        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }
        public static List<BAN_HorarioFinal> ConsultarHorarioFinal(out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_HorarioFinal>(nueva_conexion, 8000, "BAN_CONSULTAR_HORARIO_FIN", null, out OnError);
        }

        public static BAN_HorarioFinal GetHorarioFinal(int _id)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_id", _id);
            return sql_puntero.ExecuteSelectOnly<BAN_HorarioFinal>(nueva_conexion, 4000, "BAN_CONSULTAR_HORARIO_FIN_X_ID", parametros);
        }
    }
}
