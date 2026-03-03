using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using BRBKWebApiData;

namespace BRBKWebApiData
{
    public class BAN_Stowage_Plan_CabDA : Base
    {
        public BAN_Stowage_Plan_CabDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<BAN_Stowage_Plan_Cab> ConsultarLista(out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_Stowage_Plan_Cab>(nueva_conexion, 8000, "[BAN_Stowage_Plan_Cab_Consultar]", null, out OnError);
        }


        public static BAN_Stowage_Plan_Cab GetEntidad(long? _id)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idStowageCab", _id);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Stowage_Plan_Cab>(nueva_conexion, 4000, "[BAN_Stowage_Plan_Cab_Consultar]", parametros);
            return obj;
        }

        public static BAN_Stowage_Plan_Cab GetEntidad(string _idNave)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idNave", _idNave);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Stowage_Plan_Cab>(nueva_conexion, 4000, "[BAN_Stowage_Plan_Cab_Consultar_x_Nave]", parametros);
            return obj;
        }
    }
}
