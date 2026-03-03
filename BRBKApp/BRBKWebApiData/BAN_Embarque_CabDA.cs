using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRBKWebApiData
{
    public  class BAN_Embarque_CabDA : Base
    {
        public BAN_Embarque_CabDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<BAN_Embarque_Cab> ConsultarLista(string idNave, string idExportador,  out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idNave", idNave);
            parametros.Add("i_idExportador", idExportador);
            return sql_puntero.ExecuteSelectControl<BAN_Embarque_Cab>(nueva_conexion, 8000, "[BAN_Embarque_Cab_Consultar]", parametros, out OnError);
        }

        public static BAN_Embarque_Cab GetEntidad(long _id)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idEmbarqueCab", _id);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Embarque_Cab>(nueva_conexion, 4000, "[BAN_Embarque_Cab_Consultar]", parametros);
            return obj;
        }

        public Int64? Save_Update(BAN_Embarque_Cab oEntidad, out string OnError)
        {
            OnInit("VBS");
            //using (var scope = new System.Transactions.TransactionScope())
            //{
            parametros.Clear();
            parametros.Add("i_idEmbarqueCab", oEntidad.idEmbarqueCab);  
            parametros.Add("i_barcode", oEntidad.barcode);
            parametros.Add("i_idNave", oEntidad.idNave);
            parametros.Add("i_nave", oEntidad.nave);
            parametros.Add("i_idExportador", oEntidad.idExportador);
            parametros.Add("i_Exportador", oEntidad.Exportador);
            parametros.Add("i_estado", oEntidad.estado);
            parametros.Add("i_usuarioCrea", oEntidad.usuarioCrea);
            parametros.Add("i_usuarioModifica", oEntidad.usuarioModifica);

            var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[BAN_Embarque_Cab_Insertar]", parametros, out OnError);
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
