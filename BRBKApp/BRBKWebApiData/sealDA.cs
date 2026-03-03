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
    public class sealDA : Base
    {
        public sealDA() : base()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static sealValidation GetSello(sealValidation _sello)//(string _container, string _seal, string _user)
        {
            sealValidation oSello;
            OnInit("N4Middleware");
            using (var scope = new System.Transactions.TransactionScope())
            {
                parametros.Clear();
                parametros.Add("i_container", _sello.container);
                parametros.Add("i_seal", _sello.seals);
                parametros.Add("i_usuarioCrea", _sello.usuarioCrea);
                parametros.Add("i_addposition", _sello.addposition);
                parametros.Add("i_position", _sello.position);
                parametros.Add("i_idWorkPosition", _sello.idWorkPosition);
                parametros.Add("i_xmlN4", _sello.xmlN4);
                parametros.Add("i_respuestaN4", _sello.respuestaN4);
                parametros.Add("i_referencia", _sello.referencia);
                parametros.Add("i_gkey", _sello.gkey);

                oSello = sql_puntero.ExecuteSelectOnly<sealValidation>(nueva_conexion, 4000, "mty.consultarSellos", parametros);
                
                if (oSello != null)
                {
                    foreach (var oItem in _sello.Fotos)
                    {
                        oItem.idSealValidation = (long)oSello.id;
                        sealDA oFoto = new sealDA();
                        string OnError = string.Empty;
                        var dbItem = oFoto.Save_Update(oItem, out OnError);

                        //if (!dbItem.HasValue || dbItem.Value < 0)
                        //{
                        //    return null;
                        //}
                    }
                }
                
                scope.Complete();
            }
            
            return oSello;
        }

        public static Int64? UpdateSello(sealValidation _sello, out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_id", _sello.id);
            parametros.Add("i_usuarioModifica", _sello.usuarioCrea);
            parametros.Add("i_addPosition", _sello.addposition);
            parametros.Add("i_position", _sello.position);
            parametros.Add("i_idWorkPosition", _sello.idWorkPosition);
            parametros.Add("i_xmlN4", _sello.xmlN4);
            parametros.Add("i_respuestaN4", _sello.respuestaN4);
            parametros.Add("i_referencia", _sello.referencia + '-' + _sello.grua);

            var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "mty.updateWorkPosition", parametros, out OnError);
            if (!db.HasValue || db.Value < 0)
            {
                return null;
            }
            OnError = string.Empty;
            return db.Value;
        }

        public Int64? Save_Update(fotoSealValidation oFotoSello, out string OnError)
        {
            if (oFotoSello.id > 0)
            {
                OnInit("N4Middleware");
            }
            parametros.Clear();
            parametros.Add("i_id", oFotoSello.id);
            parametros.Add("i_idSealValidation", oFotoSello.idSealValidation);
            parametros.Add("i_ruta", oFotoSello.ruta);
            parametros.Add("i_estado", oFotoSello.estado);
            parametros.Add("i_usuarioCrea", oFotoSello.usuarioCrea);
            parametros.Add("i_usuarioModifica", oFotoSello.usuarioModifica);

            var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[mty].[insertarFotoSello]", parametros, out OnError);
            if (!db.HasValue || db.Value < 0)
            {
                return null;
            }
            OnError = string.Empty;
            return db.Value;

        }


    }
}
