using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRBKWebApiData
{
    public class fotoDespachoDA : Base
    {
        public fotoDespachoDA() : base()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }
       
        public Int64? Save_Update(fotoDespacho oFoto, out string OnError)
        {
            //if (oFoto.id > 0)
            //{
            OnInit("N4Middleware");
            //}
            parametros.Clear();
            parametros.Add("i_id", oFoto.id);
            parametros.Add("i_idDespacho", oFoto.idDespacho);
            parametros.Add("i_ruta", oFoto.ruta);
            parametros.Add("i_estado", oFoto.estado);
            parametros.Add("i_usuarioCrea", oFoto.usuarioCrea);
            parametros.Add("i_usuarioModifica", oFoto.usuarioModifica);

            var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[brbk].insertarFotoDespacho", parametros, out OnError);
            if (!db.HasValue || db.Value < 0)
            {
                return null;
            }
            OnError = string.Empty;
            return db.Value;

        }
    }
}
