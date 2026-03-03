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
    public class despachoDA : Base
    {

        public despachoDA() : base()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<despacho> listadoDespachos(long _idTarjaDet, out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_idTarjaDet", _idTarjaDet);
            return sql_puntero.ExecuteSelectControl<despacho>(nueva_conexion, 4000, "[brbk].consultarDespacho", parametros, out OnError);
        }

        public static despacho GetDespacho(long _id)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_idDespacho", _id);
            var obj = sql_puntero.ExecuteSelectOnly<despacho>(nueva_conexion, 4000, "[brbk].consultarDespacho", parametros);
            try
            {
                obj.tarjaDet = tarjaDetDA.GetTarjaDet(obj.idTarjaDet);
            }
            catch { }
            return obj;
        }

        public Int64? Save_Update(despacho oDesp, out string OnError)
        {
            OnInit("N4Middleware");
            //using (var scope = new System.Transactions.TransactionScope())
            //{
                parametros.Clear();
                parametros.Add("i_idTarjaDet", oDesp.idTarjaDet);
                parametros.Add("i_pase", oDesp.pase);
                parametros.Add("i_mrn", oDesp.mrn);
                parametros.Add("i_msn", oDesp.msn);
                parametros.Add("i_hsn", oDesp.hsn);
                parametros.Add("i_placa", oDesp.placa);
                parametros.Add("i_idchofer", oDesp.idchofer);
                parametros.Add("i_chofer", oDesp.chofer);
                parametros.Add("i_cantidad", oDesp.cantidad);
                parametros.Add("i_observacion", oDesp.observacion);
                parametros.Add("i_estado", oDesp.estado);
                parametros.Add("i_usuarioCrea", oDesp.usuarioCrea);
                parametros.Add("i_usuarioModifica", oDesp.usuarioModifica);
                parametros.Add("i_delivery", oDesp.delivery);
                parametros.Add("i_PRE_GATE_ID", oDesp.PRE_GATE_ID);
                parametros.Add("i_SMDT_xml", oDesp.SMDT_xml);

                var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[brbk].insertarDespacho", parametros, out OnError);
                if (!db.HasValue || db.Value < 0)
                {
                    return null;
                }
                OnError = string.Empty;

                foreach (var oItem in oDesp.Fotos)
                {
                    oItem.idDespacho = db.Value;
                    fotoDespachoDA oFoto = new fotoDespachoDA();
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
