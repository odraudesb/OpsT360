using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRBKWebApiData
{
    public class fotoRecepcionDA : Base
    {
        public fotoRecepcionDA() : base()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<fotoRecepcion> listadoFotoRecepcion(long _idrecepcion, out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_idrecepcion", _idrecepcion);
            return sql_puntero.ExecuteSelectControl<fotoRecepcion>(nueva_conexion, 4000, "[brbk].consultarFotoRecepcion", parametros, out OnError);
        }

        public static fotoRecepcion GetFotoRecepcion(long _id)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<fotoRecepcion>(nueva_conexion, 4000, "[brbk].consultarFotoRecepcion", parametros);
            try
            {
                obj.Recepcion = recepcionDA.GetRecepcion(obj.idrecepcion);
                obj.Estados = estadosDA.GetEstado(obj.estado);
            }catch { }
            return obj;
        }

        public Int64? Save_Update(fotoRecepcion oFotoRec,out string OnError)
        {

            //if (oFotoRec.id > 0)
            //{
                OnInit("N4Middleware");
            //}
            parametros.Clear();
            parametros.Add("i_id", oFotoRec.id);
            parametros.Add("i_idRecepcion", oFotoRec.idrecepcion);
            parametros.Add("i_ruta", oFotoRec.ruta);
            parametros.Add("i_estado", oFotoRec.estado);
            parametros.Add("i_usuarioCrea", oFotoRec.usuarioCrea);
            parametros.Add("i_usuarioModifica", oFotoRec.usuarioModifica);

            var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[brbk].insertarFotoRecepcion", parametros, out OnError);
            if (!db.HasValue || db.Value < 0)
            {
                return null;
            }
            OnError = string.Empty;
            return db.Value;

        }

    }
}
