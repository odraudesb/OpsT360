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
    public class sealAssignsExpoDA : Base
    {
        public sealAssignsExpoDA() : base()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static AsignaSealExpo GetSelloPorId(long? _id)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<AsignaSealExpo>(nueva_conexion, 4000, "seal.[consultarSealAssignExpoPorId]", parametros);
            return obj;
        }

        public static bool? GetExigeFoto(string _code, out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_code", _code);
            var obj = sql_puntero.ExecuteSelectOnlyBool(nueva_conexion, 4000, "seal.exigeFoto", parametros, out OnError);
            return obj;
        }

        public Int64? Save_Update(AsignaSealExpo oSeal, out string OnError)
        {
            //long v_id = -1;
            OnInit("N4Middleware");

            //SP N5 
            string v_mensaje = string.Empty;
            parametros.Clear();
            parametros.Add("i_container", oSeal.container);
            parametros.Add("i_selloCGSA", oSeal.sello_CGSA);
            parametros.Add("i_sello1", oSeal.sello1);
            parametros.Add("i_sello2", oSeal.sello2);
            parametros.Add("i_sello3", oSeal.sello3);
            parametros.Add("i_sello4", oSeal.sello4);
            parametros.Add("i_ip", oSeal.ip);
            parametros.Add("i_usuarioCrea", oSeal.usuarioCrea);

            var n5 = sql_puntero.ExecuteSelectOnlyString(nueva_conexion, 6000, "seal.ASIGNA_SEAL_EXPO_4S_MOVIL", parametros, out OnError);
            v_mensaje = n5.ToString();

            //using (var scope = new System.Transactions.TransactionScope())
            //{
                parametros.Clear();
                parametros.Add("i_gkey", oSeal.gkey);
                parametros.Add("i_container", oSeal.container);
                parametros.Add("i_selloCGSA", oSeal.sello_CGSA);
                parametros.Add("i_sello1", oSeal.sello1);
                parametros.Add("i_sello2", oSeal.sello2);
                parametros.Add("i_sello3", oSeal.sello3);
                parametros.Add("i_sello4", oSeal.sello4);
                parametros.Add("i_ip", oSeal.ip);
                parametros.Add("i_usuarioCrea", oSeal.usuarioCrea);
                parametros.Add("i_mensajeOUT", v_mensaje);


                var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "seal.[insertarSealAssignExpo]", parametros, out OnError);
                if (!db.HasValue || db.Value < 0)
                {
                    return null;
                }
                OnError = string.Empty;

                foreach (var oItem in oSeal.Fotos)
                {
                    oItem.idSealValidation = db.Value;
                    fotoSealAssignsExpoDA oFoto = new fotoSealAssignsExpoDA();
                    var dbItem = oFoto.Save_Update(oItem, out OnError);

                    if (!dbItem.HasValue || dbItem.Value < 0)
                    {
                        return null;
                    }
                }
                //scope.Complete();
                return db.Value;
            //}

            //return GetSelloMuellePorId(v_id); ;
        }
    }


    public class fotoSealAssignsExpoDA : Base
    {
        public fotoSealAssignsExpoDA() : base()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }


        public Int64? Save_Update(fotoSealValidation oFoto, out string OnError)
        {
            //if (oFoto.id > 0)
            //{
                OnInit("N4Middleware");
            //}
            parametros.Clear();
            parametros.Add("i_id", oFoto.id);
            parametros.Add("i_idSealAssignExpo", oFoto.idSealValidation);
            parametros.Add("i_ruta", oFoto.ruta);
            parametros.Add("i_estado", oFoto.estado);
            parametros.Add("i_usuarioCrea", oFoto.usuarioCrea);
            parametros.Add("i_usuarioModifica", oFoto.usuarioModifica);

            var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[seal].[insertarFotoSealAssignExpo]", parametros, out OnError);
            if (!db.HasValue || db.Value < 0)
            {
                return null;
            }
            OnError = string.Empty;
            return db.Value;
        }
    }
}
