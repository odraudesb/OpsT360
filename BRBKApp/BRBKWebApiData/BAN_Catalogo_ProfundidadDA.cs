using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;

namespace BRBKWebApiData
{
    public class BAN_Catalogo_ProfundidadDA : Base
    {
        public BAN_Catalogo_ProfundidadDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<BAN_Catalogo_Profundidad> ConsultarLista(out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_Catalogo_Profundidad>(nueva_conexion, 8000, "[BAN_Catalogo_Profundidad_Consultar]", null, out OnError);
        }

        public static List<BAN_Catalogo_Profundidad> ConsultarListaDisponibles(int _idbodega, int _idBloque, int _idFila, int _idAltura ,out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idBodega", _idbodega);
            parametros.Add("i_idBloque", _idBloque);
            parametros.Add("i_idFila", _idFila);
            parametros.Add("i_idAltura", _idAltura);
            return sql_puntero.ExecuteSelectControl<BAN_Catalogo_Profundidad>(nueva_conexion, 8000, "[BAN_Consultar_SlotDisponible]", parametros, out OnError);
        }

        public static BAN_Catalogo_Profundidad GetEntidad(int? _id)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_id", _id);
            var obj = sql_puntero.ExecuteSelectOnly<BAN_Catalogo_Profundidad>(nueva_conexion, 4000, "[BAN_Catalogo_Profundidad_Consultar]", parametros);
            return obj;
        }
    }
}
