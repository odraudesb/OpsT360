using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRBKWebApiData
{
    public class BAN_Embarque_MovimientoDA : Base
    {
        public BAN_Embarque_MovimientoDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<BAN_Embarque_Movimiento> ConsultarLista(long idEmbarqueCab, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idEmbarqueCab", idEmbarqueCab);
            return sql_puntero.ExecuteSelectControl<BAN_Embarque_Movimiento>(nueva_conexion, 8000, "[BAN_Embarque_Movimiento_Consultar]", parametros, out OnError);
        }

        public static BAN_Embarque_Movimiento GetEntidad(long _id)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idEmbarqueMovimiento", _id);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Embarque_Movimiento>(nueva_conexion, 4000, "[BAN_Embarque_Movimiento_Consultar]", parametros);
            return obj;
        }

        public Int64? Save_Update(BAN_Embarque_Movimiento oEntidad, out string OnError)
        {
            OnInit("VBS");
            //using (var scope = new System.Transactions.TransactionScope())
            //{
                parametros.Clear();
                parametros.Add("i_idEmbarqueMovimiento", oEntidad.idEmbarqueMovimiento);
                parametros.Add("i_idEmbarqueCab", oEntidad.idEmbarqueCab);
                parametros.Add("i_origen", oEntidad.origen);
                parametros.Add("i_codigoCaja", oEntidad.codigoCaja);
                parametros.Add("i_idHold", oEntidad.idHold);
                parametros.Add("i_idPiso", oEntidad.idPiso);
                parametros.Add("i_idMarca", oEntidad.idMarca);
                parametros.Add("i_idModalidad", oEntidad.idModalidad);
                parametros.Add("i_box", oEntidad.box);
                parametros.Add("i_tipo", oEntidad.tipo);
                parametros.Add("i_idtipoMovimiento", oEntidad.idtipoMovimiento);
                parametros.Add("i_comentario", oEntidad.comentario);
                parametros.Add("i_estado", oEntidad.estado);
                parametros.Add("i_usuarioCrea", oEntidad.usuarioCrea);
                parametros.Add("i_usuarioModifica", oEntidad.usuarioModifica);

                var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[BAN_Embarque_Movimiento_Insertar]", parametros, out OnError);
                if (!db.HasValue || db.Value < 0)
                {
                    return null;
                }
                OnError = string.Empty;

                if (oEntidad.Fotos != null)
                {
                    foreach (var oItem in oEntidad.Fotos)
                    {
                        oItem.idMovimiento = db.Value;
                        BAN_Embarque_Movimiento_FotoDA oFoto = new BAN_Embarque_Movimiento_FotoDA();
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

    }
}
