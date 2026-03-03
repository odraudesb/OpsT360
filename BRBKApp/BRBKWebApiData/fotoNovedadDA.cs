using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRBKWebApiData
{
    public class fotoNovedadDA : Base
    {
        public fotoNovedadDA() : base()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<fotoNovedad> listadoFotosNovedad(long _idNovedad, out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_idNovedad", _idNovedad);
            return sql_puntero.ExecuteSelectControl<fotoNovedad>(nueva_conexion, 4000, "[brbk].consultarFotoNovedades", parametros, out OnError);
        }

        public static fotoNovedad GetFotoNovedad(long _id)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<fotoNovedad>(nueva_conexion, 4000, "[brbk].consultarFotoNovedades", parametros);
            try
            {
                obj.Novedad = novedadDA.GetNovedad(obj.idnovedad);
                obj.Estados = estadosDA.GetEstado(obj.estado);
            }catch { }
            return obj;
        }

        public Int64? Save_Update(fotoNovedad oFotoNov, out string OnError)
        {
            //if (oFotoNov.id > 0)
            //{
                OnInit("N4Middleware");
            //}
            parametros.Clear();
            parametros.Add("i_id", oFotoNov.id);
            parametros.Add("i_idNovedad", oFotoNov.idnovedad);
            parametros.Add("i_ruta", oFotoNov.ruta);
            parametros.Add("i_estado", oFotoNov.estado);
            parametros.Add("i_usuarioCrea", oFotoNov.usuarioCrea);
            parametros.Add("i_usuarioModifica", oFotoNov.usuarioModifica);

            var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "[brbk].insertarFotoNovedad", parametros, out OnError);
            if (!db.HasValue || db.Value < 0)
            {
                return null;
            }
            OnError = string.Empty;
            return db.Value;

        }
    }
}
