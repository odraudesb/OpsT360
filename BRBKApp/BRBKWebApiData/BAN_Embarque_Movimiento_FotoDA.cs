using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;

namespace BRBKWebApiData
{
    public class BAN_Embarque_Movimiento_FotoDA : Base
    {
        public BAN_Embarque_Movimiento_FotoDA() : base()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<BAN_Embarque_Movimiento_Foto> ConsultarLista(long _id, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idMovimineto", _id);
            return sql_puntero.ExecuteSelectControl<BAN_Embarque_Movimiento_Foto>(nueva_conexion, 4000, "[]", parametros, out OnError);
        }

        public static BAN_Embarque_Movimiento_Foto GetEntidad(long _id)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Embarque_Movimiento_Foto>(nueva_conexion, 4000, "[]", parametros);
            try
            {
                obj.oEmbarque_Movimiento = BAN_Embarque_MovimientoDA.GetEntidad(obj.idMovimiento);
            }
            catch { }
            return obj;
        }

        public Int64? Save_Update(fotoEmbarqueVBS oFotoRec, out string OnError)
        {
            //if (oFotoRec.id > 0)
            //{
            OnInit("VBS");
            //}
            parametros.Clear();
            parametros.Add("i_id", oFotoRec.id);
            parametros.Add("i_idMovimiento", oFotoRec.idMovimiento);
            parametros.Add("i_ruta", oFotoRec.ruta);
            parametros.Add("i_estado", oFotoRec.estado);
            parametros.Add("i_usuarioCrea", oFotoRec.usuarioCrea);
            parametros.Add("i_usuarioModifica", oFotoRec.usuarioModifica);

            var db = sql_puntero.ExecuteInsertUpdateDeleteReturn(nueva_conexion, 6000, "BAN_Embarque_Movimiento_Foto_Insertar", parametros, out OnError);
            if (!db.HasValue || db.Value < 0)
            {
                return null;
            }
            OnError = string.Empty;
            return db.Value;

        }


    }
}
