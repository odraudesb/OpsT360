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
    public class pesajeDA : Base
    {
        public pesajeDA() : base()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }
        public static pesaje GetPesaje(long _id)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<pesaje>(nueva_conexion, 4000, "cgsa40.consultarPesajes", parametros);
            return obj;
        }

        public static pesaje GetPesajePorId(long? _id)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<pesaje>(nueva_conexion, 4000, "[cgsa40].[consultarPesajePorId]", parametros);
            return obj;
        }
        
        public Int64? Save_Update(pesaje objeto, out string OnError)
        {
            OnInit("N4Middleware");

            //using (var scope = new System.Transactions.TransactionScope())
            //{
                //SP N5 
                string v_mensaje = string.Empty;
                //parametros.Clear();
                //parametros.Add("PSI_USUARIO", objeto.usuarioCrea);
                //parametros.Add("PSI_IP", objeto.ip);
                //parametros.Add("PSI_MAQUINA", objeto.ip);
                //parametros.Add("PSI_CONTENEDOR", objeto.container);
                //parametros.Add("PSI_PESO", objeto.peso);
                //parametros.Add ("PSO_MENSAJE", v_mensaje);

                //var n5 = sql_puntero.ExecuteSelectOnlyString(nueva_conexion, 6000, "PES_PRO_SAVE_WEIGH_STAKER_REP", parametros, out OnError);
                //v_mensaje = n5.ToString();

                parametros.Clear();
                parametros.Add("PSI_USUARIO", objeto.usuarioCrea);
                parametros.Add("PSI_IP", objeto.ip);
                parametros.Add("PSI_MAQUINA", objeto.ip);
                parametros.Add("PSI_CONTENEDOR", objeto.container);
                parametros.Add("PSI_PESO", objeto.peso);
                //parametros.Add("PSO_MENSAJE", v_mensaje);

                var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "PES_PRO_SAVE_WEIGH_STAKER_REP", parametros, out OnError);
                if (!db.HasValue || db.Value < 0)
                {
                    return null;
                }
                OnError = string.Empty;
                
                //scope.Complete();
                return db.Value;
            //}

        }
    }
}
