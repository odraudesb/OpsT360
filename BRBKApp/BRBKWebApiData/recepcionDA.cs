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
    public class recepcionDA : Base
    {
        public recepcionDA() : base()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<recepcion> listadoRecepcion(long _idTarjaDet, string _lugar,out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_idTarjaDet", _idTarjaDet);
            parametros.Add("i_lugar", _lugar);
            var obj =  sql_puntero.ExecuteSelectControl<recepcion>(nueva_conexion, 4000, "[brbk].consultarRecepcion", parametros, out OnError);

            string error = string.Empty;
            var oEstados = estadosDA.listadoEstados(out error);
            var oUbicacion = ubicacionDA.listadoUbicacion(out error);
            var oDetalle = tarjaDetDA.GetTarjaDet(_idTarjaDet);

            if (obj != null)
            {
                foreach (recepcion oDet in obj)
                {
                    oDet.Estados = oEstados.Where(a => a.id == oDet.estado).FirstOrDefault();
                    oDet.Ubicaciones = oUbicacion.Where(a => a.id == oDet.ubicacion).FirstOrDefault();
                    oDet.TarjaDet = oDetalle;
                }
            }
            
            return obj;
        }

        public static recepcion GetRecepcion(long _id)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_idRecepcion", _id);
            var obj = sql_puntero.ExecuteSelectOnly<recepcion>(nueva_conexion, 4000, "[brbk].consultarRecepcion", parametros);
            try
            {
                //obj.Grupo = gruposDA.GetGrupos(obj.idGrupo);
                obj.Ubicaciones = ubicacionDA.GetUbicacion(obj.ubicacion);
                obj.TarjaDet = tarjaDetDA.GetTarjaDet(obj.idTarjaDet);
                obj.Estados = estadosDA.GetEstado(obj.estado);
            }catch { }
            return obj;
        }

        public Int64? Save_Update(recepcion oRec,out string OnError)
        {
            OnInit("N4Middleware");
            //using (var scope = new System.Transactions.TransactionScope())
            //{
                parametros.Clear();
                parametros.Add("i_idRecepcion", oRec.idRecepcion);
                parametros.Add("i_idTarjaDet", oRec.idTarjaDet);
                parametros.Add("i_idGrupo", oRec.idGrupo);
                parametros.Add("i_lugar", oRec.lugar);
                parametros.Add("i_cantidad", oRec.cantidad);
                parametros.Add("i_ubicacion", oRec.ubicacion);
                parametros.Add("i_observacion", oRec.observacion);
                parametros.Add("i_estado", oRec.estado);
                parametros.Add("i_usuarioCrea", oRec.usuarioCrea);
                parametros.Add("i_usuarioModifica", oRec.usuarioModifica);

                var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[brbk].insertarRecepcion", parametros, out OnError);
                if (!db.HasValue || db.Value < 0)
                {
                    return null;
                }
                OnError = string.Empty;

                if (oRec.Fotos != null)
                {
                    foreach (var oItem in oRec.Fotos)
                    {
                        oItem.idrecepcion = db.Value;
                        fotoRecepcionDA oFoto = new fotoRecepcionDA();
                        var dbItem = oFoto.Save_Update(oItem, out OnError);

                        if (!dbItem.HasValue || dbItem.Value < 0)
                        {
                            return null;
                        }
                    }
                }
                //scope.Complete();
                return db.Value;
            //}
        }

        public static string GetConfiguracion(string app, string name)
        {
            OnInit("N4Middleware");
            SqlConnection cn = new SqlConnection(nueva_conexion);

            var d = new DataTable();
            using (var c = cn)
            {
                var coman = c.CreateCommand();
                coman.CommandType = CommandType.StoredProcedure;
                coman.CommandText = "[brbk].consultarConfiguracion";
                coman.Parameters.AddWithValue("i_module", app);
                coman.Parameters.AddWithValue("i_name", name);
                try
                {
                    c.Open();
                    d.Load(coman.ExecuteReader(CommandBehavior.CloseConnection));
                }
                catch
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
            if (d != null)
            {
                if (d.Rows.Count > 0)
                {
                    return d.Rows[0][0].ToString();
                }
            }
            return string.Empty;



       


        }


    }
}
