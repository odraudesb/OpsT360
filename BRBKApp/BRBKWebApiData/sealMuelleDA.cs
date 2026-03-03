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
    public class sealMuelleDA : Base
    {
        public sealMuelleDA() : base()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }
        public static sealAsignacionMuelle GetSelloMuelle(long _id)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<sealAsignacionMuelle>(nueva_conexion, 4000, "mty.consultarSealMuelle", parametros);
            return obj;
        }

        //public static sealAsignacionMuelle GetSelloMuellePorId(long _id)
        //{
        //    OnInit("N4Middleware");
        //    parametros.Clear();
        //    parametros.Add("i_id", _id);
        //    var obj = sql_puntero.ExecuteSelectOnly<sealAsignacionMuelle>(nueva_conexion, 4000, "mty.consultarSealMuellePorId", parametros);
        //    return obj;
        //}

        public static sealAsignacionMuelle GetSelloMuellePorId(long? _id)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<sealAsignacionMuelle>(nueva_conexion, 4000, "mty.consultarSealMuellePorId", parametros);
            return obj;
        }
        //public sealAsignacionMuelle Save_Update(sealAsignacionMuelle oSeal, out string OnError)
        //{
        //    long v_id = -1;
        //    OnInit("N4Middleware");
        //    using (var scope = new System.Transactions.TransactionScope())
        //    {
        //        parametros.Clear();
        //        parametros.Add("i_referencia", oSeal.referencia);
        //        parametros.Add("i_gkey", oSeal.gkey);
        //        parametros.Add("i_container", oSeal.container);
        //        parametros.Add("i_sello_CGSA", oSeal.sello_CGSA);
        //        parametros.Add("i_sello1", oSeal.sello1);
        //        parametros.Add("i_sello2", oSeal.sello2);
        //        parametros.Add("i_sello3", oSeal.sello3);
        //        parametros.Add("i_sello4", oSeal.sello4);
        //        parametros.Add("i_color", oSeal.color);
        //        parametros.Add("i_ip", oSeal.ip);
        //        parametros.Add("i_usuarioCrea", oSeal.usuarioCrea);
        //        parametros.Add("i_dataContainer", oSeal.dataContainer);
        //        parametros.Add("i_position", oSeal.position);
        //        parametros.Add("i_xmlN4Discharge", oSeal.xmlN4Discharge);
        //        parametros.Add("i_respuestaN4Discharge", oSeal.respuestaN4Discharge);

        //        var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "mty.insertarSealMuelle", parametros, out OnError);
        //        if (!db.HasValue || db.Value < 0)
        //        {
        //            return null;
        //        }
        //        OnError = string.Empty;

        //        foreach (var oItem in oSeal.Fotos)
        //        {
        //            oItem.idSealValidation = db.Value;
        //            fotoSealMuelleDA oFoto = new fotoSealMuelleDA();
        //            var dbItem = oFoto.Save_Update(oItem, out OnError);

        //            if (!dbItem.HasValue || dbItem.Value < 0)
        //            {
        //                return null;
        //            }
        //        }
        //        scope.Complete();
        //        v_id = db.Value;
        //    }

        //    return GetSelloMuellePorId(v_id); ;
        //}

        public Int64? Save_Update(sealAsignacionMuelle oSeal, string mensajeOut, bool diferencia, out string OnError)
        {
            //long v_id = -1;
            OnInit("N4Middleware");
            //using (var scope = new System.Transactions.TransactionScope())
            //{
            parametros.Clear();
            parametros.Add("i_referencia", oSeal.referencia);
            parametros.Add("i_gkey", oSeal.gkey);
            parametros.Add("i_container", oSeal.container);
            parametros.Add("i_sello_CGSA", oSeal.sello_CGSA);
            parametros.Add("i_sello1", oSeal.sello1);
            parametros.Add("i_sello2", oSeal.sello2);
            parametros.Add("i_sello3", oSeal.sello3);
            parametros.Add("i_sello4", oSeal.sello4);
            parametros.Add("i_color", oSeal.color);
            parametros.Add("i_ip", oSeal.ip);
            parametros.Add("i_usuarioCrea", oSeal.usuarioCrea);
            parametros.Add("i_dataContainer", oSeal.dataContainer);
            parametros.Add("i_position", oSeal.position);
            parametros.Add("i_xmlN4Discharge", oSeal.xmlN4Discharge);
            parametros.Add("i_respuestaN4Discharge", oSeal.respuestaN4Discharge);
            parametros.Add("mensajeOUT", mensajeOut);
            parametros.Add("diferenciaOUT", diferencia);

            var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "mty.insertarSealMuelle", parametros, out OnError);
            if (!db.HasValue || db.Value < 0)
            {
                return null;
            }
            OnError = string.Empty;

            //foreach (var oItem in oSeal.Fotos)
            //{
            //    oItem.idSealValidation = db.Value;
            //    fotoSealMuelleDA oFoto = new fotoSealMuelleDA();
            //    var dbItem = oFoto.Save_Update(oItem, out OnError);

            //    if (!dbItem.HasValue || dbItem.Value < 0)
            //    {
            //        return null;
            //    }
            //}
            //scope.Complete();
            return db.Value;
            //}

            //return GetSelloMuellePorId(v_id); ;
        }

        public Int64? UpdateStatus(long id, bool estado, string comentario, string user, out string OnError)
        {
            //long v_id = -1;
            OnInit("N4Middleware");
            //using (var scope = new System.Transactions.TransactionScope())
            //{
            parametros.Clear();
            parametros.Add("i_id", id);
            parametros.Add("i_comentario", comentario);
            parametros.Add("i_usuario", user);
            parametros.Add("i_estado", estado);

            var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[mty].[updateStatusSealMuelle]", parametros, out OnError);
            if (!db.HasValue || db.Value < 0)
            {
                return null;
            }
            OnError = string.Empty;

            //foreach (var oItem in oSeal.Fotos)
            //{
            //    oItem.idSealValidation = db.Value;
            //    fotoSealMuelleDA oFoto = new fotoSealMuelleDA();
            //    var dbItem = oFoto.Save_Update(oItem, out OnError);

            //    if (!dbItem.HasValue || dbItem.Value < 0)
            //    {
            //        return null;
            //    }
            //}
            //scope.Complete();
            return db.Value;
            //}

            //return GetSelloMuellePorId(v_id); ;
        }

        public static DataTable consultaDataContenedorSealMuelle(string contenedor)
        {
            OnInit("N5");
            SqlConnection cn = new SqlConnection(nueva_conexion);

            var d = new DataTable();
            using (var c = cn)
            {
                var coman = c.CreateCommand();
                coman.CommandType = CommandType.StoredProcedure;
                coman.CommandText = "FNA_FUN_CONTAINERS_SEAL_MUELLE";
                coman.Parameters.AddWithValue("@unit", contenedor);
                try
                {
                    c.Open();
                    d.Load(coman.ExecuteReader(CommandBehavior.CloseConnection));
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    if (c.State == ConnectionState.Open)
                    {
                        c.Close();
                    }
                    c.Dispose();
                }
            }
            return d;
        }

        public static DataTable asignaSealMovil(sealAsignacionMuelle oSeal, out string mensaje, out bool diferencia, long Gkey)
        {
            OnInit("N4Middleware");
            SqlConnection cn = new SqlConnection(nueva_conexion);

            var d = new DataTable();
            using (var c = cn)
            {
                var coman = c.CreateCommand();
                coman.CommandType = CommandType.StoredProcedure;
                coman.CommandText = "[mty].[SEL_PRO_ASIGNA_SEAL_BER_MOVIL_V2]";
                coman.Parameters.AddWithValue("@PSI_CONTAINER", oSeal.container);
                coman.Parameters.AddWithValue("@PSI_SEAL_CGSA", oSeal.sello_CGSA);
                coman.Parameters.AddWithValue("@PSI_SEAL_1", oSeal.sello1);
                coman.Parameters.AddWithValue("@PSI_SEAL_2", oSeal.sello2);
                coman.Parameters.AddWithValue("@PSI_SEAL_3", oSeal.sello3);
                coman.Parameters.AddWithValue("@PSI_SEAL_4", oSeal.sello4);
                coman.Parameters.AddWithValue("@PSI_COLOR", oSeal.color);
                coman.Parameters.AddWithValue("@PSI_USUARIO", oSeal.usuarioCrea);
                coman.Parameters.AddWithValue("@PSI_IP", oSeal.ip);
                //coman.Parameters.AddWithValue("@PSO_MENSAJE", out mensaje);
                //coman.Parameters.AddWithValue("@PSI_DIF", diferencia);
                coman.Parameters.AddWithValue("@PSI_CONSECUTIVO", Gkey);

                var paramMensaje = new SqlParameter("@PSO_MENSAJE", SqlDbType.VarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                coman.Parameters.Add(paramMensaje);

                var paramDiferencia = new SqlParameter("@PSI_DIF", SqlDbType.Bit, 500)
                {
                    Direction = ParameterDirection.Output
                };
                coman.Parameters.Add(paramDiferencia);

                try
                {
                    c.Open();
                    d.Load(coman.ExecuteReader(CommandBehavior.CloseConnection));
                    mensaje = paramMensaje.Value?.ToString() ?? string.Empty;
                    diferencia = bool.Parse(paramDiferencia.Value.ToString());
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    if (c.State == ConnectionState.Open)
                    {
                        c.Close();
                    }
                    c.Dispose();
                }
            }
            return d;
        }
    }


    public class fotoSealMuelleDA : Base
    {
        public fotoSealMuelleDA() : base()
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
            parametros.Add("i_idSelloMuelle", oFoto.idSealValidation);
            parametros.Add("i_ruta", oFoto.ruta);
            parametros.Add("i_estado", oFoto.estado);
            parametros.Add("i_usuarioCrea", oFoto.usuarioCrea);
            parametros.Add("i_usuarioModifica", oFoto.usuarioModifica);

            var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[mty].insertarFotoSelloMuelle", parametros, out OnError);
            if (!db.HasValue || db.Value < 0)
            {
                return null;
            }
            OnError = string.Empty;
            return db.Value;

        }

        public Int64? Save_Update(string xml, out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_xml", xml);

            var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[mty].insertarFotoSelloMuelleNew", parametros, out OnError);
            if (!db.HasValue || db.Value < 0)
            {
                return null;
            }
            OnError = string.Empty;
            return db.Value;

        }
    }
}
