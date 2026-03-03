using ApiModels.AppModels;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRBKWebApiData
{
    [Serializable]
    public class tarjaDetDA : Base
    { 
        public tarjaDetDA() : base()
        {
            //init();
        }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static List<tarjaDet> listadoTarjaDet(long _idTarja, out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_idTarja", _idTarja);
            return sql_puntero.ExecuteSelectControl<tarjaDet>(nueva_conexion, 4000, "[brbk].consultartarjaDet", parametros, out OnError);
        }

        public static tarjaDet GetTarjaDet(long _idTarjaDet)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_idTarjaDet", _idTarjaDet);
            var obj = sql_puntero.ExecuteSelectOnly<tarjaDet>(nueva_conexion, 4000, "[brbk].consultartarjaDet", parametros);
            try
            {
                obj.carga = string.Format("{0}-{1}-{2}", obj.mrn, obj.msn, obj.hsn);
                obj.consigna = string.Format("{0} {1} ", obj.idConsignatario.Trim(), obj.Consignatario);
                obj.Estados = estadosDA.GetEstado(obj.estado);
                obj.producto = productosDA.GetProducto(obj.idProducto);
                obj.condicion = condicionDA.GetCondicion(obj.idCondicion);
                obj.maniobra = obj.producto.Maniobra;
                obj.tarjaCab = tarjaCabDA.GetTarjaCab(long.Parse(obj.idTarja.ToString()));
                obj.Ubicaciones = ubicacionDA.GetUbicacion(obj.ubicacion);
            }
            catch { }

            return obj;
        }

        public static tarjaDet GetTarjaDet(string _mrn, string _msn, string _hsn)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_mrn", _mrn);
            parametros.Add("i_msn", _msn);
            parametros.Add("i_hsn", _hsn);
            var obj = sql_puntero.ExecuteSelectOnly<tarjaDet>(nueva_conexion, 4000, "[brbk].consultarTarjaDetXcarga", parametros);
            try
            {
                obj.carga = string.Format("{0}-{1}-{2}", obj.mrn, obj.msn, obj.hsn);
                obj.consigna = string.Format("{0} {1} ", obj.idConsignatario.Trim(), obj.Consignatario);
                obj.Estados = estadosDA.GetEstado(obj.estado);
                obj.producto = productosDA.GetProducto(obj.idProducto);
                obj.condicion = condicionDA.GetCondicion(obj.idCondicion);
                obj.maniobra = obj.producto.Maniobra;
                obj.tarjaCab = tarjaCabDA.GetTarjaCab(long.Parse(obj.idTarja.ToString()));
            }
            catch { }

            return obj;
        }

        public static List<tarjaDet> GetTarjaDetXNave(string _nave, out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_idNave", _nave);

            var obj = sql_puntero.ExecuteSelectControl<tarjaDet>(nueva_conexion, 4000, "[brbk].consultarTarjaDetPorNave", parametros, out OnError);
            try
            {
                foreach (tarjaDet oDet in obj)
                {
                    oDet.Estados = estadosDA.GetEstado(oDet.estado);
                    oDet.carga = string.Format("{0}-{1}-{2}", oDet.mrn, oDet.msn, oDet.hsn);
                    oDet.consigna = string.Format("{0} {1} ", oDet.idConsignatario.Trim(), oDet.Consignatario);
                }

            }
            catch { }

            return obj;
        }

        public static List<tarjaDet> GetTarjaDetXmrn(string _mrn, string _lugar, out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            try
            {
                parametros.Add("i_idMRN", _mrn);
                parametros.Add("i_lugar", _lugar);
            }
            catch
            {
                if (!parametros.ContainsKey(_mrn))
                {
                    parametros.Clear();
                    parametros.Add("i_idMRN", _mrn);
                    parametros.Add("i_lugar", _lugar);
                }
            }

            var obj = sql_puntero.ExecuteSelectControl<tarjaDet>(nueva_conexion, 4000, "[brbk].consultarTarjaDetPorMRN", parametros, out OnError);
            try
            {
                string error = string.Empty;
                var oEstados = estadosDA.listadoEstados(out error);
                var oProductos = productosDA.listadoProductos(out error);
                var oUbicacion = ubicacionDA.listadoUbicacion(out error);
                var oItems = itemsDA.listadoItems(out error);

                if (obj != null)
                {
                    foreach (tarjaDet oDet in obj)
                    {
                        oDet.Estados = oEstados.Where(a => a.id == oDet.estado).FirstOrDefault();
                        oDet.carga = string.Format("{0}-{1}-{2}", oDet.mrn, oDet.msn, oDet.hsn);
                        oDet.consigna = string.Format("{0} {1} ", oDet.idConsignatario.Trim(), oDet.Consignatario);
                        oDet.producto = oProductos.Where(a => a.id == oDet.idProducto).FirstOrDefault();
                        oDet.producto.Items = oItems.Where(a => a.id == oDet.idItem).FirstOrDefault();
                                                                                                            //productosDA.GetProducto(oDet.idProducto);
                                                                                                            //oDet.condicion = condicionDA.GetCondicion(oDet.idCondicion);
                                                                                                            //oDet.maniobra = oDet.producto.Maniobra;
                                                                                                            //oDet.tarjaCab = tarjaCabDA.GetTarjaCab(long.Parse(oDet.idTarja.ToString()));
                        oDet.Ubicaciones = oUbicacion.Where(a => a.id == oDet.ubicacion).FirstOrDefault();//ubicacionDA.GetUbicacion(oDet.ubicacion);
                    }
                }
            }
            catch { throw; }

            return obj;
        }
    }
}
