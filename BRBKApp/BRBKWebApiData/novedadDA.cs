using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace BRBKWebApiData
{
    public class novedadDA : Base
    {
        
        public novedadDA() : base()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<novedad> listadoNovedades(long _idRecepcion, out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_idRecepcion", _idRecepcion);
            return sql_puntero.ExecuteSelectControl<novedad>(nueva_conexion, 4000, "[brbk].consultarNovedades", parametros, out OnError);
        }

        public static novedad GetNovedad(long _id)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_idRecepcion", _id);
            var obj = sql_puntero.ExecuteSelectOnly<novedad>(nueva_conexion, 4000, "[brbk].consultarNovedades", parametros);
            try
            {
                obj.Recepcion = recepcionDA.GetRecepcion(obj.idRecepcion);
                obj.Estados = estadosDA.GetEstado(obj.estado);
            }catch { }
            return obj;
        }

        public Int64? Save_Update(novedad oNov, out string OnError)
        {
            OnInit("N4Middleware");
            //using (var scope = new System.Transactions.TransactionScope())
            //{
                parametros.Clear();
                parametros.Add("i_idNovedad", oNov.idNovedad);
                parametros.Add("i_idRecepcion", oNov.idRecepcion);
                parametros.Add("i_fecha", oNov.fecha);
                parametros.Add("i_descripcion", oNov.descripcion);
                parametros.Add("i_estado", oNov.estado);
                parametros.Add("i_usuarioCrea", oNov.usuarioCrea);
                parametros.Add("i_usuarioModifica", oNov.usuarioModifica);

                var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[brbk].insertarNovedad", parametros, out OnError);
                if (!db.HasValue || db.Value < 0)
                {
                    return null;
                }
                OnError = string.Empty;

                foreach (var oItem in oNov.Fotos)
                {
                    oItem.idnovedad = db.Value;
                    fotoNovedadDA oFoto = new fotoNovedadDA();
                    var dbItem = oFoto.Save_Update(oItem, out OnError);

                    if (!dbItem.HasValue || dbItem.Value < 0)
                    {
                        return null;
                    }
                }
                //scope.Complete();
                return db.Value;
            //}

        }
    }
}
