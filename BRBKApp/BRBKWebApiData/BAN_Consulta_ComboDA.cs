using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;


namespace BRBKWebApiData
{
    public class BAN_Consulta_ComboDA : Base
    {
        public BAN_Consulta_ComboDA()
        {
            init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<BAN_Consulta_Combo> ConsultarListaNave(out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_Consulta_Combo>(nueva_conexion, 4000, "BAN_Consulta_NaveXMovimiento", parametros, out OnError);
        }
        public static List<BAN_Consulta_Combo> ConsultarListaBodega(string idNave ,out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idNave", idNave);
            return sql_puntero.ExecuteSelectControl<BAN_Consulta_Combo>(nueva_conexion, 4000, "BAN_Consulta_BodegaXMovimientoNave", parametros, out OnError);
        }
        public static List<BAN_Consulta_Combo> ConsultarListaExportadoresPorNaveBodega(string idNave,int? idBodega, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idNave", idNave);
            parametros.Add("i_idBodega", idBodega);
            return sql_puntero.ExecuteSelectControl<BAN_Consulta_Combo>(nueva_conexion, 4000, "BAN_Consulta_ExportadorXMovimientoNave", parametros, out OnError);
        }

        public static List<BAN_Consulta_Combo> ConsultarListaExportadoresPorNave(string idNave, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idNave", idNave);
            return sql_puntero.ExecuteSelectControl<BAN_Consulta_Combo>(nueva_conexion, 4000, "BAN_Consulta_ExportadorXNave", parametros, out OnError);
        }

        public static List<BAN_Consulta_Combo> ConsultarListaBloques(string idNave, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idNave", idNave);
            return sql_puntero.ExecuteSelectControl<BAN_Consulta_Combo>(nueva_conexion, 4000, "BAN_Consulta_BloqueXMovimientoNave", parametros, out OnError);
        }

        public static List<BAN_Consulta_Combo> ConsultarListaBodegaXDespacho(out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_Consulta_Combo>(nueva_conexion, 4000, "BAN_Consulta_BodegaXOrdenesDespacho", null, out OnError);
        }

        public static List<BAN_Consulta_Combo> ConsultarListaNavesVBS(out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_Consulta_Combo>(nueva_conexion, 4000, "BAN_Catalogo_Nave_Consulta", null, out OnError);
        }

        public static List<BAN_Consulta_Combo> ConsultarListaExportadoresVBS(out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_Consulta_Combo>(nueva_conexion, 4000, "BAN_Catalogo_Exportador_Consulta", null, out OnError);
        }

        public static List<BAN_Consulta_Combo> ConsultarListaTipoMovimientoVBS(out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_Consulta_Combo>(nueva_conexion, 4000, "BAN_Catalogo_TipoMovimiento_Consulta", null, out OnError);
        }

        public static List<BAN_Consulta_Combo> ConsultarListaHoldVBS(out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_Consulta_Combo>(nueva_conexion, 4000, "[BAN_Catalogo_Hold_Consultar_Embarque]", null, out OnError);
        }

        public static List<BAN_Consulta_Combo> ConsultarListaPisoVBS(out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_Consulta_Combo>(nueva_conexion, 4000, "[BAN_Catalogo_Piso_Consultar_Embarque]", null, out OnError);
        }

        public static List<BAN_Consulta_Combo> ConsultarListaMarcaVBS(string rucExportador, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_rucCliente", rucExportador);
            return sql_puntero.ExecuteSelectControl<BAN_Consulta_Combo>(nueva_conexion, 4000, "[BAN_Catalogo_Marca_Consultar_Embarque]", parametros, out OnError);
        }
        public static List<BAN_Consulta_Combo> ConsultarListaOrigenVBS(out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_Consulta_Combo>(nueva_conexion, 4000, "[BAN_Catalogo_Origen_Consultar]", null, out OnError);
        }
        public static List<BAN_Consulta_Combo> ConsultarListaBodegasVBS(out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_Consulta_Combo>(nueva_conexion, 4000, "BAN_Consulta_BodegasVBS", null, out OnError);
        }
        public static List<BAN_Consulta_Combo> ConsultarListaBloquesVBS(int? _idBodega, out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            parametros.Add("i_idBodega", _idBodega);
            return sql_puntero.ExecuteSelectControl<BAN_Consulta_Combo>(nueva_conexion, 4000, "BAN_Consulta_BloquesVBS", parametros, out OnError);
        }

        public static List<BAN_Consulta_Combo> ConsultarListaNaveST(out string OnError)
        {
            OnInit("VBS");
            parametros.Clear();
            return sql_puntero.ExecuteSelectControl<BAN_Consulta_Combo>(nueva_conexion, 4000, "BAN_Consulta_NaveST", parametros, out OnError);
        }
    }
}

